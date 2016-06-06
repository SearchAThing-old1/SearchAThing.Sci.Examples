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
using netDxf;
using System.Diagnostics;
using MIConvexHull;
using netDxf.Entities;
using System.Globalization;
using static System.Math;
using SearchAThing.Sci.Examples;
using netDxf.Tables;

namespace SearchAThing.Sci.Examples
{

    class Program
    {               

        static void Main(string[] args)
        {
            var tol = 1e-3;

            var vs = Vector3D.From2DCoords(138.057658280273, 311.253742925475,
                262.868103460813, 203.998850055038,
                19.0415792255856, 252.148940345342,
                310.67393431937, 107.172264068933,
                389.416187950138, 373.369659238201,
                121.334824534755, 77.27920174472,
                446.644815125803, 438.541181124067,
                492.844538294172, 108.737732800067,
                452.47291934326, 265.714573564806,
                254.554713496265, 64.5225288181205).ToList();

            var mesh2d = new Mesh2D(1e-2, vs, Vector3D.From2DCoords(
                -45, -45,
                550, -45,
                550, 550,
                -45, 550
                ));

            // draw

            var dxf = new DxfDocument();
            EntityObject eo = null;

            const double origPointRadius = 5;
            const double convexPointRadius = 10;
            var layerOrigPoints = new Layer("orig_points") { Color = AciColor.Magenta };
            var layerConvexPoints = new Layer("convex_points") { Color = AciColor.Cyan };
            var layerMeshPoly = new Layer("mesh_poly") { Color = AciColor.DarkGray };
            var layerClosureLines = new Layer("closure") { Color = AciColor.Red };
            var layerTriangles = new Layer("triangles") { Color = AciColor.Blue };
            var layerBoundary = new Layer("bounday") { Color = AciColor.Yellow };

            // draw orig points with mesh
            foreach (var p in vs)
            {
                dxf.AddEntity(eo = new Circle(p.ToVector3(), origPointRadius));
                eo.Layer = layerOrigPoints;

                var poly = mesh2d.VectorToPoly(p);
                if (poly != null)
                {
                    dxf.AddEntity(eo = poly.ToLwPolyline(tol));
                    eo.Layer = layerMeshPoly;
                }
            }

            // draw convex hull
            foreach (var p in mesh2d.ConvexHull)
            {
                dxf.AddEntity(eo = new Circle(p.ToVector3(), convexPointRadius));
                eo.Layer = layerConvexPoints;
            }

            // draw closure lines
            foreach (var s in mesh2d.Closures)
            {
                dxf.AddEntity(eo = new Line(s.From.ToVector3(), s.To.ToVector3()));
                eo.Layer = layerClosureLines;
            }

            // draw voronoi triangles
            foreach (var t in mesh2d.Triangles)
            {
                dxf.AddEntity(eo = t.ToLwPolyline(tol));
                eo.Layer = layerTriangles;
            }

            // draw boundary
            dxf.AddEntity(eo = mesh2d.Boundary.ToLwPolyline(tol));
            eo.Layer = layerBoundary;

            dxf.Viewport.ShowGrid = false;

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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1})", V.X, V.Y);
        }
    }

    public class Cell : TriangulationCell<Vertex, Cell>
    {

        public override string ToString()
        {
            return string.Format($"cvs={Vertices[0].ToString()} {Vertices[1].ToString()} {Vertices[2].ToString()}");
        }

    }

}
