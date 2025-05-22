using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections.FixedSize;

/// <summary>
/// Extension methods for <see cref="ValueList8{T}"/>.
/// </summary>
public static class ValueList8Extensions {
    /// <summary>
    /// Gets a span over the elements in the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ref ValueList8<T> valueList) {
        return ValueList8<T>.AsSpan(ref valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-6)]
#endif
    public static ValueList8<T> ToValueList8<T>(this IEnumerable<T> enumerable) {
        return new ValueList8<T>(enumerable);
    }

    /// <summary>
    /// Copies the contents of <paramref name="span"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList8<T> ToValueList8<T>(this scoped ReadOnlySpan<T> span) {
        return new ValueList8<T>(span);
    }

    /// <summary>
    /// Copies the contents of <paramref name="memory"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public static ValueList8<T> ToValueList8<T>(this ReadOnlyMemory<T> memory) {
        return new ValueList8<T>(memory);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public static ValueList8<T> ToValueList8<T>(this ValueList8<T> valueList) {
        return new ValueList8<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    public static ValueList8<T> ToValueList8<T>(this ValueList<T> valueList) {
        return new ValueList8<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-4)]
#endif
    public static ValueList8<T> ToValueList8<T>(this scoped ValueHashSet<T> valueHashSet) {
        return new ValueList8<T>(valueHashSet);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-5)]
#endif
    public static ValueList8<KeyValuePair<TKey, TValue>> ToValueList8<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        ValueList8<KeyValuePair<TKey, TValue>> valueList = new();
        valueList.AddRange(valueDictionary.AsSpan());
        return valueList;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> matching <paramref name="predicate"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList8<T> ToValueList8Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
        ValueList8<T> result = new();
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
    public static ValueList8<T> ToValueList8OfType<T>(this IEnumerable enumerable) {
        ValueList8<T> result = new();
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
    public static T[] ToArray<T>(this ValueList8<T> valueList) {
        return valueList.AsSpan().ToArray();
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> ToList<T>(this ValueList8<T> valueList) {
        List<T> list = new(valueList.Count);
        list.AddRange(valueList.AsSpan());
        return list;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="HashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToHashSet<T>(this ValueList8<T> valueList) {
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
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this ValueList8<TSource> valueList, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull {
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
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this ValueList8<TSource> valueList, Func<TSource, KeyValuePair<TKey, TValue>> keyValuePairSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueList.Count);
        foreach (TSource element in valueList) {
            KeyValuePair<TKey, TValue> keyValuePair = keyValuePairSelector(element);
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return dictionary;
    }
}