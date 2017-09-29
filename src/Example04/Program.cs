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
using System.Text;
using System.Threading.Tasks;

namespace SearchAThing.Sci.Examples
{

    class Program
    {

        static void _Main(string[] args)
        {
            const string python_imports = @"
import numpy as np
";

            var sb = new StringBuilder();
            sb.AppendLine("print(np.mgrid[0:5,0:5])");

            var python = new PythonPipe(python_imports);

            var res = python.Exec(sb.ToStringWrapper());

            Console.WriteLine($"Execution of [{sb}] result in follow\r\n{res}");

            python.Dispose();
        }

        /// <summary>
        /// test speed
        /// </summary>        
        static void Main(string[] args)
        {
            const string python_imports = @"
import numpy as np
";

            var src = @"print(np.mgrid[0:5,0:5])";

            var python = new PythonPipe(python_imports);

            var sw = new Stopwatch();
            sw.Start();
            {
                var res = python.Exec(new StringWrapper() { str = src });
            }
            sw.Stop();
            Console.WriteLine($"First execution {sw.Elapsed}");

            //python.Recycle();

            sw.Reset();
            sw.Start();
            {
                var res = python.Exec(new StringWrapper() { str = src });
            }
            sw.Stop();
            Console.WriteLine($"Second execution {sw.Elapsed}");

            python.Dispose();
        }

    }

}