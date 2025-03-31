using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// Extension methods for <see cref="ValueHashSet{T}"/>.
/// </summary>
public static class ValueSetExtensions {
    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> to a new <see cref="ValueHashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueHashSet<T> ToValueHashSet<T>(this IEnumerable<T> enumerable) {
        return new ValueHashSet<T>(enumerable);
    }

    /// <summary>
    /// Copies the contents of <paramref name="span"/> to a new <see cref="ValueHashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueHashSet<T> ToValueHashSet<T>(this scoped ReadOnlySpan<T> span) {
        return new ValueHashSet<T>(span);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueHashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueHashSet<T> ToValueHashSet<T>(this scoped ValueList<T> valueList) {
        return new ValueHashSet<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="ValueHashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueHashSet<T> ToValueHashSet<T>(this scoped ValueHashSet<T> valueHashSet) {
        return new ValueHashSet<T>(valueHashSet);
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> matching <paramref name="predicate"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueHashSet<T> ToValueHashSetWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
        ValueHashSet<T> result = new();
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
    public static ValueHashSet<T> ToValueHashSetOfType<T>(this IEnumerable enumerable) {
        ValueHashSet<T> result = new();
        foreach (T element in enumerable) {
            if (element is T elementOfT) {
                result.Add(elementOfT);
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="Array"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(this scoped ValueHashSet<T> valueHashSet) {
        return valueHashSet.AsSpan().ToArray();
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> ToList<T>(this scoped ValueHashSet<T> valueHashSet) {
        List<T> list = new(valueHashSet.Count);
        list.AddRange(valueHashSet.AsSpan());
        return list;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="HashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToHashSet<T>(this scoped ValueHashSet<T> valueHashSet) {
        HashSet<T> hashSet = new(valueHashSet.Count);
        foreach (T element in valueHashSet) {
            hashSet.Add(element);
        }
        return hashSet;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keySelector"/> and <paramref name="valueSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this scoped ValueHashSet<T> valueHashSet, Func<T, TKey> keySelector, Func<T, TValue> valueSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueHashSet.Count);
        foreach (T element in valueHashSet) {
            TKey key = keySelector(element);
            TValue value = valueSelector(element);
            dictionary[key] = value;
        }
        return dictionary;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keyValuePairSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this scoped ValueHashSet<T> valueHashSet, Func<T, KeyValuePair<TKey, TValue>> keyValuePairSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueHashSet.Count);
        foreach (T element in valueHashSet) {
            KeyValuePair<TKey, TValue> keyValuePair = keyValuePairSelector(element);
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return dictionary;
    }
}