using System;
using System.Diagnostics;
using System.Linq;
using netDxf.Tables;
using netDxf;

namespace SearchAThing.Sci.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new netDxf.DxfDocument();

            var N = 20;
            var l = 100;
            var samples_rows = 10;
            var samples_cols = 10;

            var lay_bbox = new Layer("bbox") { Color = AciColor.Yellow };
            var lay_hull = new Layer("hull") { Color = AciColor.Cyan };

            var rnd = new Random();

            var sr = 0;
            var sc = 0;
            for (int i = 0; i < samples_rows * samples_cols; ++i)
            {
                var delta = new Vector3D(sc * l * 1.2, sr * l * 1.2);

                var pts = Vector3D.Random(N, 0, l, 0, l, 0, 0, random: rnd).Select(w => w + delta).ToList();

                foreach (var p in pts)
                {
                    doc.AddEntity(p);
                }

                var bbox = pts.BBox();
                bbox.DrawCuboid(doc, lay_bbox);

                var hull = pts.DummyConvexHull(.1).PolygonSegments(.1);
                foreach (var seg in hull)
                {
                    doc.AddEntity(seg, lay_hull);
                }

                ++sc;
                if (sc == samples_cols)
                {
                    sc = 0;
                    ++sr;
                }
            }

            doc.DrawingVariables.PdMode = netDxf.Header.PointShape.CircleCross;
            doc.DrawingVariables.PdSize = 1;
            doc.Viewport.ShowGrid = false;
            doc.Save("out.dxf", true);

            Process.Start("out.dxf");
        }
    }
}
