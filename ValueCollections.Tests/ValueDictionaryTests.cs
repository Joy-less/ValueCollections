namespace ValueCollections.Tests;

public class ValueDictionaryTests {
    [Fact]
    public void BasicTest() {
        using ValueDictionary<float, double> doubles = new() {
            [1] = -1,
            [2] = -2,
            [3] = -3,
        };
        doubles.Add(4, -4);
        doubles.AddRange([new KeyValuePair<float, double>(5, -5), new KeyValuePair<float, double>(6, -6)]);
        doubles.ToList().ShouldBe([new(1, -1), new(2, -2), new(3, -3), new(4, -4), new(5, -5), new(6, -6)], ignoreOrder: true);
    }
    [Fact]
    public void ConstructorsTest() {
        new ValueDictionary<string, int>().ToList().ShouldBe([], ignoreOrder: true);
        new ValueDictionary<string, int>(4).Capacity.ShouldBeGreaterThanOrEqualTo(4);
        ValueDictionary<char, long>.FromBuffer(stackalloc KeyValuePair<char, long>[3], stackalloc int[3]).Capacity.ShouldBe(3);
        new ValueDictionary<char, long>([new('a', 5L), new('X', 3L)]).ToList().ShouldBe([new('a', 5L), new('X', 3L)], ignoreOrder: true);
    }
    [Fact]
    public void UniqueTest() {
        using ValueDictionary<string, float> strings = [];
        strings.TryAdd("abacus", 3f);
        strings.TryAdd("banana", 123.5f);
        strings.TryAdd("abacus", -7f);
        strings.Count.ShouldBe(2);
    }
    [Fact]
    public void Where() {
        List<KeyValuePair<int, char>> list = [new(1, 'a'), new(2, 'b'), new(3, 'c')];
        list.ToValueDictionary()
            .ToDictionary() // TODO: remove
        .Where(entry => entry.Key % 2 == 0).ToList().ShouldBe([new(2, 'b')]);
    }
}