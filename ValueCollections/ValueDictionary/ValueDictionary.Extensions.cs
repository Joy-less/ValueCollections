using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// Extension methods for <see cref="ValueDictionary{TKey, TValue}"/>.
/// </summary>
public static class ValueDictionaryExtensions {
    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> to a new <see cref="ValueDictionary{TKey, TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) {
        return new ValueDictionary<TKey, TValue>(enumerable);
    }

    /// <summary>
    /// Copies the contents of <paramref name="span"/> to a new <see cref="ValueDictionary{TKey, TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionary<TKey, TValue>(this scoped ReadOnlySpan<KeyValuePair<TKey, TValue>> span) {
        return new ValueDictionary<TKey, TValue>(span);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="ValueDictionary{TKey, TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionary<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        return new ValueDictionary<TKey, TValue>(valueDictionary);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueDictionary{TKey, TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionary<TKey, TValue>(this scoped ValueList<KeyValuePair<TKey, TValue>> valueList) {
        return new ValueDictionary<TKey, TValue>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> matching <paramref name="predicate"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionaryWhere<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        ValueDictionary<TKey, TValue> result = new();
        foreach (KeyValuePair<TKey, TValue> entry in enumerable) {
            if (predicate(entry)) {
                result[entry.Key] = entry.Value;
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> of type <see cref="KeyValuePair{TKey, TValue}"/> to a new <see cref="ValueList{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToValueDictionaryOfType<TKey, TValue>(this IEnumerable enumerable) {
        ValueDictionary<TKey, TValue> result = new();
        foreach (object? element in enumerable) {
            if (element is KeyValuePair<TKey, TValue> entry) {
                result[entry.Key] = entry.Value;
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="Array"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KeyValuePair<TKey, TValue>[] ToArray<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        return valueDictionary.AsSpan().ToArray();
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<KeyValuePair<TKey, TValue>> ToList<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        List<KeyValuePair<TKey, TValue>> list = new(valueDictionary.Count);
        list.AddRange(valueDictionary.AsSpan());
        return list;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueDictionary.Count);
        foreach (KeyValuePair<TKey, TValue> entry in valueDictionary) {
            dictionary[entry.Key] = entry.Value;
        }
        return dictionary;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keySelector"/> and <paramref name="valueSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSourceKey, TSourceValue, TKey, TValue>(this scoped ValueDictionary<TSourceKey, TSourceValue> valueDictionary, Func<KeyValuePair<TSourceKey, TSourceValue>, TKey> keySelector, Func<KeyValuePair<TSourceKey, TSourceValue>, TValue> valueSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueDictionary.Count);
        foreach (KeyValuePair<TSourceKey, TSourceValue> entry in valueDictionary) {
            TKey key = keySelector(entry);
            TValue value = valueSelector(entry);
            dictionary[key] = value;
        }
        return dictionary;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keyValuePairSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSourceKey, TSourceValue, TKey, TValue>(this scoped ValueDictionary<TSourceKey, TSourceValue> valueDictionary, Func<KeyValuePair<TSourceKey, TSourceValue>, KeyValuePair<TKey, TValue>> keyValuePairSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueDictionary.Count);
        foreach (KeyValuePair<TSourceKey, TSourceValue> entry in valueDictionary) {
            KeyValuePair<TKey, TValue> keyValuePair = keyValuePairSelector(entry);
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return dictionary;
    }
}