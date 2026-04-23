using System.Collections;
using System.Runtime.CompilerServices;

namespace ValueCollections;

/// <summary>
/// Extension methods for <see cref="ValueStack{T}"/>.
/// </summary>
public static class ValueStackExtensions {
    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-5)]
#endif
    public static ValueStack<T> ToValueStack<T>(this IEnumerable<T> enumerable) {
        return new ValueStack<T>(enumerable);
    }

    /// <summary>
    /// Copies the contents of <paramref name="span"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueStack<T> ToValueStack<T>(this scoped ReadOnlySpan<T> span) {
        return new ValueStack<T>(span);
    }

    /// <summary>
    /// Copies the contents of <paramref name="memory"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public static ValueStack<T> ToValueStack<T>(this ReadOnlyMemory<T> memory) {
        return new ValueStack<T>(memory);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public static ValueStack<T> ToValueStack<T>(this scoped ValueList<T> valueList) {
        return new ValueStack<T>(valueList);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueHashSet"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-3)]
#endif
    public static ValueStack<T> ToValueStack<T>(this scoped ValueHashSet<T> valueHashSet) {
        return new ValueStack<T>(valueHashSet);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-4)]
#endif
    public static ValueStack<T> ToValueStack<T>(this scoped ValueStack<T> valueStack) {
        return new ValueStack<T>(valueStack);
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueDictionary"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-5)]
#endif
    public static ValueStack<KeyValuePair<TKey, TValue>> ToValueStack<TKey, TValue>(this scoped ValueDictionary<TKey, TValue> valueDictionary) {
        ValueStack<KeyValuePair<TKey, TValue>> valueStack = new();
        valueStack.PushRange(valueDictionary.AsSpan());
        return valueStack;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> matching <paramref name="predicate"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueStack<T> ToValueStackWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
        ValueStack<T> result = new();
        foreach (T element in enumerable) {
            if (predicate(element)) {
                result.Push(element);
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="enumerable"/> of type <typeparamref name="T"/> to a new <see cref="ValueStack{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueStack<T> ToValueStackOfType<T>(this IEnumerable enumerable) {
        ValueStack<T> result = new();
        foreach (object? element in enumerable) {
            if (element is T elementOfT) {
                result.Push(elementOfT);
            }
        }
        return result;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="Array"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(this scoped ValueStack<T> valueStack) {
        T[] array = new T[valueStack.Count];
        for (int index = valueStack.Count - 1; index >= 0; index--) {
            int arrayIndex = valueStack.Count - 1 - index;

            array[arrayIndex] = valueStack[index];
        }
        return array;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> ToList<T>(this scoped ValueStack<T> valueStack) {
        List<T> list = new(valueStack.Count);
        foreach (T element in valueStack) {
            list.Add(element);
        }
        return list;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="HashSet{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToHashSet<T>(this scoped ValueStack<T> valueStack) {
        HashSet<T> hashSet = new(valueStack.Count);
        foreach (T element in valueStack) {
            hashSet.Add(element);
        }
        return hashSet;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keySelector"/> and <paramref name="valueSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this scoped ValueStack<TSource> valueStack, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueStack.Count);
        foreach (TSource element in valueStack) {
            TKey key = keySelector(element);
            TValue value = valueSelector(element);
            dictionary[key] = value;
        }
        return dictionary;
    }

    /// <summary>
    /// Copies the contents of <paramref name="valueStack"/> to a new <see cref="Dictionary{TKey, TValue}"/> using <paramref name="keyValuePairSelector"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this scoped ValueStack<TSource> valueStack, Func<TSource, KeyValuePair<TKey, TValue>> keyValuePairSelector) where TKey : notnull {
        Dictionary<TKey, TValue> dictionary = new(valueStack.Count);
        foreach (TSource element in valueStack) {
            KeyValuePair<TKey, TValue> keyValuePair = keyValuePairSelector(element);
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return dictionary;
    }
}