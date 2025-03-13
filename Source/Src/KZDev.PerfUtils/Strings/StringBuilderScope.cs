using System.Text;

namespace KZDev.PerfUtils
{
    //================================================================================
    /// <summary>
    /// Represents a scope in which a cached <see cref="Builder"/> instance is 
    /// retrieved and released back to the cache.
    /// </summary>
    public readonly struct StringBuilderScope : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilderScope"/> struct.
        /// </summary>
        /// <param name="capacity">
        /// The capacity of the <see cref="Builder"/> instance to acquire.
        /// </param>
        internal StringBuilderScope (int capacity)
        {
            Builder = StringBuilderCache.Acquire(capacity);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the <see cref="Builder"/> instance that was retrieved from the
        /// cache for this scope.
        /// </summary>
        public StringBuilder Builder { get; }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public void Dispose ()
        {
            StringBuilderCache.Release(Builder);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string value of the <see cref="Builder"/> instance.
        /// </summary>
        /// <returns>
        /// The string value of the <see cref="Builder"/> instance.
        /// </returns>
        public override string ToString () => Builder.ToString();
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Converts the value of a substring of the <see cref="Builder"/> instance to
        /// a string by using the specified starting position and length of the substring.
        /// </summary>
        /// <param name="startIndex">
        /// The starting position of the substring in this instance.
        /// </param>
        /// <param name="length">
        /// The length of the substring.
        /// </param>
        public string ToString (int startIndex, int length) => Builder.ToString(startIndex, length);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Operator to implicitly convert the <see cref="StringBuilderScope"/> instance to a
        /// <see cref="Builder"/> for use in code that expects a <see cref="Builder"/>.
        /// </summary>
        /// <param name="scope">
        /// The scope instance to convert to a <see cref="Builder"/>.
        /// </param>
        public static implicit operator StringBuilder (in StringBuilderScope scope) =>
            scope.Builder;
        //--------------------------------------------------------------------------------
    }
}
