namespace ValueCollections;

partial struct ValueList<T> {
    /// <summary>
    /// Removes every element not matching <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    public ValueList<T> Where(Func<T, bool> predicate) {
        int index = 0;
        while (index < BufferPosition) {
            if (!predicate(Buffer[index])) {
                RemoveAt(index);
            }
            else {
                index++;
            }
        }
        return this;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    public ValueList<T> Select(Func<T, T> selector) {
        for (int index = 0; index < BufferPosition; index++) {
            Buffer[index] = selector(Buffer[index]);
        }
        return this;
    }
    /// <summary>
    /// Transforms each element using <paramref name="selector"/>.
    /// </summary>
    /// <remarks>
    /// This method affects the original list.
    /// </remarks>
    public ValueList<T> Select(Func<T, int, T> selector) {
        for (int index = 0; index < BufferPosition; index++) {
            Buffer[index] = selector(Buffer[index], index);
        }
        return this;
    }
}