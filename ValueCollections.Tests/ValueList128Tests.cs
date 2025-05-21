using ValueCollections.FixedSize;

namespace ValueCollections.Tests;

public class ValueList128Tests {
    [Fact]
    public void BasicTest() {
        ValueList128<double> doubles = [1, 2, 3];
        doubles.Add(4);
        doubles.AddRange([5, 6]);
        doubles.ToList().ShouldBe([1, 2, 3, 4, 5, 6]);
    }
    [Fact]
    public void ConstructorsTest() {
        new ValueList128<string>().ToList().ShouldBe([]);
        ValueList128<string>.Capacity.ShouldBeGreaterThanOrEqualTo(4);
        new ValueList128<char>("abc").ToList().ShouldBe(['a', 'b', 'c']);
    }
    [Fact]
    public void IndexerTest() {
        ValueList128<string> strings = [];
        strings.Add("abacus");
        strings.Add("banana");
        strings[0].ShouldBe("abacus");
        strings[1].ShouldBe("banana");

        strings[1] = "zyzzyva";
        strings[1].ShouldBe("zyzzyva");
    }
    [Fact]
    public void SortTest() {
        ValueList128<double> doubles = [3, 1, 2];
        doubles.Sort();
        doubles.ToList().ShouldBe([1, 2, 3]);
    }
    [Fact]
    public void IndexerOutOfRangeTest() {
        Should.Throw<IndexOutOfRangeException>(() => {
            ValueList128<string> strings = [];
            _ = strings[0];
        });
    }
    [Fact]
    public void IndexOf() {
        ValueList128<string> strings = ["a", "b", "c"];
        strings.IndexOf("a").ShouldBe(0);
        strings.IndexOf("b").ShouldBe(1);
        strings.IndexOf("c").ShouldBe(2);
        strings.IndexOf("d").ShouldBe(-1);
    }
    [Fact]
    public void Add() {
        ValueList128<int> list = [];
        for (int i = 0; i < 100; i++) {
            list.Add(i);
        }
        list.Count.ShouldBe(100);
        ValueList128<int>.Capacity.ShouldBeGreaterThanOrEqualTo(list.Count);
    }
    [Fact]
    public void Where() {
        List<int> list = [1, 2, 3];
        list.ToValueList().Where(num => num % 2 == 0).ToList().ShouldBe([2]);
    }
    [Fact]
    public void Select() {
        List<int> list = [1, 2, 3];
        list.ToValueList().Select(num => num + 1).ToList().ShouldBe([2, 3, 4]);
        list.ToValueList().Select((num, index) => num + index).ToList().ShouldBe([1, 3, 5]);
        list.ToValueList().Select((num, index) => num / 2.0).ToList().ShouldBe([0.5, 1.0, 1.5]);
    }
    [Fact]
    public void Except() {
        List<int> list = [1, 2, 3];
        list.ToValueList().Except(2).ToList().ShouldBe([1, 3]);
        list.ToValueList().Except([2, 1]).ToList().ShouldBe([3]);
    }
    [Fact]
    public void OrderBy() {
        List<int> list = [4, 2, 5, 1, 3];
        list.ToValueList().Order().ToList().ShouldBe([1, 2, 3, 4, 5]);
        list.ToValueList().OrderDescending().ToList().ShouldBe([5, 4, 3, 2, 1]);
        list.ToValueList().OrderBy(element => 10 - element).ToList().ShouldBe([5, 4, 3, 2, 1]);
        list.ToValueList().OrderByDescending(element => 10 - element).ToList().ShouldBe([1, 2, 3, 4, 5]);
    }
}