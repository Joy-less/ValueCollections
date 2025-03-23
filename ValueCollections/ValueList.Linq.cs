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
        while (index < Count) {
            if (!predicate(this[index])) {
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
        for (int index = 0; index < Count; index++) {
            this[index] = selector(this[index]);
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
        for (int index = 0; index < Count; index++) {
            this[index] = selector(this[index], index);
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
    /// Removes each element which is not of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueList<T> OfType<TFilter>() {
        int index = 0;
        while (index < Count) {
            if (this[index] is not TFilter) {
                RemoveAt(index);
            }
            else {
                index++;
            }
        }
        return this;
    }

    /// <summary>
    /// Returns whether all elements match <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool All(Func<T, bool> predicate) {
        for (int index = 0; index < Count; index++) {
            if (!predicate(this[index])) {
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
        for (int index = 0; index < Count; index++) {
            if (predicate(this[index])) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Returns the first element.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
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
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (predicate(element)) {
                return element;
            }
        }
        throw new Exception("No elements in the value list match the predicate.");
    }
    /// <summary>
    /// Returns the first element or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(T? defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return this[0];
    }
    /// <summary>
    /// Returns the first element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (predicate(element)) {
                return element;
            }
        }
        return defaultValue;
    }
    /// <summary>
    /// Returns the last element.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
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
        for (int index = Count - 1; index >= 0; index--) {
            T element = this[index];
            if (predicate(element)) {
                return element;
            }
        }
        throw new Exception("No elements in the value list match the predicate.");
    }
    /// <summary>
    /// Returns the last element or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(T? defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return this[^1];
    }
    /// <summary>
    /// Returns the last element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        for (int index = Count - 1; index >= 0; index--) {
            T element = this[index];
            if (predicate(element)) {
                return element;
            }
        }
        return defaultValue;
    }
    /// <summary>
    /// Ensures the list has exactly one element and returns that element.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single() {
        if (Count != 1) {
            throw new IndexOutOfRangeException("The value list does not contain exactly one element.");
        }
        return this[0];
    }
    /// <summary>
    /// Ensures the list has exactly one element matching <paramref name="predicate"/> and returns that element.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single(Func<T, bool> predicate) {
        bool found = false;
        T result = default!;
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (predicate(element)) {
                if (found) {
                    throw new IndexOutOfRangeException("The value list contains more than one element matching the predicate.");
                }
                found = true;
                result = element;
            }
        }
        if (found) {
            return result;
        }
        throw new Exception("No elements in the value list match the predicate.");
    }
    /// <summary>
    /// Ensures the list has exactly one element and returns that element or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(T? defaultValue = default) {
        if (Count != 1) {
            return defaultValue;
        }
        return this[0];
    }
    /// <summary>
    /// Ensures the list has exactly one element and returns that element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        bool found = false;
        T result = default!;
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (predicate(element)) {
                if (found) {
                    return defaultValue;
                }
                found = true;
                result = element;
            }
        }
        if (found) {
            return result;
        }
        return defaultValue;
    }
}