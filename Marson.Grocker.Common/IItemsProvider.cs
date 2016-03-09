// This code is based on DataVirtualization by Paul McClean
// http://www.codeproject.com/Articles/34405/WPF-Data-Virtualization
using System.Collections.Generic;

namespace Marson.Grocker.Common
{
    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IItemsProvider<T>
    {
        /// <summary>
        /// Gets the total number of items available.
        /// </summary>
        int GetCount();

        /// <summary>
        /// Gets a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns>A list of 0 to count items starting at startIndex</returns>
        IList<T> GetRange(int startIndex, int count);
    }
}
