using System.Collections;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// A version of <see cref="HashSet{T}"/> which minimizes as many heap allocations as possible.
/// </summary>
/// <remarks>
/// You should dispose it after use to ensure the rented buffer is returned to the array pool.
/// </remarks>
public ref partial struct ValueHashSet<T> : IDisposable, ISet<T>, IReadOnlySet<T> {
    /// <summary>
    /// A comparer used to differentiate the keys of the hash set.
    /// </summary>
    public IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

    internal Span<T> Buffer { get; set; }
    internal int BufferPosition { get; set; }
    internal T[]? RentedBuffer { get; set; }
    internal Span<int> HashCodes { get; set; }
    internal int[]? RentedHashCodes { get; set; }

    /// <summary>
    /// Constructs a value list with a default capacity of 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet() {

    }
    /// <summary>
    /// Constructs a value list with the given capacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(int capacity) {
        EnsureCapacity(capacity);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped Span<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped ReadOnlySpan<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(ReadOnlyMemory<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(IEnumerable<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped ValueList<T> initialElements) {
        AddRange(initialElements.AsSpan());
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped ValueHashSet<T> initialElements) {
        AddRange(initialElements.AsSpan());
    }

    /// <summary>
    /// Constructs a value hash set from the given buffer.
    /// </summary>
    /// <remarks>
    /// The elements in the buffer are ignored. This is useful if you want to use the <see langword="stackalloc"/> keyword.
    /// </remarks>
    public static ValueHashSet<T> FromBuffer(Span<T> Buffer) {
        return new ValueHashSet<T>() {
            Buffer = Buffer,
        };
    }

    /// <summary>
    /// Disposes the instance and returns the rented buffer to the array pool if needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        if (RentedBuffer is not null) {
            ArrayPool<T>.Shared.Return(RentedBuffer);
        }
        if (RentedHashCodes is not null) {
            ArrayPool<int>.Shared.Return(RentedHashCodes);
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
    public int Capacity {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Buffer.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (value == Capacity) {
                return;
            }

            T[] rentedBuffer = ArrayPool<T>.Shared.Rent(value);
            int[] rentedHashCodes = ArrayPool<int>.Shared.Rent(value);

            if (BufferPosition > 0) {
                Buffer.CopyTo(rentedBuffer);
                HashCodes.CopyTo(rentedHashCodes);
            }

            if (RentedBuffer is not null) {
                ArrayPool<T>.Shared.Return(RentedBuffer);
            }
            if (RentedHashCodes is not null) {
                ArrayPool<int>.Shared.Return(RentedHashCodes);
            }

            Buffer = rentedBuffer;
            RentedBuffer = rentedBuffer;
            HashCodes = rentedHashCodes;
            RentedHashCodes = rentedHashCodes;
        }
    }

    /// <inheritdoc/>
    readonly bool ICollection<T>.IsReadOnly {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    /// <summary>
    /// Gets a span over the elements in the list.
    /// </summary>
    /// <remarks>
    /// Do not change the capacity of the list while the span is in use, because the span will continue pointing to the old buffer.<br/>
    /// The span is read-only to ensures the elements are synchronized with the hash codes.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> AsSpan() => Buffer[..BufferPosition];

    /// <summary>
    /// Adds an element to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(T value) {
        if (TryFindIndex(value, out int index)) {
            return false;
        }
        else {
            Insert(index, value);
            return true;
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
    /// Adds multiple elements to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public void AddRange(ReadOnlyMemory<T> values) {
        AddRange(values.Span);
    }

    /// <summary>
    /// Adds multiple elements to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
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
    /// Ensure's the list's capacity is at least <paramref name="newCapacity"/>, renting a larger buffer if not.<br/>
    /// This is useful when adding a predetermined number of items to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity) {
        if (Capacity >= newCapacity) {
            return;
        }
        Capacity = FindSmallestPowerOf2Above(newCapacity);
    }

    /// <summary>
    /// Returns the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindSmallestPowerOf2Above(int minimum) {
        return 1 << (int)Math.Ceiling(Math.Log2(minimum));
    }

    /// <summary>
    /// Ensures the list's capacity is equal its count, renting a smaller buffer if not.<br/>
    /// This is useful for reducing memory overhead when it is known that no more elements will be added to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess() {
        if (Count >= Capacity) {
            return;
        }
        Capacity = Count;
    }

    /// <summary>
    /// Returns whether <paramref name="value"/> is found in the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(T value) {
        return TryFindIndex(value, out _);
    }

    /// <summary>
    /// Finds and removes the first instance of <paramref name="value"/> from the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T value) {
        if (TryFindIndex(value, out int index)) {
            RemoveAt(index);
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Removes every element matching <paramref name="predicate"/> from the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() {
        Buffer[..BufferPosition].Clear();
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
    /// Returns an enumerator that iterates over the elements of the list.
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int GetHashCode(T value) {
        return value is null ? 0 : Comparer.GetHashCode(value);
    }

    /// <summary>
    /// Finds the starting index of <paramref name="hashCode"/> using a binary search.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int FindStartingIndexFromHashCode(int hashCode) {
        int leftPointer = 0;
        int rightPointer = BufferPosition - 1;

        while (true) {
            int midPointer = (leftPointer + rightPointer) / 2;

            if (leftPointer < rightPointer) {
                return midPointer;
            }

            if (HashCodes[midPointer] < hashCode) {
                leftPointer = midPointer + 1;
            }
            else if (HashCodes[midPointer] > hashCode) {
                rightPointer = midPointer - 1;
            }
            else {
                return midPointer;
            }
        }
    }

    /// <summary>
    /// Returns the index of <paramref name="value"/> or -1 if not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool TryFindIndex(T value, out int index) {
        int hashCode = GetHashCode(value);

        int startIndex = FindStartingIndexFromHashCode(hashCode);

        for (index = startIndex; index < BufferPosition; index++) {
            int existingHashCode = HashCodes[index];
            if (existingHashCode != hashCode) {
                break;
            }
            T existingValue = Buffer[index];
            if (Comparer.Equals(existingValue, value)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Inserts <paramref name="value"/> at <paramref name="index"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Insert(int index, T value) {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, BufferPosition);

        int hashCode = GetHashCode(value);

        EnsureCapacity(BufferPosition + 1);
        Buffer[index..BufferPosition].CopyTo(Buffer[(index + 1)..]);
        HashCodes[index..BufferPosition].CopyTo(HashCodes[(index + 1)..]);
        Buffer[index] = value;
        HashCodes[index] = hashCode;
        BufferPosition++;
    }

    /// <summary>
    /// Removes an entry at <paramref name="index"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RemoveAt(int index) {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, BufferPosition);

        Buffer[(index + 1)..].CopyTo(Buffer[index..]);
        HashCodes[(index + 1)..].CopyTo(HashCodes[index..]);
        Buffer[BufferPosition] = default!;
        HashCodes[BufferPosition] = default!;
        BufferPosition--;
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="ValueList{T}"/>.
    /// </summary>
    public ref struct Enumerator : IEnumerator<T> {
        private readonly ValueHashSet<T> HashSet;
        private int Index;

        /// <summary>
        /// Constructs a new enumerator over the elements of <paramref name="hashSet"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ValueHashSet<T> hashSet) {
            HashSet = hashSet;
            Index = -1;
        }

        /// <summary>
        /// Returns the element at the current position of the list.
        /// </summary>
        public readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => HashSet.Buffer[Index];
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
        /// Advances the enumerator to the next element of the list.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator successfully advanced to the next element; <see langword="false"/> if the enumerator reached the end of the list.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            Index++;
            return Index < HashSet.Count;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Index = -1;
        }
    }
}