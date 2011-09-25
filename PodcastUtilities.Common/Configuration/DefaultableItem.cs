using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an item that can have a value or provide a default value
    /// </summary>
    public class DefaultableItem<T> : IDefaultableItem<T> where T : struct 
    {
        private Func<T> _findDefaultValue;

        private T? _value;

        /// <summary>
        /// create a defaultable item and specify the method of obtaining the default value
        /// </summary>
        public DefaultableItem(Func<T> defaultValueFinder)
        {
            _findDefaultValue = defaultValueFinder;
        }

        /// <summary>
        /// the item value, or its default value if not set
        /// </summary>
        public T Value
        {
            get { return _value.GetValueOrDefault(_findDefaultValue()); }
            set { _value = value; }
        }

        /// <summary>
        /// true if the value is set at this level
        /// </summary>
        public bool IsSet
        {
            get { return _value.HasValue; }
        }

        /// <summary>
        /// remove the value at this level
        /// </summary>
        public void RevertToDefault()
        {
            _value = null;
        }
    }
}
