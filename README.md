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

## ValueDictionary

An implementation of `IDctionary<TKey, TValue>` using spans and array pools.

```cs
using ValueDictionary<string, string> strings = [];

strings.Add("food", "pizza");
strings.Add("drink", "cola");

Console.WriteLine(string.Join(", ", strings.ToArray())); // [food, pizza], [drink, cola]
```

## Benchmarks

| Method                 | Mean         | Error      | StdDev     | Gen0    | Allocated |
|----------------------- |-------------:|-----------:|-----------:|--------:|----------:|
| SmallListOfStruct      |     16.96 ns |   0.139 ns |   0.130 ns |  0.0255 |      80 B |
| SmallValueListOfStruct |     17.28 ns |   0.064 ns |   0.060 ns |       - |         - |
| SmallListOfClass       |     24.76 ns |   0.374 ns |   0.350 ns |  0.0306 |      96 B |
| SmallValueListOfClass  |     21.49 ns |   0.193 ns |   0.180 ns |       - |         - |
| LargeListOfStruct      | 21,227.63 ns | 170.160 ns | 159.168 ns | 41.6565 |  131400 B |
| LargeValueListOfStruct |  9,559.52 ns | 126.272 ns | 118.115 ns |       - |       2 B |

| Method                    | Mean          | Error        | StdDev       | Gen0     | Gen1     | Gen2    | Allocated |
|-------------------------- |--------------:|-------------:|-------------:|---------:|---------:|--------:|----------:|
| SmallHashSetOfStruct      |      93.25 ns |     0.644 ns |     0.538 ns |   0.1070 |        - |       - |     336 B |
| SmallValueHashSetOfStruct |     140.43 ns |     1.252 ns |     1.171 ns |        - |        - |       - |         - |
| SmallHashSetOfClass       |     134.52 ns |     1.164 ns |     1.089 ns |   0.1173 |        - |       - |     368 B |
| SmallValueHashSetOfClass  |     192.37 ns |     0.353 ns |     0.330 ns |        - |        - |       - |         - |
| LargeHashSetOfStruct      | 229,623.24 ns | 1,678.047 ns | 1,487.545 ns | 460.2051 | 460.2051 | 76.9043 |  538650 B |
| LargeValueHashSetOfStruct | 228,776.38 ns |   709.610 ns |   629.051 ns |        - |        - |       - |      32 B |

| Method                        | Mean         | Error       | StdDev      | Gen0     | Gen1     | Gen2     | Allocated |
|------------------------------ |-------------:|------------:|------------:|---------:|---------:|---------:|----------:|
| SmallDictionaryOfStructs      |     100.6 ns |     0.60 ns |     0.56 ns |   0.1223 |        - |        - |     384 B |
| SmallValueDictionaryOfStructs |     124.9 ns |     0.68 ns |     0.64 ns |        - |        - |        - |         - |
| SmallDictionaryOfClasses      |     154.5 ns |     1.68 ns |     1.31 ns |   0.1478 |        - |        - |     464 B |
| SmallValueDictionaryOfClasses |     216.0 ns |     0.69 ns |     0.61 ns |        - |        - |        - |         - |
| LargeDictionaryOfStructs      | 217,012.6 ns |   496.10 ns |   439.78 ns | 367.6758 | 367.6758 | 105.2246 |  673203 B |
| LargeValueDictionaryOfStructs | 286,426.3 ns | 1,705.62 ns | 1,511.99 ns |        - |        - |        - |     193 B |

## Gotchas

Value collections should be disposed after use, otherwise the internal rented array will not be returned to the pool.
```cs
using ValueList<string> strings = ["a", "b", "c"];
using ValueList<string> whitespaceStrings = strings.Where(string.IsNullOrWhiteSpace);
using ValueList<string> shortWhitespaceStrings = whitespaceStrings.Where(str => str.Length <= 10);
```

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.