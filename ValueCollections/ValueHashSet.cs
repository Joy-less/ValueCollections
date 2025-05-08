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

    private Span<T> Buffer;
    private int BufferPosition;
    private T[]? RentedBuffer;
    private Span<int> HashCodes;
    private int[]? RentedHashCodes;

    /// <summary>
    /// Constructs a value hash set with a default capacity of 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet() {

    }
    /// <summary>
    /// Constructs a value hash set with the given capacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(int capacity) {
        EnsureCapacity(capacity);
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped Span<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped ReadOnlySpan<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(ReadOnlyMemory<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-5)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(IEnumerable<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHashSet(scoped ValueList<T> initialElements) {
        AddRange(initialElements.AsSpan());
    }
    /// <summary>
    /// Constructs a value hash set with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-4)]
#endif
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
    /// <exception cref="ArgumentException"/>
    public static ValueHashSet<T> FromBuffer(Span<T> buffer, Span<int> hashCodesBuffer) {
        if (buffer.Length != hashCodesBuffer.Length) {
            throw new ArgumentException($"{nameof(buffer)}.Length should equal {nameof(hashCodesBuffer)}.Length");
        }

        return new ValueHashSet<T>() {
            Buffer = buffer,
            HashCodes = hashCodesBuffer,
        };
    }

    /// <summary>
    /// Disposes the instance and returns the rented buffers to the array pool if needed.
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
    /// Returns the current number of elements in the hash set.
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
    /// Gets a span over the elements in the hash set.
    /// </summary>
    /// <remarks>
    /// Do not change the capacity of the hash set while the span is in use, because the span will continue pointing to the old buffer.<br/>
    /// The span is read-only to ensure the elements are synchronized with the hash codes.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> AsSpan() => Buffer[..BufferPosition];

    /// <summary>
    /// Adds an element to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(T value) {
        if (TryFindIndex(value, out int index)) {
            return false;
        }
        Insert(index, value);
        return true;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T value) {
        Add(value);
    }

    /// <summary>
    /// Adds multiple elements to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(scoped ReadOnlySpan<T> values) {
        EnsureCapacity(BufferPosition + values.Length);
        foreach (T value in values) {
            Add(value);
        }
    }

    /// <summary>
    /// Adds multiple elements to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public void AddRange(ReadOnlyMemory<T> values) {
        AddRange(values.Span);
    }

    /// <summary>
    /// Adds multiple elements to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public void AddRange(IEnumerable<T> values) {
        if (values.TryGetNonEnumeratedCount(out int count)) {
            EnsureCapacity(BufferPosition + count);
            foreach (T value in values) {
                Add(value);
            }
        }
        else {
            foreach (T value in values) {
                Add(value);
            }
        }
    }

    /// <summary>
    /// Ensure's the hash set's capacity is at least <paramref name="newCapacity"/>, renting a larger buffer if not.<br/>
    /// This is useful when adding a predetermined number of items to the hash set.
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
    /// Ensures the hash set's capacity is equal to its count, renting a smaller buffer if not.<br/>
    /// This is useful for reducing memory overhead when it is known that no more elements will be added to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess() {
        if (Count >= Capacity) {
            return;
        }
        Capacity = Count;
    }

    /// <summary>
    /// Returns whether <paramref name="value"/> is found in the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(T value) {
        return TryFindIndex(value, out _);
    }

    /// <summary>
    /// Finds and removes the first instance of <paramref name="value"/> from the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T value) {
        if (TryFindIndex(value, out int index)) {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes every element matching <paramref name="predicate"/> from the hash set.
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
    /// Removes the elements in <paramref name="other"/> from the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExceptWith(scoped ValueHashSet<T> other) {
        foreach (T value in other) {
            Remove(value);
        }
    }

    /// <inheritdoc cref="ExceptWith(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExceptWith(IEnumerable<T> other) {
        foreach (T value in other) {
            Remove(value);
        }
    }

    /// <summary>
    /// Removes the elements not in <paramref name="other"/> from the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IntersectWith(scoped ValueHashSet<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                RemoveAt(index);
                index--;
            }
        }
    }

    /// <inheritdoc cref="IntersectWith(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IntersectWith(IEnumerable<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                RemoveAt(index);
                index--;
            }
        }
    }

    /// <summary>
    /// Returns whether the hash set contains every element in <paramref name="other"/> but is not equal to <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsProperSubsetOf(scoped ValueHashSet<T> other) {
        return IsSubsetOf(other) && !SetEquals(other);
    }

    /// <inheritdoc cref="IsProperSubsetOf(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsProperSubsetOf(IEnumerable<T> other) {
        return IsSubsetOf(other) && !SetEquals(other);
    }

    /// <summary>
    /// Returns whether <paramref name="other"/> contains every element in the hash set but is not equal to <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsProperSupersetOf(scoped ValueHashSet<T> other) {
        return IsSupersetOf(other) && !SetEquals(other);
    }

    /// <inheritdoc cref="IsProperSupersetOf(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsProperSupersetOf(IEnumerable<T> other) {
        return IsSupersetOf(other) && !SetEquals(other);
    }

    /// <summary>
    /// Returns whether the hash set contains every element in <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSubsetOf(scoped ValueHashSet<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc cref="IsSubsetOf(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSubsetOf(IEnumerable<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns whether <paramref name="other"/> contains every element in the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSupersetOf(scoped ValueHashSet<T> other) {
        foreach (T value in other) {
            if (!Contains(value)) {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc cref="IsSupersetOf(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSupersetOf(IEnumerable<T> other) {
        foreach (T value in other) {
            if (!Contains(value)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns whether the hash set and <paramref name="other"/> share at least one common element.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Overlaps(scoped ValueHashSet<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (other.Contains(Buffer[index])) {
                return true;
            }
        }
        foreach (T value in other) {
            if (Contains(value)) {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc cref="Overlaps(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Overlaps(IEnumerable<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (other.Contains(Buffer[index])) {
                return true;
            }
        }
        foreach (T value in other) {
            if (Contains(value)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns whether the hash sets contain the same elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool SetEquals(scoped ValueHashSet<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                return false;
            }
        }
        foreach (T value in other) {
            if (!Contains(value)) {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc cref="SetEquals(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool SetEquals(IEnumerable<T> other) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                return false;
            }
        }
        foreach (T value in other) {
            if (!Contains(value)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Replaces the elements with the elements that are in <paramref name="other"/> but not in this hash set and the elements that are in this hash set but not in <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SymmetricExceptWith(scoped ValueHashSet<T> other) {
        using ValueHashSet<T> copy = new(this);

        Clear();

        foreach (T value in other) {
            if (!copy.Contains(value)) {
                Add(value);
            }
        }
        for (int index = 0; index < copy.BufferPosition; index++) {
            if (!other.Contains(copy.Buffer[index])) {
                Add(copy.Buffer[index]);
            }
        }
    }

    /// <inheritdoc cref="SymmetricExceptWith(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SymmetricExceptWith(IEnumerable<T> other) {
        using ValueHashSet<T> copy = new(this);

        Clear();

        foreach (T value in other) {
            if (!copy.Contains(value)) {
                Add(value);
            }
        }
        for (int index = 0; index < BufferPosition; index++) {
            if (!other.Contains(Buffer[index])) {
                Add(Buffer[index]);
            }
        }
    }

    /// <summary>
    /// Adds the elements in <paramref name="other"/> to the hash set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnionWith(scoped ValueHashSet<T> other) {
        AddRange(other.AsSpan());
    }

    /// <inheritdoc cref="UnionWith(ValueHashSet{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnionWith(IEnumerable<T> other) {
        AddRange(other);
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
    /// Returns an enumerator that iterates over the elements of the hash set.
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
    /// Calculates a hash code for <paramref name="value"/> using <see cref="Comparer"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int GetHashCode(T? value) {
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
            if (leftPointer > rightPointer) {
                return leftPointer;
            }

            int midPointer = (leftPointer + rightPointer) / 2;

            if (HashCodes[midPointer] < hashCode) {
                leftPointer = midPointer + 1;
            }
            else if (HashCodes[midPointer] > hashCode) {
                rightPointer = midPointer - 1;
            }
            else {
                // Move to first entry with same hash code
                while (midPointer >= 1 && HashCodes[midPointer - 1] == hashCode) {
                    midPointer--;
                }
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
        /// Returns the element at the current position of the hash set.
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
        /// Advances the enumerator to the next element of the hash set.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator successfully advanced to the next element; <see langword="false"/> if the enumerator reached the end of the hash set.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            Index++;
            return Index < HashSet.Count;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the hash set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Index = -1;
        }
    }
}