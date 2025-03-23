#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0305 // Simplify collection initialization
#pragma warning disable IDE0306 // Simplify collection initialization

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
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="Array"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(this ValueList<T> valueList) {
        return valueList.AsSpan().ToArray();
    }
    /// <summary>
    /// Copies the contents of <paramref name="valueList"/> to a new <see cref="List{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> ToList<T>(this ValueList<T> valueList) {
        List<T> list = new(valueList.Count);
        list.AddRange(valueList.AsSpan());
        return list;
    }
}