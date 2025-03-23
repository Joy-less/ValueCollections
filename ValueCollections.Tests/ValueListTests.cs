namespace ValueCollections.Tests;

public class ValueListTests {
    [Fact]
    public void BasicTest() {
        using ValueList<double> doubles = [1, 2, 3];
        doubles.Add(4);
        doubles.AddRange([5, 6]);
        doubles.ToList().ShouldBe([1, 2, 3, 4, 5, 6]);
    }
    [Fact]
    public void ConstructorsTest() {
        new ValueList<string>().ToList().ShouldBe([]);
        new ValueList<string>(4).Capacity.ShouldBeGreaterThanOrEqualTo(4);
        new ValueList<char>(stackalloc char[3]).Capacity.ShouldBe(3);
        new ValueList<char>("abc").ToList().ShouldBe(['a', 'b', 'c']);
    }
    [Fact]
    public void IndexerTest() {
        using ValueList<string> strings = [];
        strings.Add("abacus");
        strings.Add("banana");
        strings[0].ShouldBe("abacus");
        strings[1].ShouldBe("banana");

        strings[1] = "zyzzyva";
        strings[1].ShouldBe("zyzzyva");
    }
    [Fact]
    public void IndexerOutOfRangeTest() {
        Should.Throw<IndexOutOfRangeException>(() => {
            using ValueList<string> strings = [];
            _ = strings[0];
        });
    }
    [Fact]
    public void IndexOf() {
        using ValueList<string> strings = ["a", "b", "c"];
        strings.IndexOf("a").ShouldBe(0);
        strings.IndexOf("b").ShouldBe(1);
        strings.IndexOf("c").ShouldBe(2);
        strings.IndexOf("d").ShouldBe(-1);
    }
    [Fact]
    public void Add() {
        using ValueList<int> list = [];
        for (int i = 0; i < 100; i++) {
            list.Add(i);
        }
        list.Count.ShouldBe(100);
        list.Capacity.ShouldBeGreaterThanOrEqualTo(list.Count);
    }
}