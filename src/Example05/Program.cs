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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using netDxf.Entities;
using netDxf;
using netDxf.Header;
using SearchAThing;
using static System.Math;
using netDxf.Tables;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {
            var dxf = new DxfDocument();
            EntityObject eo = null;

            const double ptsize = .2;
            const int N = 30000;
            const double L = 100;

            var layerSource = new Layer("source") { };
            var layerInside = new Layer("inside") { Color = AciColor.Magenta };

            var pts = Vector3D.Random(N, L).Where(p => p.Length <= L / 2);

            pts.Foreach(w => dxf.DrawStar(w, ptsize, layerSource));

            var ds = new DiscreteSpace<Line>(.1, dxf.Lines, (p) => p.ToLine3D().MidPoint, 3);
            var q = ds.GetItemsAt(new Vector3D(0, 0, 0), L / 6).ToList();
            q.SetLayer(layerInside);

            var bbox = q.SelectMany(r => r.ToLine3D().Points).BBox();
            var faces = bbox.DrawCuboid(dxf, layerInside);

            //dxf.DrawingVariables.PdMode = PointShape.Plus;
            dxf.Viewport.ShowGrid = false;

            dxf.Save("discrete-space-test.dxf");
            Process.Start("discrete-space-test.dxf");
        }

    }

}