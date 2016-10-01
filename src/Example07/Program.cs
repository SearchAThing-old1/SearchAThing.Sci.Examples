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
using SearchAThing;
using netDxf;
using System.Diagnostics;
using netDxf.Entities;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {

            var dxf = new DxfDocument();

            var line1 = new Line3D(new Vector3D(19.2325, -3.2294), new Vector3D(18.8826, -0.2294));
            var line2 = new Line3D(new Vector3D(19.2325, -3.2294), new Vector3D(21.2355, -2.1642));
            var pt = new Vector3D(21.2355, -2.1642);
            dxf.AddEntity(new Circle(pt, .1).SetColor(AciColor.Green));
            
            dxf.AddEntity(line1.ToLine().SetColor(AciColor.Red));
            dxf.AddEntity(line2.ToLine().SetColor(AciColor.Red));
            
            // build circle tangent line1,line2 (between line1-line2) through given point pt contained in line1 or line2
            var c = new Circle3D(1e-4, line1, line2, pt);            
            dxf.AddEntity(new Circle(c.CS.Origin, c.Radius).SetColor(AciColor.Yellow));

            dxf.Viewport.ShowGrid = false;
            dxf.Save("test.dxf");
            Process.Start("test.dxf");


        }

    }

}