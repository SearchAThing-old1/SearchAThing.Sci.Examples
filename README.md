# SearchAThing.Sci.Examples

Examples for SearchAThing.Sci

## Build

There are many dependencies between projects in relative path directly from other source repositories,
in order to build successfully its suggested to clone follow repository [SearchAThing](https://github.com/devel0/SearchAThing) containing all of them.

## Unit Tests

[Unit Tests](https://github.com/devel0/SearchAThing.UnitTest/blob/master/src/Sci.cs) are a good source for misc samples.

## Mesh2d, Voronoi, Convex hull
- [Example08](/src/Example08/Program.cs) : create some random point island and compute quick (dummy) hull
![img](/doc/Example08.PNG)

- [Example03](/src/Example03/Program.cs) : create a mesh2d given some input points
![img](/doc/Example03.PNG)

## Circle tan to 2 line and 1 pt
- [Example07](/src/Example07/Program.cs)
![img](/doc/Example07.PNG)

## PythonWrapper
- [Example04](/src/Example04/Program.cs) : invoke python using PythonWrapper helper
Follow code
```csharp
static void Main(string[] args)
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
```
produces this output:
```
Execution of [import numpy as np
np.mgrid[0:5,0:5]] result in follow
array([[[0, 0, 0, 0, 0],
        [1, 1, 1, 1, 1],
        [2, 2, 2, 2, 2],
        [3, 3, 3, 3, 3],
        [4, 4, 4, 4, 4]],

       [[0, 1, 2, 3, 4],
        [0, 1, 2, 3, 4],
        [0, 1, 2, 3, 4],
        [0, 1, 2, 3, 4],
        [0, 1, 2, 3, 4]]])
```

Python interpreter can be instantiated one time and used at runtime through a pipe resulting in an acceptable responsiveness despite the fact its a process redirection instead of an expected tight integration with the .NET.
A recycle method allow you to restart the process if you consider its internal memory growth too much.
Follow test results on speed:
```
First execution 00:00:00.5451368
Second execution 00:00:00.0486802
```

## Discrete space
- [Example05](/src/Example05/Program.cs) : create a sample space and query for items in it
![img](/doc/Example05_01.PNG)
![img](/doc/Example05_02.PNG)
![img](/doc/Example05_03.PNG)

## Measure Unit
- [Example01](/src/Example01/Program.cs) : create a sci project with two measure object inside it and serialize then read back from deserialized data. Here measure serialized contains value and measure unit. Use this type of solution if want to keep track of the measure unit ( eg. user interface )
- [Example02](/src/Example02/Program.cs) : create a sci project with two measure object as double thus using a measure unit domain. Use this type of solution if want minimize file size ( eg. many data )
