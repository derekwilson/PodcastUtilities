using System;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an item that can have a value or provide a default value
    /// </summary>
    public class DefaultableReferenceTypeItem<T> : IDefaultableItem<T> where T : class
    {
        private readonly Func<T> _findDefaultValue;

        private T _value;

        /// <summary>
        /// create a defaultable item and specify the method of obtaining the default value
        /// </summary>
        public DefaultableReferenceTypeItem(Func<T> defaultValueFinder)
        {
            _findDefaultValue = defaultValueFinder;
        }

        /// <summary>
        /// the item value, or its default value if not set
        /// </summary>
        public T Value
        {
            get { return (_value ?? _findDefaultValue()); }
            set { _value = value; }
        }

        /// <summary>
        /// true if the value is set at this level
        /// </summary>
        public bool IsSet
        {
            get { return _value != null; }
        }

        /// <summary>
        /// remove the value at this level
        /// </summary>
        public void RevertToDefault()
        {
            _value = null;
        }

        ///<summary>
        /// Make this into a copy of the source, ie. if the source IsSet copy its value,
        /// otherwise revert this to default.
        ///</summary>
        ///<param name="source"></param>
        public void Copy(IDefaultableItem<T> source)
        {
            if (source.IsSet)
            {
                Value = source.Value;
            }
            else
            {
                RevertToDefault();
            }
        }
    }
}