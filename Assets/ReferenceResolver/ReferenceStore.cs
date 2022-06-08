using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.References
{
    /// <summary>
    /// A store containing all references intended to be resolved through <see cref="AutoReferenceAttribute"/> attributes.
    /// </summary>
    public static class ReferenceStore
    {
        private static readonly Dictionary<Type, object> References = new Dictionary<Type, object>();

        /// <summary>
        /// Registers a new reference of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="reference">The object to register.</param>
        public static void RegisterReference<T>(T reference)
        {
            Type referenceType = reference.GetType();

            if (References.ContainsKey(referenceType))
            {
                throw new ArgumentException($"Reference of type {referenceType} already exists in store");
            }

            References.Add(referenceType, reference);
        }

        /// <summary>
        /// Replaces an already existing reference with a new one of the same type.
        /// If no reference of the specified type can already be found, then a new one is added.
        /// </summary>
        /// <typeparam name="T">The type to replace.</typeparam>
        /// <param name="reference">The object to register.</param>
        public static void ReplaceReference<T>(T reference)
        {
            Type referenceType = reference.GetType();

            References[referenceType] = reference;
        }

        /// <summary>
        /// Clears all references from the store.
        /// </summary>
        public static void Clear()
        {
            References.Clear();
        }

        /// <summary>
        /// Checks to see if a reference of the desired type is contained in the store.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if the desired type is contained, false if not.</returns>
        public static bool ContainsReferenceType(Type type)
        {
            return References.ContainsKey(type);
        }

        /// <summary>
        /// Gets a reference of the specified type.
        /// </summary>
        /// <param name="type">The type to get.</param>
        /// <returns>The desired reference.</returns>
        public static object GetReference(Type type)
        {
            return References[type];
        }
    }
}
