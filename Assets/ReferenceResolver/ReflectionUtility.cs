using System;
using System.Collections.Generic;

namespace BeanLib.References
{
    /// <summary>
    /// https://www.codeproject.com/Tips/5267157/How-to-Get-a-Collection-Element-Type-Using-Reflect.
    /// </summary>
    internal static class ReflectionUtility
    {
        /// <summary>
        /// Retrieves the collection element type from this type.
        /// </summary>
        /// <param name="type">The type to query.</param>
        /// <returns>The element type of the collection or null if the type was not a collection.
        /// </returns>
        public static Type GetCollectionElementType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // first try the generic way
            // this is easy, just query the IEnumerable<T> interface for its generic parameter
            var etype = typeof(IEnumerable<>);
            foreach (var bt in type.GetInterfaces())
            {
                if (bt.IsGenericType && bt.GetGenericTypeDefinition() == etype)
                {
                    return bt.GetGenericArguments()[0];
                }
            }

            // now try the non-generic way

            // if it's a dictionary we always return DictionaryEntry
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
            {
                return typeof(System.Collections.DictionaryEntry);
            }

            // if it's a list we look for an Item property with an int index parameter
            // where the property type is anything but object
            if (typeof(System.Collections.IList).IsAssignableFrom(type))
            {
                foreach (var prop in type.GetProperties())
                {
                    if (prop.Name == "Item" && typeof(object) != prop.PropertyType)
                    {
                        var ipa = prop.GetIndexParameters();
                        if (ipa.Length == 1 && typeof(int) == ipa[0].ParameterType)
                        {
                            return prop.PropertyType;
                        }
                    }
                }
            }

            // if it's a collection, we look for an Add() method whose parameter is
            // anything but object
            if (typeof(System.Collections.ICollection).IsAssignableFrom(type))
            {
                foreach (var meth in type.GetMethods())
                {
                    if (meth.Name == "Add")
                    {
                        var pa = meth.GetParameters();
                        if (pa.Length == 1 && typeof(object) != pa[0].ParameterType)
                        {
                            return pa[0].ParameterType;
                        }
                    }
                }
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                return typeof(object);
            }

            return null;
        }
    }
}
