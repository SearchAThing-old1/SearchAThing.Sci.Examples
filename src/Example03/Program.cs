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
using SearchAThing;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {
            const double tol = 1e-3;

            const int N = 500; // nr. of points
            const double W = 2000; // ellipse width
            const double H = 1000; // ellipse height
            const double boundaryPaddingPercent = 0;
            const double ellipseFlatness = .1; // ellipse polygonalized flatness
            var origin = Vector3D.Zero;// new Vector3D(400, 200, 0);

            const double origPointRadius = 1;
            const double boundarySplitPtRadius = 1;
            const double convexPointRadius = 2;
            const double failedPointRadius = 4;

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
                .ToList();

            var dxf = new DxfDocument();
            EntityObject eo = null;

            var disableBoundary = false; // TO TEST

            var layerAllRndPoints = new Layer("all_rnd_points") { Color = AciColor.DarkGray, IsVisible = false };
            var layerOrigPoints = new Layer("orig_points") { Color = AciColor.Magenta };
            var layerConvexPoints = new Layer("convex_points") { Color = AciColor.Cyan };
            var layerMeshPoly = new Layer("mesh_poly") { Color = AciColor.DarkGray };
            var layerMeshPolyCv = new Layer("mesh_poly_cv") { Color = AciColor.DarkGray };
            var layerClosureLines = new Layer("closure") { Color = AciColor.Red, IsVisible = disableBoundary };
            var layerTriangles = new Layer("triangles") { Color = AciColor.Blue };
            var layerBoundary = new Layer("bounday") { Color = AciColor.Yellow, IsVisible = !disableBoundary };
            var layerAllSegs = new Layer("all_segs") { Color = AciColor.Green, IsVisible = !disableBoundary };
            var layerPtFailed = new Layer("pt_failed") { Color = AciColor.Yellow };

            if (vs.Count > 0)
            {
                var failedPoints = new List<Vector3D>();

                var mesh2d = new Mesh2D(1e-2, vs, boundary, failedPoints,
                     // default 10
                     boundaryPolyIntersectToleranceFactor: 10,

                     // default: 1e-1
                     boundaryPolyBooleanMapToleranceFactor: 1e-1,

                     // default: 1
                     closedPolyToleranceFactor: 2,

                     disableBoundary: disableBoundary
                     );

                var layerBoundarySplitPts = new Layer("boundary_split") { Color = AciColor.Red, IsVisible = failedPoints.Count > 0 };
                if (failedPoints.Count > 0) layerClosureLines.IsVisible = true;

                // draw orig points with mesh
                foreach (var p in vs)
                {
                    dxf.AddEntity(eo = new Circle(p, origPointRadius));
                    eo.Layer = layerOrigPoints;

                    var poly = mesh2d.PointToPoly(p);
                    if (poly != null)
                    {
                        dxf.AddEntity(eo = poly.Poly.ToLwPolyline(tol));
                        if (poly.PointIsConvexHull)
                            eo.Layer = layerMeshPolyCv;
                        else
                            eo.Layer = layerMeshPoly;
                    }
                }

                // draw convex hull
                foreach (var p in mesh2d.ConvexHull)
                {
                    dxf.AddEntity(eo = new Circle(p, convexPointRadius));
                    eo.Layer = layerConvexPoints;
                }

                // draw failed pts
                foreach (var p in failedPoints)
                {
                    dxf.AddEntity(eo = new Circle(p, failedPointRadius));
                    eo.Layer = layerPtFailed;
                }

                // draw boundary split pts
                foreach (var p in mesh2d.BoundarySplitPts)
                {
                    dxf.AddEntity(eo = new Circle(p, boundarySplitPtRadius));
                    eo.Layer = layerBoundarySplitPts;
                }

                // draw closure lines
                foreach (var s in mesh2d.Closures)
                {
                    dxf.AddEntity(eo = new Line(s.From, s.To));
                    eo.Layer = layerClosureLines;
                }

                // draw voronoi triangles
                foreach (var t in mesh2d.Triangles)
                {
                    dxf.AddEntity(eo = t.ToLwPolyline(tol));
                    eo.Layer = layerTriangles;
                }

                // draw all segs
                foreach (var s in mesh2d.AllSegs)
                {
                    dxf.AddEntity(eo = new Line(s.From, s.To));
                    eo.Layer = layerAllSegs;
                }
            }
            else
            {
                // draw orig points with mesh
                foreach (var p in vs)
                {
                    dxf.AddEntity(eo = new Circle(p, origPointRadius));
                    eo.Layer = layerOrigPoints;
                }

                layerAllRndPoints.IsVisible = true;
            }

            // draw all rnd points
            foreach (var p in rndPts)
            {
                dxf.AddEntity(eo = new Circle(p, origPointRadius));
                eo.Layer = layerAllRndPoints;
            }

            // draw boundary
            dxf.AddEntity(eo = boundary.ToLwPolyline(tol));
            eo.Layer = layerBoundary;

            // setup layers
            //dxf.Layers.Foreach(w => w.IsVisible = false);
            //layerAllSegs.IsVisible = true;
            layerAllSegs.IsVisible = false;

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
