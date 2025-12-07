using System.Runtime.CompilerServices;

namespace ValueCollections;

partial struct ValueDictionary<TKey, TValue> {
    /// <inheritdoc cref="ValueList{T}.Where(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Where(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().Where(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Select{TResult}(Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Select<TResult>(Func<KeyValuePair<TKey, TValue>, TResult> selector) {
        return this.ToValueList().Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Select{TResult}(Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Select<TResult>(Func<KeyValuePair<TKey, TValue>, int, TResult> selector) {
        return this.ToValueList().Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany{TResult}(Func{T, IEnumerable{TResult}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> SelectMany<TResult>(Func<KeyValuePair<TKey, TValue>, IEnumerable<TResult>> selector) {
        return this.ToValueList().SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany{TResult}(Func{T, int, IEnumerable{TResult}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> SelectMany<TResult>(Func<KeyValuePair<TKey, TValue>, int, IEnumerable<TResult>> selector) {
        return this.ToValueList().SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Append(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Append(KeyValuePair<TKey, TValue> entry) {
        return this.ToValueList().Append(entry);
    }

    /// <inheritdoc cref="ValueList{T}.Prepend(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Prepend(KeyValuePair<TKey, TValue> entry) {
        return this.ToValueList().Prepend(entry);
    }

    /// <inheritdoc cref="ValueList{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Reverse() {
        return this.ToValueList().Reverse();
    }

    /// <inheritdoc cref="ValueList{T}.OfType{TFilter}()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TFilter> OfType<TFilter>() {
        return this.ToValueList().OfType<TFilter>();
    }

    /// <inheritdoc cref="ValueList{T}.Cast{TResult}()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Cast<TResult>() {
        return this.ToValueList().Cast<TResult>();
    }

    /// <inheritdoc cref="ValueList{T}.Except(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Except(KeyValuePair<TKey, TValue> entry) {
        return this.ToValueList().Except(entry);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Except(scoped ReadOnlySpan<KeyValuePair<TKey, TValue>> entries) {
        return this.ToValueList().Except(entries);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public readonly ValueList<KeyValuePair<TKey, TValue>> Except(ReadOnlyMemory<KeyValuePair<TKey, TValue>> entries) {
        return this.ToValueList().Except(entries.Span);
    }

    /// <inheritdoc cref="ValueList{T}.Except(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public readonly ValueList<KeyValuePair<TKey, TValue>> Except(IEnumerable<KeyValuePair<TKey, TValue>> entries) {
        return this.ToValueList().Except(entries);
    }

    /// <inheritdoc cref="ValueList{T}.Order()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Order() {
        return this.ToValueList().Order();
    }

    /// <inheritdoc cref="ValueList{T}.Order{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> Order<TComparer>(TComparer comparer) where TComparer : IComparer<KeyValuePair<TKey, TValue>> {
        return this.ToValueList().Order(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderDescending() {
        return this.ToValueList().OrderDescending();
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderDescending<TComparer>(TComparer comparer) where TComparer : IComparer<KeyValuePair<TKey, TValue>> {
        return this.ToValueList().OrderDescending(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderBy<TOrderKey>(Func<KeyValuePair<TKey, TValue>, TOrderKey> keySelector) {
        return this.ToValueList().OrderBy(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderBy<TOrderKey, TComparer>(Func<KeyValuePair<TKey, TValue>, TOrderKey> keySelector, TComparer comparer) where TComparer : IComparer<TOrderKey> {
        return this.ToValueList().OrderBy(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderByDescending<TOrderKey>(Func<KeyValuePair<TKey, TValue>, TOrderKey> keySelector) {
        return this.ToValueList().OrderByDescending(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<KeyValuePair<TKey, TValue>> OrderByDescending<TOrderKey, TComparer>(Func<KeyValuePair<TKey, TValue>, TOrderKey> keySelector, TComparer comparer) where TComparer : IComparer<TOrderKey> {
        return this.ToValueList().OrderByDescending(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.All(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool All(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().All(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Any()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any() {
        return Count > 0;
    }

    /// <inheritdoc cref="ValueList{T}.Any(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().Any(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.First()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> First() {
        if (Count <= 0) {
            throw new IndexOutOfRangeException("The value dictionary contains no elements.");
        }
        return Buffer[0];
    }

    /// <inheritdoc cref="ValueList{T}.First(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> First(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().First(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> FirstOrDefault(KeyValuePair<TKey, TValue> defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return Buffer[0];
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> FirstOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate, KeyValuePair<TKey, TValue> defaultValue = default) {
        return this.ToValueList().FirstOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Last()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> Last() {
        if (Count <= 0) {
            throw new IndexOutOfRangeException("The value dictionary contains no elements.");
        }
        return Buffer[^1];
    }

    /// <inheritdoc cref="ValueList{T}.Last(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> Last(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().Last(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> LastOrDefault(KeyValuePair<TKey, TValue> defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return Buffer[^1];
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> LastOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate, KeyValuePair<TKey, TValue> defaultValue = default) {
        return this.ToValueList().LastOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Single()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> Single() {
        return this.ToValueList().Single();
    }

    /// <inheritdoc cref="ValueList{T}.Single(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> Single(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        return this.ToValueList().Single(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> SingleOrDefault(KeyValuePair<TKey, TValue> defaultValue = default) {
        return this.ToValueList().SingleOrDefault(defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly KeyValuePair<TKey, TValue> SingleOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate, KeyValuePair<TKey, TValue> defaultValue = default) {
        return this.ToValueList().SingleOrDefault(predicate, defaultValue);
    }
}