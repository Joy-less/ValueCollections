# ValueCollections

[![NuGet](https://img.shields.io/nuget/v/ValueCollections.svg)](https://www.nuget.org/packages/ValueCollections)

A set of collections in C# implemented as `struct` to minimize heap allocations.

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

## ValueList (Fixed Size)

A set of implementations of `IList<T>` using fixed-size inlined arrays.

Supported for capacities of `[1, 2, 3, 4, 8, 16, 32, 64, 128, 256, 512]`.

```cs
ValueList128<int> numbers = [];

for (int n = 0; n < 10; n++) {
    numbers.Add(n);
}

Console.WriteLine(string.Join(", ", numbers.Where(number => number % 2 == 0))); // 0, 2, 4, 6, 8
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

## ValueDictionary

An implementation of `IDictionary<TKey, TValue>` using spans and array pools.

```cs
using ValueDictionary<string, string> strings = [];

strings.Add("food", "pizza");
strings.Add("drink", "cola");

Console.WriteLine(string.Join(", ", strings.ToArray())); // [food, pizza], [drink, cola]
```

## Benchmarks

| Method                   | Mean         | Error      | StdDev     | Gen0    | Allocated |
|------------------------- |-------------:|-----------:|-----------:|--------:|----------:|
| SmallListOfStruct        |     17.57 ns |   0.204 ns |   0.181 ns |  0.0255 |      80 B |
| SmallValueListOfStruct   |     15.68 ns |   0.099 ns |   0.092 ns |       - |         - |
| SmallValueList32OfStruct |     13.75 ns |   0.054 ns |   0.047 ns |       - |         - |
| SmallListOfClass         |     24.54 ns |   0.255 ns |   0.226 ns |  0.0306 |      96 B |
| SmallValueListOfClass    |     20.17 ns |   0.077 ns |   0.072 ns |       - |         - |
| SmallValueList32OfClass  |     23.62 ns |   0.133 ns |   0.118 ns |       - |         - |
| LargeListOfStruct        | 20,617.90 ns | 169.942 ns | 150.649 ns | 41.6565 |  131400 B |
| LargeValueListOfStruct   |  9,473.77 ns |  43.585 ns |  40.770 ns |       - |         - |

| Method                    | Mean          | Error        | StdDev       | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------- |--------------:|-------------:|-------------:|--------:|--------:|--------:|----------:|
| SmallHashSetOfStruct      |      92.44 ns |     1.886 ns |     2.097 ns |  0.1070 |       - |       - |     336 B |
| SmallValueHashSetOfStruct |     159.45 ns |     0.612 ns |     0.542 ns |       - |       - |       - |         - |
| SmallHashSetOfClass       |     131.35 ns |     1.075 ns |     1.005 ns |  0.1173 |       - |       - |     368 B |
| SmallValueHashSetOfClass  |     170.13 ns |     0.646 ns |     0.540 ns |       - |       - |       - |         - |
| LargeHashSetOfStruct      | 178,224.06 ns |   958.507 ns |   800.397 ns | 95.2148 | 95.2148 | 95.2148 |  538656 B |
| LargeValueHashSetOfStruct | 227,838.63 ns | 1,594.403 ns | 1,413.397 ns |       - |       - |       - |         - |

| Method                        | Mean         | Error       | StdDev      | Gen0     | Gen1     | Gen2     | Allocated |
|------------------------------ |-------------:|------------:|------------:|---------:|---------:|---------:|----------:|
| SmallDictionaryOfStructs      |     101.2 ns |     1.04 ns |     0.98 ns |   0.1223 |        - |        - |     384 B |
| SmallValueDictionaryOfStructs |     114.8 ns |     0.26 ns |     0.24 ns |        - |        - |        - |         - |
| SmallDictionaryOfClasses      |     152.4 ns |     1.27 ns |     1.18 ns |   0.1478 |        - |        - |     464 B |
| SmallValueDictionaryOfClasses |     203.0 ns |     0.89 ns |     0.79 ns |        - |        - |        - |         - |
| LargeDictionaryOfStructs      | 229,800.1 ns | 3,274.30 ns | 2,902.59 ns | 124.7559 | 124.7559 | 124.7559 |  673106 B |
| LargeValueDictionaryOfStructs | 288,403.7 ns | 1,601.55 ns | 1,498.09 ns |        - |        - |        - |         - |

## Gotchas

Value collections should be disposed after use, otherwise the internal rented array will not be returned to the pool.
```cs
using ValueList<string> strings = ["a", "b", "c"];
using ValueList<string> whitespaceStrings = strings.Where(string.IsNullOrWhiteSpace);
using ValueList<string> shortWhitespaceStrings = whitespaceStrings.Where(str => str.Length <= 10);
```

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.