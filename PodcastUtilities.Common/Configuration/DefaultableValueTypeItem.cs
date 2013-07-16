#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
