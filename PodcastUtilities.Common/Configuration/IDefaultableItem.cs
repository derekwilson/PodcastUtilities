using System;
namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an item that can have a value or provide a default value
    /// </summary>
    public interface IDefaultableItem<T>
    {
        /// <summary>
        /// true if the value is set at this level
        /// </summary>
        bool IsSet { get; }

        /// <summary>
        /// remove the value at this level
        /// </summary>
        void RevertToDefault();

        /// <summary>
        /// the item value, or its default value if not set
        /// </summary>
        T Value { get; set; }
    }
}
