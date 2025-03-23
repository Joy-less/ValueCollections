# ValueCollections

[![NuGet](https://img.shields.io/nuget/v/ValueCollections.svg)](https://www.nuget.org/packages/ValueCollections)

A set of collections in C# implemented as `ref struct` to minimize heap allocations.

## ValueList

An implementation of `IList<T>` using spans and array pools.

```cs
using ValueList<int> list = [];

for (int i = 0; i < 100; i++) {
    list.Add(i);
}

Console.WriteLine(string.Join(", ", list.ToList()));
```

## Benchmarks

| Method                 | Mean         | Error      | StdDev     | Gen0    | Allocated |
|----------------------- |-------------:|-----------:|-----------:|--------:|----------:|
| SmallListOfStruct      |     17.92 ns |   0.122 ns |   0.114 ns |  0.0255 |      80 B |
| SmallValueListOfStruct |     15.14 ns |   0.050 ns |   0.047 ns |       - |         - |
| SmallListOfClass       |     25.69 ns |   0.077 ns |   0.069 ns |  0.0306 |      96 B |
| SmallValueListOfClass  |     18.36 ns |   0.037 ns |   0.033 ns |       - |         - |
| LargeListOfStruct      | 21,131.86 ns | 119.642 ns | 111.913 ns | 41.6565 |  131400 B |
| LargeValueListOfStruct |  7,454.80 ns |  11.838 ns |  10.494 ns |       - |         - |

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.