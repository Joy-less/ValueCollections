using System.Collections;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ValueCollections;

/// <summary>
/// A version of <see cref="List{T}"/> which minimizes as many heap allocations as possible.
/// </summary>
/// <remarks>
/// You should dispose it after use to ensure the rented buffer is returned to the array pool.
/// </remarks>
public ref partial struct ValueList<T> : IDisposable, IList<T>, IReadOnlyList<T> {
    private Span<T> Buffer;
    private int BufferPosition;
    private T[]? ArrayFromPool;

    /// <summary>
    /// Constructs a value list with a default capacity of 32.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList() {
        EnsureCapacity(32);
    }
    /// <summary>
    /// Constructs a value list with the given capacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList(int capacity) {
        EnsureCapacity(capacity);
    }
    /// <summary>
    /// Constructs a value list with the given buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList(Span<T> initialBuffer) {
        Buffer = initialBuffer;
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList(ReadOnlySpan<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList(IEnumerable<T> initialElements) {
        AddRange(initialElements);
    }

    /// <summary>
    /// Disposes the instance and returns the rented buffer to the array pool if needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        if (ArrayFromPool is not null) {
            ArrayPool<T>.Shared.Return(ArrayFromPool);
        }

        this = default;
    }

    /// <summary>
    /// Returns the current number of elements in the list.
    /// </summary>
    public readonly int Count {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BufferPosition;
    }

    /// <summary>
    /// Returns the current maximum capacity before the span must be resized.
    /// </summary>
    public readonly int Capacity {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Buffer.Length;
    }

    /// <inheritdoc/>
    readonly bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Returns the element at the given index.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
    public readonly ref T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (index < 0 || index >= BufferPosition) {
                throw new IndexOutOfRangeException();
            }

            return ref Buffer[index];
        }
    }

    /// <inheritdoc/>
    T IList<T>.this[int index] {
        readonly get => this[index];
        set => this[index] = value;
    }

    /// <inheritdoc/>
    readonly T IReadOnlyList<T>.this[int index] {
        get => this[index];
    }

    /// <summary>
    /// Gets a span over the elements in the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() => Buffer[..BufferPosition];

    /// <summary>
    /// Adds an element to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value) {
        EnsureCapacity(BufferPosition + 1);
        Buffer[BufferPosition] = value;
        BufferPosition++;
    }

    /// <summary>
    /// Adds multiple elements to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(IEnumerable<T> values) {
        if (values.TryGetNonEnumeratedCount(out int count)) {
            EnsureCapacity(BufferPosition + count);
            foreach (T value in values) {
                Buffer[BufferPosition] = value;
                BufferPosition++;
            }
        }
        else {
            foreach (T value in values) {
                Add(value);
            }
        }
    }

    /// <summary>
    /// Adds multiple elements to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(scoped ReadOnlySpan<T> values) {
        EnsureCapacity(BufferPosition + values.Length);
        foreach (T value in values) {
            Buffer[BufferPosition] = value;
            BufferPosition++;
        }
    }

    /// <summary>
    /// Ensure's the list's capacity is at least <paramref name="newCapacity"/>, renting a larger buffer if not.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity) {
        if (Capacity >= newCapacity) {
            return;
        }

        T[] rented = ArrayPool<T>.Shared.Rent(FindSmallestPowerOf2Above(newCapacity));

        if (BufferPosition > 0) {
            Buffer.CopyTo(rented);
        }

        if (ArrayFromPool is not null) {
            ArrayPool<T>.Shared.Return(ArrayFromPool);
        }

        Buffer = rented;
        ArrayFromPool = rented;
    }

    /// <summary>
    /// Returns the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    private static int FindSmallestPowerOf2Above(int minimum) {
        return 1 << (int)Math.Ceiling(Math.Log2(minimum));
    }

    /// <summary>
    /// Returns the index of <paramref name="value"/> or -1 if not found.
    /// </summary>
    public readonly int IndexOf(T value) {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int index = 0; index < BufferPosition; index++) {
            if (comparer.Equals(Buffer[index], value)) {
                return index;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of <paramref name="value"/> or -1 if not found.
    /// </summary>
    public readonly int LastIndexOf(T value) {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int index = BufferPosition - 1; index >= 0; index--) {
            if (comparer.Equals(Buffer[index], value)) {
                return index;
            }
        }
        return -1;
    }

    /// <summary>
    /// Inserts <paramref name="value"/> at <paramref name="index"/>.
    /// </summary>
    public void Insert(int index, T value) {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, BufferPosition);

        EnsureCapacity(BufferPosition + 1);
        Buffer[index..BufferPosition].CopyTo(Buffer[(index + 1)..]);
        Buffer[index] = value;
        BufferPosition++;
    }

    /// <summary>
    /// Removes an element at <paramref name="index"/>.
    /// </summary>
    public void RemoveAt(int index) {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, BufferPosition);

        Buffer[(index + 1)..].CopyTo(Buffer[index..]);
        Buffer[BufferPosition] = default!;
        BufferPosition--;
    }

    /// <summary>
    /// Finds and removes the first instance of <paramref name="value"/> from the list.
    /// </summary>
    public bool Remove(T value) {
        int index = IndexOf(value);
        if (index < 0) {
            return false;
        }
        RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Removes every element matching <paramref name="predicate"/> from the list.
    /// </summary>
    public int RemoveWhere(Func<T, bool> predicate) {
        int counter = 0;
        int index = 0;
        while (index < BufferPosition) {
            if (predicate(Buffer[index])) {
                RemoveAt(index);
                counter++;
            }
            else {
                index++;
            }
        }
        return counter;
    }

    /// <summary>
    /// Removes every element.
    /// </summary>
    public void Clear() {
        Buffer[..BufferPosition].Clear();
        BufferPosition = 0;
    }

    /// <summary>
    /// Returns whether <paramref name="value"/> is found in the list.
    /// </summary>
    public readonly bool Contains(T value) {
        return IndexOf(value) >= 0;
    }

    /// <summary>
    /// Copies every element to <paramref name="destination"/>.
    /// </summary>
    public readonly void CopyTo(scoped Span<T> destination) {
        Buffer[..BufferPosition].CopyTo(destination);
    }

    /// <summary>
    /// Copies every element to <paramref name="destination"/> at <paramref name="destinationIndex"/>.
    /// </summary>
    public readonly void CopyTo(T[] destination, int destinationIndex) {
        CopyTo(destination.AsSpan(destinationIndex));
    }

    /// <summary>
    /// Returns an enumerator that iterates over the elements of the list.
    /// </summary>
    public readonly Enumerator GetEnumerator() {
        return new Enumerator(this);
    }

    /// <inheritdoc/>
    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() {
        return ((IEnumerable<T>)this.ToArray()).GetEnumerator();
    }

    /// <inheritdoc/>
    readonly IEnumerator IEnumerable.GetEnumerator() {
        return this.ToArray().GetEnumerator();
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="ValueList{T}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public ref struct Enumerator : IEnumerator<T> {
        private readonly ValueList<T> List;
        private int Index;

        /// <summary>
        /// Constructs a new enumerator over the elements of <paramref name="list"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ValueList<T> list) {
            List = list;
            Index = -1;
        }

        /// <summary>
        /// Returns the element at the current position of the list.
        /// </summary>
        public readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => List[Index];
        }

        /// <inheritdoc/>
        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        readonly void IDisposable.Dispose() {
        }

        /// <summary>
        /// Advances the enumerator to the next element of the list.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator successfully advanced to the next element; <see langword="false"/> if the enumerator reached the end of the list.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            return ++Index < List.Count;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the list.
        /// </summary>
        public void Reset() {
            Index = -1;
        }
    }
}