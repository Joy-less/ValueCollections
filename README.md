# ValueCollections

[![NuGet](https://img.shields.io/nuget/v/ValueCollections.svg)](https://www.nuget.org/packages/ValueCollections)

A set of collections in C# implemented as `ref struct` to minimize heap allocations.

## ValueList

An implementation of `IList<T>` using spans and array pools.

```cs
using ValueList<int> numbers = [];

for (int n = 0; n < 100; n++) {
    numbers.Add(n);
}

using ValueList<int> evenNumbers = numbers.Where(number => number % 2 == 0);

Console.WriteLine(string.Join(", ", evenNumbers.ToList()));
```

## Benchmarks

| Method                 | Mean         | Error     | StdDev    | Gen0    | Allocated |
|----------------------- |-------------:|----------:|----------:|--------:|----------:|
| SmallListOfStruct      |     17.71 ns |  0.103 ns |  0.091 ns |  0.0255 |      80 B |
| SmallValueListOfStruct |     14.50 ns |  0.038 ns |  0.034 ns |       - |         - |
| SmallListOfClass       |     25.31 ns |  0.153 ns |  0.135 ns |  0.0306 |      96 B |
| SmallValueListOfClass  |     17.22 ns |  0.047 ns |  0.044 ns |       - |         - |
| LargeListOfStruct      | 20,962.48 ns | 99.370 ns | 92.951 ns | 41.6565 |  131400 B |
| LargeValueListOfStruct |  9,663.62 ns | 37.740 ns | 33.455 ns |       - |         - |

## Gotchas

`ValueList` should be disposed after use, otherwise the internal rented array will not be returned to the pool.
```cs
using ValueList<string> strings = ["a", "b", "c"];
using ValueList<string> whitespaceStrings = strings.Where(string.IsNullOrWhiteSpace);
using ValueList<string> shortWhitespaceStrings = whitespaceStrings.Where(str => str.Length <= 10);
```

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.