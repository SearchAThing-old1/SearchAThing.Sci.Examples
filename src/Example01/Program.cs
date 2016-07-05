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

namespace SearchAThing.Sci.Examples
{

    [DataContract(IsReference = true)]
    class SampleProject : Project
    {

        public SampleProject() : base(null)
        {

        }

        [DataMember]
        public Measure Offset { get; set; }

        [DataMember]
        public Measure Force { get; set; }

    };

    class Program
    {

        static void Main(string[] args)
        {
            var binary = false;

            var offset = .12 * MUCollection.Length.m;
            var force = 1e-3 * MUCollection.Force.kN;

            // write a project to xml
            {                
                var knownTypes = new List<Type>() { typeof(SampleProject) };

                {
                    var prj = new SampleProject();
                    prj.Offset = offset;
                    prj.Force = force;
                    prj.Save("test.xml", binary, knownTypes);
                }
            }

            // read from xml
            {
                var prj = "test.xml".DeserializeFile<SampleProject>(binary);
                Console.WriteLine($"prj read back data");
                Console.WriteLine($"offset={prj.Offset} force={prj.Force}");
            }

        }

    }

}
