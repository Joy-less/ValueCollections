# ValueCollections

[![NuGet](https://img.shields.io/nuget/v/ValueCollections.svg)](https://www.nuget.org/packages/ValueCollections)

A set of collections in C# implemented as `ref struct` to minimize heap allocations.

## ValueList

An implementation of `IList<T>` using spans and array pools.

```cs
using ValueList<int> numbers = [];

for (int n = 0; n < 10; n++) {
    numbers.Add(n);
}

using ValueList<int> evenNumbers = numbers.Where(number => number % 2 == 0);

Console.WriteLine(string.Join(", ", evenNumbers.ToList())); // 0, 2, 4, 6, 8
```

## ValueHashSet

An implementation of `ISet<T>` using spans and array pools.

```cs
using ValueHashSet<int> numbers = [];

for (int n = 1; n <= 5; n++) {
    numbers.Add(n);
}
for (int n = 3; n <= 7; n++) {
    numbers.Add(n);
}

Console.WriteLine(string.Join(", ", numbers.ToArray())); // 1, 2, 3, 4, 5, 6, 7
```

## Benchmarks

| Method                 | Mean         | Error     | StdDev    | Gen0    | Allocated |
|----------------------- |-------------:|----------:|----------:|--------:|----------:|
| SmallListOfStruct      |     16.85 ns |  0.122 ns |  0.108 ns |  0.0255 |      80 B |
| SmallValueListOfStruct |     14.72 ns |  0.072 ns |  0.064 ns |       - |         - |
| SmallListOfClass       |     24.52 ns |  0.152 ns |  0.135 ns |  0.0306 |      96 B |
| SmallValueListOfClass  |     18.70 ns |  0.070 ns |  0.062 ns |       - |         - |
| LargeListOfStruct      | 20,468.89 ns | 83.070 ns | 77.704 ns | 41.6565 |  131400 B |
| LargeValueListOfStruct |  9,432.68 ns | 33.047 ns | 27.596 ns |       - |         - |

| Method                    | Mean          | Error        | StdDev       | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------- |--------------:|-------------:|-------------:|--------:|--------:|--------:|----------:|
| SmallHashSetOfStruct      |      94.20 ns |     0.662 ns |     0.620 ns |  0.1070 |       - |       - |     336 B |
| SmallValueHashSetOfStruct |     137.42 ns |     0.616 ns |     0.577 ns |       - |       - |       - |         - |
| SmallHashSetOfClass       |     130.56 ns |     0.762 ns |     0.675 ns |  0.1173 |       - |       - |     368 B |
| SmallValueListOfClass     |     163.45 ns |     0.554 ns |     0.518 ns |       - |       - |       - |         - |
| LargeHashSetOfStruct      | 186,989.50 ns | 1,425.253 ns | 1,263.450 ns | 95.2148 | 95.2148 | 95.2148 |  538656 B |
| LargeValueHashSetOfStruct | 227,245.29 ns |   603.642 ns |   564.647 ns |       - |       - |       - |         - |

## Gotchas

Value collections should be disposed after use, otherwise the internal rented array will not be returned to the pool.
```cs
using ValueList<string> strings = ["a", "b", "c"];
using ValueList<string> whitespaceStrings = strings.Where(string.IsNullOrWhiteSpace);
using ValueList<string> shortWhitespaceStrings = whitespaceStrings.Where(str => str.Length <= 10);
```

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.