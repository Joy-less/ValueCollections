using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ValueCollections.Benchmarks;

public class Program {
    public static void Main() {
        BenchmarkSwitcher.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Run();
    }
}

[MemoryDiagnoser]
public class Benchmarks {
    [Benchmark]
    public void SmallListOfStruct() {
        List<int> list = [1, 2, 3, 4, 5];
        _ = list.IndexOf(3);
    }
    [Benchmark]
    public void SmallValueListOfStruct() {
        using ValueList<int> list = [1, 2, 3, 4, 5];
        _ = list.IndexOf(3);
    }

    [Benchmark]
    public void SmallListOfClass() {
        List<string> list = ["1", "2", "3", "4", "5"];
        _ = list.IndexOf("3");
    }
    [Benchmark]
    public void SmallValueListOfClass() {
        using ValueList<string> list = ["1", "2", "3", "4", "5"];
        _ = list.IndexOf("3");
    }

    [Benchmark]
    public void LargeListOfStruct() {
        List<int> list = [];
        for (int i = 0; i < 10_000; i++) {
            list.Add(i);
        }
        _ = list.Count;
    }
    [Benchmark]
    public void LargeValueListOfStruct() {
        using ValueList<int> list = [];
        for (int i = 0; i < 10_000; i++) {
            list.Add(i);
        }
        _ = list.Count;
    }
}