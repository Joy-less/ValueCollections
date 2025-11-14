using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections.FixedSize;

/// <summary>
/// A version of <see cref="List{T}"/> which has a fixed capacity of <see cref="Capacity"/> elements.
/// </summary>
public partial struct ValueList256<T> : IList<T>, IReadOnlyList<T> {
    private InlineBuffer Buffer;
    private int BufferPosition;

    /// <summary>
    /// The fixed capacity of the value list.
    /// </summary>
    public const int Capacity = 256;

    /// <summary>
    /// Constructs a value list with no elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256() {
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256(scoped ReadOnlySpan<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256(ReadOnlyMemory<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-4)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256(IEnumerable<T> initialElements) {
        AddRange(initialElements);
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256(scoped ValueList<T> initialElements) {
        AddRange(initialElements.AsSpan());
    }
    /// <summary>
    /// Constructs a value list with the given elements.
    /// </summary>
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList256(scoped ValueHashSet<T> initialElements) {
        AddRange(initialElements.AsSpan());
    }

    /// <summary>
    /// Returns the current number of elements in the list.
    /// </summary>
    public readonly int Count {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BufferPosition;
    }

    /// <inheritdoc/>
    readonly bool ICollection<T>.IsReadOnly {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    /// <summary>
    /// Returns the element at the given index.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
    public T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get {
            if (index < 0 || index >= BufferPosition) {
                throw new IndexOutOfRangeException("The index is outside the bounds of the value list.");
            }
            return Buffer[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (index < 0 || index >= BufferPosition) {
                throw new IndexOutOfRangeException("The index is outside the bounds of the value list.");
            }
            Buffer[index] = value;
        }
    }

    /// <inheritdoc/>
    T IList<T>.this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => this[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[index] = value;
    }

    /// <inheritdoc/>
    readonly T IReadOnlyList<T>.this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
    }

    /// <summary>
    /// Adds an element to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value) {
        ValidateCapacity(BufferPosition + 1);
        Buffer[BufferPosition] = value;
        BufferPosition++;
    }

    /// <summary>
    /// Adds multiple elements to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(scoped ReadOnlySpan<T> values) {
        ValidateCapacity(BufferPosition + values.Length);
        values.CopyTo(Buffer[BufferPosition..]);
        BufferPosition += values.Length;
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
            ValidateCapacity(BufferPosition + count);
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
    /// Ensures the list's capacity is at least <paramref name="newCapacity"/>, throwing if not.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateCapacity(int newCapacity) {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(newCapacity, Capacity);
    }

    /// <summary>
    /// Returns the index of <paramref name="value"/> or -1 if not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Returns whether <paramref name="value"/> is found in the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(T value) {
        return IndexOf(value) >= 0;
    }

    /// <summary>
    /// Inserts <paramref name="value"/> at <paramref name="index"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, T value) {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, BufferPosition);

        ValidateCapacity(BufferPosition + 1);
        Buffer[index..BufferPosition].CopyTo(Buffer[(index + 1)..]);
        Buffer[index] = value;
        BufferPosition++;
    }

    /// <summary>
    /// Removes an element at <paramref name="index"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Sorts the elements using the default comparer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort() {
        Buffer[..BufferPosition].Sort();
    }

    /// <summary>
    /// Sorts the elements using <paramref name="comparer"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
        Buffer[..BufferPosition].Sort(comparer);
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

    /// <summary>
    /// Gets a span over the elements in the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan(ref ValueList256<T> valueList) {
        return valueList.Buffer[..valueList.BufferPosition];
    }

    /// <summary>
    /// A fixed-size sequential buffer.
    /// </summary>
    [InlineArray(length: Capacity)]
    private struct InlineBuffer {
        public T Element0;
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="ValueList{T}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<T> {
        private readonly ValueList256<T> List;
        private int Index;

        /// <summary>
        /// Constructs a new enumerator over the elements of <paramref name="list"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ValueList256<T> list) {
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
            return Index < List.Count;
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