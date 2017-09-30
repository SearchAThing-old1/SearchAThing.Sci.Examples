#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
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
using netDxf.Entities;
using System.Globalization;
using static System.Math;
using SearchAThing.Sci.Examples;
using netDxf.Tables;
using SearchAThing;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {
            const double tol = 1e-6;

            const int N = 500; // nr. of points
            const double W = 2000; // ellipse width
            const double H = 1000; // ellipse height
            const double boundaryPaddingPercent = 0;
            const double ellipseFlatness = .1; // ellipse polygonalized flatness
            var origin = Vector3D.Zero;

            const double origPointRadius = 1;

            var boundary = Polygon.EllipseToPolygon2D(origin, W, H, ellipseFlatness).ToList();

            var rndPts = Vector3D.Random(N,
                (-W / 2) * (1 - boundaryPaddingPercent), (W / 2) * (1 - boundaryPaddingPercent), // xrange
                (-H / 2) * (1 - boundaryPaddingPercent), (H / 2) * (1 - boundaryPaddingPercent), // yrange
                0, 0, // zrange
                seed: 2)
                .Select(v => v + origin)
                .ToList();

            var vs = rndPts
                .Where(v => boundary.ContainsPoint(tol, v))
                .ToHashSet();

            var dxf = new DxfDocument();
            EntityObject eo = null;

            var layerAllRndPoints = new Layer("all_rnd_points") { Color = AciColor.DarkGray };
            var layerConvexPoints = new Layer("convex_points") { Color = AciColor.Cyan };
            var layerEllipseInsideRndPoints = new Layer("ellipse_rnd_points") { Color = AciColor.Green };
            var layerBoundary = new Layer("bounday") { Color = AciColor.Yellow };
            var layerBboxInsidePoints = new Layer("bbox_inside_points") { Color = AciColor.Magenta };

            // draw all rnd points
            foreach (var p in rndPts)
            {
                dxf.AddEntity(eo = new Circle(p, origPointRadius));
                if (vs.Contains(p))
                    eo.Layer = layerEllipseInsideRndPoints;
                else
                    eo.Layer = layerAllRndPoints;
            }

            var bbox_rnd_inside = vs.BBox();
            bbox_rnd_inside.DrawCuboid(dxf, layerBboxInsidePoints);
            var cxhull = vs.ConvexHull2D();

            // draw boundary
            dxf.AddEntity(boundary.ToLwPolyline(tol), layerBoundary);

            // draw convex hull
            cxhull.PolygonSegments(tol).Foreach(w => dxf.AddEntity(w, layerConvexPoints));

            dxf.Viewport.ShowGrid = false;

            dxf.Save("test.dxf", true);

            Process.Start("test.dxf");
        }

    }

}
