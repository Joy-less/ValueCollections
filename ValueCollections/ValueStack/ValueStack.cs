using System.Collections;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace ValueCollections;

/// <summary>
/// A version of <see cref="Stack{T}"/> which minimizes as many heap allocations as possible.
/// </summary>
/// <remarks>
/// You should dispose it after use to ensure the rented buffer is returned to the array pool.
/// </remarks>
public ref partial struct ValueStack<T> : IDisposable, IEnumerable<T>, IReadOnlyCollection<T> {
    private Span<T> Buffer;
    private int BufferPosition;
    private T[]? RentedBuffer;

    /// <summary>
    /// Constructs a value stack with a default capacity of 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack() {
    }
    /// <summary>
    /// Constructs a value stack with the given capacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(int capacity) {
        EnsureCapacity(capacity);
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(scoped ReadOnlySpan<T> initialElements) {
        PushRange(initialElements);
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(ReadOnlyMemory<T> initialElements) {
        PushRange(initialElements);
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(scoped ValueList<T> initialElements) {
        PushRange(initialElements.AsSpan());
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(scoped ValueHashSet<T> initialElements) {
        PushRange(initialElements.AsSpan());
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-4)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(scoped ValueStack<T> initialElements) {
        EnsureCapacity(BufferPosition + initialElements.Count);
        foreach (T value in initialElements) {
            Buffer[BufferPosition] = value;
            BufferPosition++;
        }
    }
    /// <summary>
    /// Constructs a value stack with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-5)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStack(IEnumerable<T> initialElements) {
        PushRange(initialElements);
    }

    /// <summary>
    /// Constructs a value stack from the given buffer.
    /// </summary>
    /// <remarks>
    /// The elements in the buffer are ignored. This is useful if you want to use the <see langword="stackalloc"/> keyword.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueStack<T> FromBuffer(Span<T> buffer) {
        return new ValueStack<T>() {
            Buffer = buffer,
        };
    }

    /// <summary>
    /// Disposes the instance and returns the rented buffer to the array pool if needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        if (RentedBuffer is not null) {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
                Buffer[..BufferPosition].Clear();
            }
            ArrayPool<T>.Shared.Return(RentedBuffer);
        }
        this = default;
    }

    /// <summary>
    /// Returns the current number of elements in the stack.
    /// </summary>
    public readonly int Count {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BufferPosition;
    }

    /// <summary>
    /// Returns the current maximum capacity before the span must be resized.
    /// </summary>
    public int Capacity {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Buffer.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => ResizeBuffer(value);
    }

    /// <summary>
    /// Resizes the buffer to the given capacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResizeBuffer(int capacity, bool allowExtra = true) {
        if (capacity == Capacity) {
            return;
        }

        T[] rentedBuffer = allowExtra
            ? ArrayPool<T>.Shared.Rent(capacity)
            : new T[capacity];

        if (BufferPosition > 0) {
            Buffer[..BufferPosition].CopyTo(rentedBuffer);
        }

        if (RentedBuffer is not null) {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
                Buffer[..BufferPosition].Clear();
            }
            ArrayPool<T>.Shared.Return(RentedBuffer);
        }

        Buffer = rentedBuffer;
        if (allowExtra) {
            RentedBuffer = rentedBuffer;
        }
    }

    /// <summary>
    /// Returns the element at the given index.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
    internal readonly ref T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (index < 0 || index >= BufferPosition) {
                throw new IndexOutOfRangeException("The index is outside the bounds of the value stack.");
            }

            return ref Buffer[index];
        }
    }

    /// <summary>
    /// Gets a span over the elements in the stack. The elements in the span are in the reverse order to enumerating the stack.
    /// </summary>
    /// <remarks>
    /// Do not change the capacity of the stack while the span is in use, because the span will continue pointing to the old buffer.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpanReversed() => Buffer[..BufferPosition];

    /// <summary>
    /// Adds an element to the top of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T value) {
        EnsureCapacity(BufferPosition + 1);
        Buffer[BufferPosition] = value;
        BufferPosition++;
    }

    /// <summary>
    /// Adds multiple elements to the top of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushRange(scoped ReadOnlySpan<T> values) {
        EnsureCapacity(BufferPosition + values.Length);
        values.CopyTo(Buffer[BufferPosition..]);
        BufferPosition += values.Length;
    }

    /// <summary>
    /// Adds multiple elements to the top of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public void PushRange(ReadOnlyMemory<T> values) {
        PushRange(values.Span);
    }

    /// <summary>
    /// Adds multiple elements to the top of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public void PushRange(IEnumerable<T> values) {
        if (values.TryGetNonEnumeratedCount(out int count)) {
            EnsureCapacity(BufferPosition + count);
            foreach (T value in values) {
                Buffer[BufferPosition] = value;
                BufferPosition++;
            }
        }
        else {
            foreach (T value in values) {
                Push(value);
            }
        }
    }

    /// <summary>
    /// Removes an element from the top of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop() {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Count);

        BufferPosition--;
        T result = Buffer[BufferPosition];
        Buffer[BufferPosition] = default!;
        return result;
    }

    /// <summary>
    /// Removes an element from the top of the stack if possible.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPop([MaybeNullWhen(false)] out T result) {
        if (Count <= 0) {
            result = default;
            return false;
        }

        BufferPosition--;
        result = Buffer[BufferPosition];
        Buffer[BufferPosition] = default!;
        return true;
    }

    /// <summary>
    /// Returns an element from the top of the stack without removing it.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Peek() {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Count);

        T result = Buffer[BufferPosition - 1];
        return result;
    }

    /// <summary>
    /// Returns an element from the top of the stack if possible without removing it.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryPeek([MaybeNullWhen(false)] out T result) {
        if (Count <= 0) {
            result = default;
            return false;
        }

        result = Buffer[BufferPosition - 1];
        return true;
    }

    /// <summary>
    /// Ensure's the stack's capacity is at least <paramref name="newCapacity"/>, renting a larger buffer if not.<br/>
    /// This is useful when adding a predetermined number of items to the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity) {
        if (Capacity >= newCapacity) {
            return;
        }
        ResizeBuffer(FindSmallestPowerOf2Above(newCapacity));
    }

    /// <summary>
    /// Returns the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindSmallestPowerOf2Above(int minimum) {
        return 1 << (int)Math.Ceiling(Math.Log2(minimum));
    }

    /// <summary>
    /// Ensures the stack's capacity is equal to its count, renting a smaller buffer if not.<br/>
    /// This is useful for reducing memory overhead when it is known that no more elements will be added to the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess() {
        if (Count >= Capacity) {
            return;
        }
        ResizeBuffer(Count, allowExtra: false);
    }

    /// <summary>
    /// Returns the index of <paramref name="value"/> or -1 if not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly int IndexOf(T value) {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int index = 0; index < BufferPosition; index++) {
            if (comparer.Equals(Buffer[index], value)) {
                return index;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns whether <paramref name="value"/> is found in the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(T value) {
        return IndexOf(value) >= 0;
    }

    /// <summary>
    /// Removes every element.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
            Buffer[..BufferPosition].Clear();
        }
        BufferPosition = 0;
    }

    /// <summary>
    /// Copies every element to <paramref name="destination"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(scoped Span<T> destination) {
        Buffer[..BufferPosition].CopyTo(destination);
    }

    /// <summary>
    /// Copies every element to <paramref name="destination"/> at <paramref name="destinationIndex"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(T[] destination, int destinationIndex) {
        CopyTo(destination.AsSpan(destinationIndex));
    }

    /// <summary>
    /// Returns an enumerator that iterates over the elements of the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Enumerator GetEnumerator() {
        return new Enumerator(this);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() {
        return ((IEnumerable<T>)this.ToArray()).GetEnumerator();
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly IEnumerator IEnumerable.GetEnumerator() {
        return this.ToArray().GetEnumerator();
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="ValueStack{T}"/>.
    /// </summary>
    public ref struct Enumerator : IEnumerator<T> {
        private readonly ValueStack<T> Stack;
        private int Index;

        /// <summary>
        /// Constructs a new enumerator over the elements of <paramref name="stack"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ValueStack<T> stack) {
            Stack = stack;
            Index = stack.Count;
        }

        /// <summary>
        /// Returns the element at the current position of the stack.
        /// </summary>
        public readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Stack[Index];
        }

        /// <inheritdoc/>
        readonly object? IEnumerator.Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Current;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() {
        }

        /// <summary>
        /// Advances the enumerator to the next element of the stack.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator successfully advanced to the next element; <see langword="false"/> if the enumerator reached the end of the stack.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            Index--;
            return Index >= 0;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Index = Stack.Count;
        }
    }
}
