# ValueCollections

[![NuGet](https://img.shields.io/nuget/v/ValueCollections.svg)](https://www.nuget.org/packages/ValueCollections)
[![NuGet](https://img.shields.io/nuget/v/ValueCollections.FixedSize.svg)](https://www.nuget.org/packages/ValueCollections.FixedSize)

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

## ValueStack

An implementation of `Stack<T>` using spans and array pools.

```cs
using ValueStack<int> numbers = new();

numbers.Push(1);
numbers.PushRange([3, 3]);
numbers.Push(7);
numbers.Pop();

Console.WriteLine(string.Join(", ", numbers.ToArray())); // 1, 3, 3
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

## Benchmarks

| Method                    | Mean      | Error     | StdDev    | Gen0   | Allocated |
|-------------------------- |----------:|----------:|----------:|-------:|----------:|
| SmallListOfStruct         | 27.801 ns | 0.0879 ns | 0.0734 ns | 0.0229 |     144 B |
| SmallValueListOfStruct    | 16.547 ns | 0.0379 ns | 0.0354 ns |      - |         - |
| SmallValueList8OfStruct   |  9.331 ns | 0.0180 ns | 0.0160 ns |      - |         - |
| SmallValueList16OfStruct  |  9.552 ns | 0.0178 ns | 0.0158 ns |      - |         - |
| SmallValueList32OfStruct  | 17.032 ns | 0.0394 ns | 0.0369 ns |      - |         - |
| SmallValueList128OfStruct | 27.538 ns | 0.1100 ns | 0.0975 ns |      - |         - |

| Method                    | Mean          | Error        | StdDev       | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------- |--------------:|-------------:|-------------:|--------:|--------:|--------:|----------:|
| SmallHashSetOfStruct      |      81.87 ns |     0.664 ns |     0.621 ns |  0.0535 |       - |       - |     336 B |
| SmallValueHashSetOfStruct |     150.36 ns |     0.617 ns |     0.515 ns |       - |       - |       - |         - |
| SmallHashSetOfClass       |     101.67 ns |     1.273 ns |     1.190 ns |  0.0587 |       - |       - |     368 B |
| SmallValueHashSetOfClass  |     150.27 ns |     0.285 ns |     0.267 ns |       - |       - |       - |         - |
| LargeHashSetOfStruct      | 141,632.55 ns | 1,615.994 ns | 1,432.536 ns | 95.2148 | 95.2148 | 95.2148 |  538592 B |
| LargeValueHashSetOfStruct | 320,518.52 ns |   786.970 ns |   736.132 ns |       - |       - |       - |         - |

| Method                        | Mean          | Error      | StdDev     | Gen0     | Gen1     | Gen2     | Allocated |
|------------------------------ |--------------:|-----------:|-----------:|---------:|---------:|---------:|----------:|
| SmallDictionaryOfStructs      |      77.02 ns |   0.799 ns |   0.708 ns |   0.0612 |        - |        - |     384 B |
| SmallValueDictionaryOfStructs |     101.35 ns |   0.160 ns |   0.141 ns |        - |        - |        - |         - |
| SmallDictionaryOfClasses      |      99.76 ns |   0.205 ns |   0.181 ns |   0.0739 |        - |        - |     464 B |
| SmallValueDictionaryOfClasses |     178.76 ns |   0.242 ns |   0.202 ns |        - |        - |        - |         - |
| LargeDictionaryOfStructs      | 163,181.73 ns | 883.658 ns | 737.894 ns | 124.7559 | 124.7559 | 124.7559 |  673026 B |
| LargeValueDictionaryOfStructs | 264,539.87 ns | 968.330 ns | 905.777 ns |        - |        - |        - |         - |

| Method                  | Mean         | Error     | StdDev    | Gen0    | Gen1   | Allocated |
|------------------------ |-------------:|----------:|----------:|--------:|-------:|----------:|
| SmallStackOfStruct      |     16.55 ns |  0.044 ns |  0.037 ns |  0.0127 |      - |      80 B |
| SmallValueStackOfStruct |     12.25 ns |  0.044 ns |  0.041 ns |       - |      - |         - |
| SmallStackOfClass       |     19.53 ns |  0.126 ns |  0.117 ns |  0.0153 |      - |      96 B |
| SmallValueStackOfClass  |     16.09 ns |  0.026 ns |  0.025 ns |       - |      - |         - |
| LargeStackOfStruct      | 17,901.06 ns | 76.238 ns | 67.583 ns | 20.8130 | 4.1504 |  131400 B |
| LargeValueStackOfStruct |  8,576.68 ns | 11.574 ns |  9.665 ns |       - |      - |         - |

## Gotchas

Value collections (except fixed-size collections) should be disposed after use, otherwise the internal rented array will not be returned to the pool.
```cs
using ValueList<string> strings = ["a", "b", "c"];
using ValueList<string> whitespaceStrings = strings.Where(string.IsNullOrWhiteSpace);
using ValueList<string> shortWhitespaceStrings = whitespaceStrings.Where(str => str.Length <= 10);
```

## Special Thanks

- [linkdotnet/StringBuilder](https://github.com/linkdotnet/StringBuilder) for inspiration.