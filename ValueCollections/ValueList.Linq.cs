using System.Runtime.CompilerServices;

namespace ValueCollections;

partial struct ValueList<T> {
    /// <summary>
    /// Returns every element matching <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Where(Func<T, bool> predicate) {
        ValueList<T> result = new();
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (predicate(element)) {
                result.Add(element);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns every element mapped using <paramref name="selector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Select(Func<T, T> selector) {
        ValueList<T> result = new(Count);
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            result.Add(selector(element));
        }
        return result;
    }

    /// <summary>
    /// Returns every element mapped using <paramref name="selector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Select(Func<T, int, T> selector) {
        ValueList<T> result = new(Count);
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            result.Add(selector(element, index));
        }
        return result;
    }

    /// <summary>
    /// Returns every element mapped to multiple elements using <paramref name="selector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> SelectMany(Func<T, IEnumerable<T>> selector) {
        ValueList<T> result = new(Count);
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            result.AddRange(selector(element));
        }
        return result;
    }

    /// <summary>
    /// Returns every element mapped to multiple elements using <paramref name="selector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> SelectMany(Func<T, int, IEnumerable<T>> selector) {
        ValueList<T> result = new(Count);
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            result.AddRange(selector(element, index));
        }
        return result;
    }

    /// <summary>
    /// Returns the elements with <paramref name="value"/> added to the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Append(T value) {
        ValueList<T> result = new(this);
        result.Add(value);
        return result;
    }

    /// <summary>
    /// Returns the elements with <paramref name="value"/> inserted at the beginning.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Prepend(T value) {
        ValueList<T> result = new(this);
        result.Insert(0, value);
        return result;
    }

    /// <summary>
    /// Returns the elements in reverse order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Reverse() {
        ValueList<T> result = new(this);
        result.AsSpan().Reverse();
        return result;
    }

    /// <summary>
    /// Returns the elements of type <typeparamref name="TFilter"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TFilter> OfType<TFilter>() {
        ValueList<TFilter> result = new();
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (element is TFilter elementOfTFilter) {
                result.Add(elementOfTFilter);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the elements casted to type <typeparamref name="TResult"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Cast<TResult>() {
        ValueList<TResult> result = new();
        for (int index = 0; index < Count; index++) {
            T element = this[index];
            if (element is TResult elementOfTResult) {
                result.Add(elementOfTResult);
            }
            else {
                result.Add((TResult)(object?)element!);
            }
        }
        return result;
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