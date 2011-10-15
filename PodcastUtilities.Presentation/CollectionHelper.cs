using System;
using System.Collections.Generic;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Presentation
{
    public static class CollectionHelper
    {
        public static IEnumerable<DefaultableValueTypeItem<T>> CreateForDefaultableEnum<T>() where T : struct
        {
            if (!typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException("Type is not an Enum");
            }

            var collection = new List<DefaultableValueTypeItem<T>>
                                 {
                                     CreateItem<T>()
                                 };

            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                var newItem = CreateItem<T>();
                newItem.Value = (T)enumValue;

                collection.Add(newItem);
            }

            return collection;
        }

        private static DefaultableValueTypeItem<T> CreateItem<T>() where T : struct
        {
            return new DefaultableValueTypeItem<T>(() => default(T));
        }
    }
}