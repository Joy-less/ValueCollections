using System.Runtime.CompilerServices;

namespace ValueCollections;

partial struct ValueHashSet<T> {
    /// <inheritdoc cref="ValueList{T}.Where(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Where(Func<T, bool> predicate) {
        return this.ToValueList().Where(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Select{TResult}(Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Select<TResult>(Func<T, TResult> selector) {
        return this.ToValueList().Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Select{TResult}(Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Select<TResult>(Func<T, int, TResult> selector) {
        return this.ToValueList().Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany{TResult}(Func{T, IEnumerable{TResult}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector) {
        return this.ToValueList().SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany{TResult}(Func{T, int, IEnumerable{TResult}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> SelectMany<TResult>(Func<T, int, IEnumerable<TResult>> selector) {
        return this.ToValueList().SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Append(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Append(T value) {
        return this.ToValueList().Append(value);
    }

    /// <inheritdoc cref="ValueList{T}.Prepend(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Prepend(T value) {
        return this.ToValueList().Prepend(value);
    }

    /// <inheritdoc cref="ValueList{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Reverse() {
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
    public readonly ValueList<T> Except(T value) {
        return this.ToValueList().Except(value);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Except(scoped ReadOnlySpan<T> values) {
        return this.ToValueList().Except(values);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public readonly ValueList<T> Except(ReadOnlyMemory<T> values) {
        return this.ToValueList().Except(values.Span);
    }

    /// <inheritdoc cref="ValueList{T}.Except(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public readonly ValueList<T> Except(IEnumerable<T> values) {
        return this.ToValueList().Except(values);
    }

    /// <inheritdoc cref="ValueList{T}.Order()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Order() {
        return this.ToValueList().Order();
    }

    /// <inheritdoc cref="ValueList{T}.Order{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Order<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
        return this.ToValueList().Order(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderDescending() {
        return this.ToValueList().OrderDescending();
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderDescending<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
        return this.ToValueList().OrderDescending(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderBy<TKey>(Func<T, TKey> keySelector) {
        return this.ToValueList().OrderBy(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderBy<TKey, TComparer>(Func<T, TKey> keySelector, TComparer comparer) where TComparer : IComparer<TKey> {
        return this.ToValueList().OrderBy(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderByDescending<TKey>(Func<T, TKey> keySelector) {
        return this.ToValueList().OrderByDescending(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderByDescending<TKey, TComparer>(Func<T, TKey> keySelector, TComparer comparer) where TComparer : IComparer<TKey> {
        return this.ToValueList().OrderByDescending(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.All(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool All(Func<T, bool> predicate) {
        return this.ToValueList().All(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Any()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any() {
        return Count > 0;
    }

    /// <inheritdoc cref="ValueList{T}.Any(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any(Func<T, bool> predicate) {
        return this.ToValueList().Any(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.First()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First() {
        if (Count <= 0) {
            throw new IndexOutOfRangeException("The value hash set contains no elements.");
        }
        return Buffer[0];
    }

    /// <inheritdoc cref="ValueList{T}.First(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First(Func<T, bool> predicate) {
        return this.ToValueList().First(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(T? defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return Buffer[0];
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return this.ToValueList().FirstOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Last()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last() {
        if (Count <= 0) {
            throw new IndexOutOfRangeException("The value hash set contains no elements.");
        }
        return Buffer[^1];
    }

    /// <inheritdoc cref="ValueList{T}.Last(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last(Func<T, bool> predicate) {
        return this.ToValueList().Last(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(T? defaultValue = default) {
        if (Count <= 0) {
            return defaultValue;
        }
        return Buffer[^1];
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return this.ToValueList().LastOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Single()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single() {
        return this.ToValueList().Single();
    }

    /// <inheritdoc cref="ValueList{T}.Single(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single(Func<T, bool> predicate) {
        return this.ToValueList().Single(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(T? defaultValue = default) {
        return this.ToValueList().SingleOrDefault(defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return this.ToValueList().SingleOrDefault(predicate, defaultValue);
    }
}