namespace APG.Shared
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns a new enumerable collection that contains the elements from source with the last count elements of the source collection omitted.
        /// </summary>
        /// <remarks>SkipLast doesn't exist in .NET Framework</remarks>
        /// <typeparam name="T">The type of the elements in the enumerable collection.</typeparam>
        /// <param name="source">An enumerable collection instance.</param>
        /// <param name="count">The number of elements to omit from the end of the collection.</param>
        /// <returns>A new enumerable collection that contains the elements from source minus count elements from the end of the collection.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static IEnumerable<T> SkipLastForFramework<T>(this IEnumerable<T> source, int count)
        {
            var queue = new Queue<T>();

            using var e = source.GetEnumerator();

            while (e.MoveNext())
            {
                if (queue.Count == count)
                {
                    do
                    {
                        yield return queue.Dequeue();
                        queue.Enqueue(e.Current);
                    } while (e.MoveNext());
                }
                else
                {
                    queue.Enqueue(e.Current);
                }
            }
        }
    }
}
