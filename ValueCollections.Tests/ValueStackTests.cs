namespace ValueCollections.Tests;

public class ValueStackTests {
    [Fact]
    public void BasicTest() {
        using ValueStack<double> doubles = new([1, 2, 3]);
        doubles.Push(4);
        doubles.PushRange([5, 6]);
        doubles.ToList().ShouldBe([6, 5, 4, 3, 2, 1]);
    }
    [Fact]
    public void ConstructorsTest() {
        new ValueStack<string>().ToList().ShouldBe([]);
        new ValueStack<string>(4).Capacity.ShouldBeGreaterThanOrEqualTo(4);
        ValueStack<char>.FromBuffer(stackalloc char[3]).Capacity.ShouldBe(3);
        new ValueStack<char>("abc").ToList().ShouldBe(['c', 'b', 'a']);
    }
    [Fact]
    public void PushPopPeekTest() {
        using ValueStack<string> strings = [];
        strings.Push("abacus");
        strings.Push("banana");
        strings.Pop();

        strings.Peek().ShouldBe("abacus");
        strings.ToList().ShouldBe(["abacus"]);

        strings.Push("zyzzyva");

        strings.Peek().ShouldBe("zyzzyva");
        strings.ToList().ShouldBe(["zyzzyva", "abacus"]);

        strings.TryPeek(out string? peekResult).ShouldBeTrue();
        peekResult.ShouldBe("zyzzyva");

        strings.TryPop(out string? popResult).ShouldBeTrue();
        popResult.ShouldBe("zyzzyva");

        strings.Pop().ShouldBe("abacus");

        strings.TryPeek(out string? _).ShouldBeFalse();
        strings.TryPop(out string? _).ShouldBeFalse();
    }
    [Fact]
    public void EnumerateOrderTest() {
        new Stack<int>([1, 2, 3]).ToList().ShouldBe([3, 2, 1]);
        new ValueStack<int>([1, 2, 3]).ToList().ShouldBe([3, 2, 1]);

        new Stack<int>([1, 2, 3]).ToArray().ShouldBe([3, 2, 1]);
        new ValueStack<int>([1, 2, 3]).ToArray().ShouldBe([3, 2, 1]);
    }
    [Fact]
    public void PushTest() {
        using ValueStack<int> stack = ValueStack<int>.FromBuffer(stackalloc int[64]);
        for (int i = 0; i < 100; i++) {
            stack.Push(i);
        }
        stack.Count.ShouldBe(100);
        stack.Capacity.ShouldBeGreaterThanOrEqualTo(stack.Count);
    }
    [Fact]
    public void WhereTest() {
        Stack<int> stack = new([1, 2, 3]);
        stack.ToValueStack().Where(num => num % 2 == 0).ToList().ShouldBe([2]);
    }
}