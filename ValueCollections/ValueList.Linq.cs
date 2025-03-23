using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// LINQ methods for <see cref="ValueList{T}"/>.
/// </summary>
/// <remarks>
/// These methods affects the original list.
/// </remarks>
public static class ValueListLinq {
    /// <summary>
    /// Removes every element not matching <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Where<T>(this ValueList<T> valueList, Func<T, bool> predicate) {
        int index = 0;
        while (index < valueList.Count) {
            if (!predicate(valueList[index])) {
                valueList.RemoveAt(index);
            }
            else {
                index++;
            }
        }
        return valueList;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Select<T>(this ValueList<T> valueList, Func<T, T> selector) {
        for (int index = 0; index < valueList.Count; index++) {
            valueList[index] = selector(valueList[index]);
        }
        return valueList;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Select<T>(this ValueList<T> valueList, Func<T, int, T> selector) {
        for (int index = 0; index < valueList.Count; index++) {
            valueList[index] = selector(valueList[index], index);
        }
        return valueList;
    }
    /// <summary>
    /// Adds an element to the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Append<T>(this ValueList<T> valueList, T value) {
        valueList.Add(value);
        return valueList;
    }
    /// <summary>
    /// Inserts an element at the beginning of the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Prepend<T>(this ValueList<T> valueList, T value) {
        valueList.Insert(0, value);
        return valueList;
    }
    /// <summary>
    /// Reverses the order of the elements of the list.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> Reverse<T>(this ValueList<T> valueList) {
        valueList.AsSpan().Reverse();
        return valueList;
    }

    /// <summary>
    /// Returns whether all elements match <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate) {
        for (int index = 0; index < valueList.Count; index++) {
            if (!predicate(valueList[index])) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns whether any element matches <paramref name="predicate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate) {
        for (int index = 0; index < valueList.Count; index++) {
            if (predicate(valueList[index])) {
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
    public static T First<T>(this scoped ValueList<T> valueList) {
        return valueList[0];
    }
    /// <summary>
    /// Returns the first element matching <paramref name="predicate"/>.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate) {
        for (int index = 0; index < valueList.Count; index++) {
            T element = valueList[index];
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
    public static T? FirstOrDefault<T>(this scoped ValueList<T> valueList, T? defaultValue = default) {
        if (valueList.Count <= 0) {
            return defaultValue;
        }
        return valueList[0];
    }
    /// <summary>
    /// Returns the first element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate, T? defaultValue = default) {
        for (int index = 0; index < valueList.Count; index++) {
            T element = valueList[index];
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
    public static T Last<T>(this scoped ValueList<T> valueList) {
        return valueList[^1];
    }
    /// <summary>
    /// Returns the last element matching <paramref name="predicate"/>.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T>(this ValueList<T> valueList, Func<T, bool> predicate) {
        for (int index = valueList.Count - 1; index >= 0; index--) {
            T element = valueList[index];
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
    public static T? LastOrDefault<T>(this scoped ValueList<T> valueList, T? defaultValue = default) {
        if (valueList.Count <= 0) {
            return defaultValue;
        }
        return valueList[^1];
    }
    /// <summary>
    /// Returns the last element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate, T? defaultValue = default) {
        for (int index = valueList.Count - 1; index >= 0; index--) {
            T element = valueList[index];
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
    public static T Single<T>(this scoped ValueList<T> valueList) {
        if (valueList.Count != 1) {
            throw new IndexOutOfRangeException("The value list does not contain exactly one element.");
        }
        return valueList[0];
    }
    /// <summary>
    /// Ensures the list has exactly one element matching <paramref name="predicate"/> and returns that element.
    /// </summary>
    /// <exception cref="Exception"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Single<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate) {
        bool found = false;
        T result = default!;
        for (int index = 0; index < valueList.Count; index++) {
            T element = valueList[index];
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
    public static T? SingleOrDefault<T>(this scoped ValueList<T> valueList, T? defaultValue = default) {
        if (valueList.Count != 1) {
            return defaultValue;
        }
        return valueList[0];
    }
    /// <summary>
    /// Ensures the list has exactly one element and returns that element matching <paramref name="predicate"/> or the default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? SingleOrDefault<T>(this scoped ValueList<T> valueList, Func<T, bool> predicate, T? defaultValue = default) {
        bool found = false;
        T result = default!;
        for (int index = 0; index < valueList.Count; index++) {
            T element = valueList[index];
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