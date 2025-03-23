using System.Runtime.CompilerServices;

namespace ValueCollections;

partial struct ValueList<T> {
    /// <summary>
    /// Removes every element not matching <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> Where(Func<T, bool> predicate) {
        int index = 0;
        while (index < BufferPosition) {
            if (!predicate(Buffer[index])) {
                RemoveAt(index);
            }
            else {
                index++;
            }
        }
        return this;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> Select(Func<T, T> selector) {
        for (int index = 0; index < BufferPosition; index++) {
            Buffer[index] = selector(Buffer[index]);
        }
        return this;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> Select(Func<T, int, T> selector) {
        for (int index = 0; index < BufferPosition; index++) {
            Buffer[index] = selector(Buffer[index], index);
        }
        return this;
    }
    /// <summary>
    /// Adds an element to the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> Append(T value) {
        Add(value);
        return this;
    }
    /// <summary>
    /// Inserts an element at the beginning of the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> Prepend(T value) {
        Insert(0, value);
        return this;
    }
    /// <summary>
    /// Reverses the order of the elements of the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Reverse() {
        AsSpan().Reverse();
        return this;
    }

    /// <summary>
    /// Returns whether all elements match <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool All(Func<T, bool> predicate) {
        for (int index = 0; index < BufferPosition; index++) {
            if (!predicate(Buffer[index])) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns whether any element matches <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any(Func<T, bool> predicate) {
        for (int index = 0; index < BufferPosition; index++) {
            if (predicate(Buffer[index])) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Returns the first element.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First() {
        return this[0];
    }
    /// <summary>
    /// Returns the first element matching <paramref name="predicate"/>.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First(Func<T, bool> predicate) {
        for (int index = 0; index < BufferPosition; index++) {
            T element = Buffer[index];
            if (predicate(element)) {
                return element;
            }
        }
        throw new Exception("No elements match the predicate.");
    }
    /// <summary>
    /// Returns the first element or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault() {
        if (Count <= 0) {
            return default;
        }
        return First();
    }
    /// <summary>
    /// Returns the first element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(Func<T, bool> predicate) {
        for (int index = 0; index < BufferPosition; index++) {
            T element = Buffer[index];
            if (predicate(element)) {
                return element;
            }
        }
        return default;
    }
    /// <summary>
    /// Returns the last element.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last() {
        return this[^1];
    }
    /// <summary>
    /// Returns the last element matching <paramref name="predicate"/>.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last(Func<T, bool> predicate) {
        for (int index = BufferPosition - 1; index >= 0; index--) {
            T element = Buffer[index];
            if (predicate(element)) {
                return element;
            }
        }
        throw new Exception("No elements match the predicate.");
    }
    /// <summary>
    /// Returns the last element or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault() {
        if (Count <= 0) {
            return default;
        }
        return Last();
    }
    /// <summary>
    /// Returns the last element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(Func<T, bool> predicate) {
        for (int index = BufferPosition - 1; index >= 0; index--) {
            T element = Buffer[index];
            if (predicate(element)) {
                return element;
            }
        }
        return default;
    }
}