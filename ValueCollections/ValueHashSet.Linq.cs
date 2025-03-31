using System.Runtime.CompilerServices;

namespace ValueCollections;

partial struct ValueHashSet<T> {
    /// <inheritdoc cref="ValueList{T}.Where(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Where(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).Where(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Select(Func{T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Select(Func<T, T> selector) {
        return ValueList<T>.FromBuffer(Buffer).Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Select(Func{T, int, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Select(Func<T, int, T> selector) {
        return ValueList<T>.FromBuffer(Buffer).Select(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany(Func{T, IEnumerable{T}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> SelectMany(Func<T, IEnumerable<T>> selector) {
        return ValueList<T>.FromBuffer(Buffer).SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.SelectMany(Func{T, int, IEnumerable{T}})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> SelectMany(Func<T, int, IEnumerable<T>> selector) {
        return ValueList<T>.FromBuffer(Buffer).SelectMany(selector);
    }

    /// <inheritdoc cref="ValueList{T}.Append(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Append(T value) {
        return ValueList<T>.FromBuffer(Buffer).Append(value);
    }

    /// <inheritdoc cref="ValueList{T}.Prepend(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Prepend(T value) {
        return ValueList<T>.FromBuffer(Buffer).Prepend(value);
    }

    /// <inheritdoc cref="ValueList{T}.Reverse()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Reverse() {
        return ValueList<T>.FromBuffer(Buffer).Reverse();
    }

    /// <inheritdoc cref="ValueList{T}.OfType{TFilter}()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TFilter> OfType<TFilter>() {
        return ValueList<T>.FromBuffer(Buffer).OfType<TFilter>();
    }

    /// <inheritdoc cref="ValueList{T}.Cast{TResult}()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<TResult> Cast<TResult>() {
        return ValueList<T>.FromBuffer(Buffer).Cast<TResult>();
    }

    /// <inheritdoc cref="ValueList{T}.Except(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Except(T value) {
        return ValueList<T>.FromBuffer(Buffer).Except(value);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Except(scoped ReadOnlySpan<T> values) {
        return ValueList<T>.FromBuffer(Buffer).Except(values);
    }

    /// <inheritdoc cref="ValueList{T}.Except(ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-1)]
#endif
    public readonly ValueList<T> Except(ReadOnlyMemory<T> values) {
        return ValueList<T>.FromBuffer(Buffer).Except(values.Span);
    }

    /// <inheritdoc cref="ValueList{T}.Except(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(-2)]
#endif
    public readonly ValueList<T> Except(IEnumerable<T> values) {
        return ValueList<T>.FromBuffer(Buffer).Except(values);
    }

    /// <inheritdoc cref="ValueList{T}.Order()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Order() {
        return ValueList<T>.FromBuffer(Buffer).Order();
    }

    /// <inheritdoc cref="ValueList{T}.Order{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> Order<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
        return ValueList<T>.FromBuffer(Buffer).Order(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderDescending() {
        return ValueList<T>.FromBuffer(Buffer).OrderDescending();
    }

    /// <inheritdoc cref="ValueList{T}.OrderDescending{TComparer}(TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderDescending<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
        return ValueList<T>.FromBuffer(Buffer).OrderDescending(comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderBy<TKey>(Func<T, TKey> keySelector) {
        return ValueList<T>.FromBuffer(Buffer).OrderBy(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderBy{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderBy<TKey, TComparer>(Func<T, TKey> keySelector, TComparer comparer) where TComparer : IComparer<TKey> {
        return ValueList<T>.FromBuffer(Buffer).OrderBy(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey}(Func{T, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderByDescending<TKey>(Func<T, TKey> keySelector) {
        return ValueList<T>.FromBuffer(Buffer).OrderByDescending(keySelector);
    }

    /// <inheritdoc cref="ValueList{T}.OrderByDescending{TKey, TComparer}(Func{T, TKey}, TComparer)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueList<T> OrderByDescending<TKey, TComparer>(Func<T, TKey> keySelector, TComparer comparer) where TComparer : IComparer<TKey> {
        return ValueList<T>.FromBuffer(Buffer).OrderByDescending(keySelector, comparer);
    }

    /// <inheritdoc cref="ValueList{T}.All(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool All(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).All(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.Any()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any() {
        return ValueList<T>.FromBuffer(Buffer).Any();
    }

    /// <inheritdoc cref="ValueList{T}.Any(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).Any(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.First()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First() {
        return ValueList<T>.FromBuffer(Buffer).First();
    }

    /// <inheritdoc cref="ValueList{T}.First(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T First(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).First(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).FirstOrDefault(defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.FirstOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? FirstOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).FirstOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Last()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last() {
        return ValueList<T>.FromBuffer(Buffer).Last();
    }

    /// <inheritdoc cref="ValueList{T}.Last(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Last(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).Last(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).LastOrDefault(defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.LastOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? LastOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).LastOrDefault(predicate, defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.Single()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single() {
        return ValueList<T>.FromBuffer(Buffer).Single();
    }

    /// <inheritdoc cref="ValueList{T}.Single(Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Single(Func<T, bool> predicate) {
        return ValueList<T>.FromBuffer(Buffer).Single(predicate);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).SingleOrDefault(defaultValue);
    }

    /// <inheritdoc cref="ValueList{T}.SingleOrDefault(Func{T, bool}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T? SingleOrDefault(Func<T, bool> predicate, T? defaultValue = default) {
        return ValueList<T>.FromBuffer(Buffer).SingleOrDefault(predicate, defaultValue);
    }
}