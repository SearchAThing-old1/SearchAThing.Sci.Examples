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

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void Main(string[] args)
        {
            var wgs84 = CRSCatalog.WGS84;

            var rome_wgs84 = new Vector3D(12.482269, 41.895623);

            var MonteMarioZone1 = CRSCatalog.CRSList["EPSG:3003"];

            var rome_mmz1 = rome_wgs84.Project(wgs84, MonteMarioZone1);

            Console.WriteLine($"rome wgs84 {rome_wgs84} -> montemario1 {rome_mmz1}");

            Console.WriteLine($"rome monteomario1 {rome_mmz1} -> wgs84 {rome_mmz1.Project(MonteMarioZone1, wgs84)}");


        }

    }

}