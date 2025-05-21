using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// Extension methods for <see cref="ValueList{T}"/>.
/// </summary>
public static class ValueListExtensions {
    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this IEnumerable<T> enumerable) {
        return new ValueList<T>(enumerable);
    }

    /// <summary>
    /// Copies the contents of <paramref name="span"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this scoped ReadOnlySpan<T> span) {
        return new ValueList<T>(span);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this scoped ValueList<T> valueList) {
        return new ValueList<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this ValueList128<T> valueList) {
        return new ValueList<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this scoped ValueHashSet<T> valueHashSet) {
        return new ValueList<T>(valueHashSet);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<KeyValuePair<TKey, TValue>> ToValueList<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        ValueList<KeyValuePair<TKey, TValue>> valueList = new();
        valueList.AddRange(valueDictionary.AsSpan());
        return valueList;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> matching <paramref name="predicate"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueListWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
        ValueList<T> result = new();
        foreach (T element in enumerable) {
            if (predicate(element)) {
                result.Add(element);
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> of type <typeparamref name="T"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueListOfType<T>(this IEnumerable enumerable) {
        ValueList<T> result = new();
        foreach (object? element in enumerable) {
            if (element is T elementOfT) {
                result.Add(elementOfT);
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="Array"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(this scoped ValueList<T> valueList) {
        return valueList.AsSpan().ToArray();
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> ToList<T>(this scoped ValueList<T> valueList) {
        List<T> list = new(valueList.Count);
        list.AddRange(valueList.AsSpan());
        return list;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="HashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToHashSet<T>(this scoped ValueList<T> valueList) {
        HashSet<T> hashSet = new(valueList.Count);
        foreach (T element in valueList) {
            hashSet.Add(element);
        }
        return hashSet;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keySelector"/> and <paramref name="valueSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this scoped ValueList<TSource> valueList, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueList.Count);
        foreach (TSource element in valueList) {
            TKey key = keySelector(element);
            TValue value = valueSelector(element);
            dictionary[key] = value;
        }
        return dictionary;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keyValuePairSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this scoped ValueList<TSource> valueList, Func<TSource, KeyValuePair<TKey, TValue>> keyValuePairSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueList.Count);
        foreach (TSource element in valueList) {
            KeyValuePair<TKey, TValue> keyValuePair = keyValuePairSelector(element);
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return dictionary;
    }
}