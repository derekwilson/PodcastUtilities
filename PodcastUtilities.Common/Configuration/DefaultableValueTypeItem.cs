using System;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an item that can have a value or provide a default value
    /// </summary>
    public class DefaultableValueTypeItem<T> : IDefaultableItem<T> where T : struct
    {
        private readonly Func<T> _findDefaultValue;

        private T? _value;

        /// <summary>
        /// create a defaultable item and specify the method of obtaining the default value
        /// </summary>
        public DefaultableValueTypeItem(Func<T> defaultValueFinder)
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

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var compareObject = obj as DefaultableValueTypeItem<T>;
            if (compareObject == null)
            {
                return false;
            }

            return _value.Equals(compareObject._value);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
