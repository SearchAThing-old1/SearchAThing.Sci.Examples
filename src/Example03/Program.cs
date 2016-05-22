#region SearchAThing.Sci, Copyright(C) 2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SearchAThing.Core;
using netDxf;
using System.Diagnostics;
using MIConvexHull;
using netDxf;
using netDxf.Entities;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {
            var dxf = new DxfDocument();

            var rnd = new Random();
            var vs = new List<Vertex>();

            netDxf.Entities.EntityObject eo = null;

            for (int i = 0; i < 1000; ++i)
            {
                var v = new Vertex(new Vector3D(rnd.NextDouble() * 1000, rnd.NextDouble() * 1000, 0));
                vs.Add(v);
                dxf.AddEntity(eo = new Circle(v.V.ToVector3(), 1));
                eo.Color = AciColor.Magenta;
            }

            var config = new TriangulationComputationConfig
            {
                PointTranslationType = PointTranslationType.TranslateInternal,
                PlaneDistanceTolerance = 1e-6,
                PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(1e-6, 0)
            };

            var voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vs, config);            

            foreach (var edge in voronoiMesh.Edges)
            {
                var from = edge.Source.Vertices.Select(w => w.V).CircleBy3Points();
                var to = edge.Target.Vertices.Select(w => w.V).CircleBy3Points();

                dxf.AddEntity(eo = new Line { StartPoint = from.Center.ToVector3(), EndPoint = to.Center.ToVector3() });
            }
            
            dxf.Save("test.dxf");

            Process.Start("test.dxf");
        }

    }

    public class Vertex : IVertex
    {
        public Vector3D V { get; private set; }

        public Vertex(Vector3D v)
        {
            V = v;
        }

        double[] _position;
        public double[] Position
        {
            get
            {
                if (_position == null) _position = new double[] { V.X, V.Y };

                return _position;
            }
        }
    }

    public class Cell : TriangulationCell<Vertex, Cell>
    {



    }

}
