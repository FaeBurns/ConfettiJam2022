using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BeanLib.References
{
    /// <summary>
    /// Object responsible for resolving references registered by <see cref="AutoReferenceAttribute"/> attributes.
    /// </summary>
    public class ReferenceResolver
    {
        private readonly HashSet<Type> resolverTypes = new HashSet<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceResolver"/> class.
        /// </summary>
        public ReferenceResolver()
        {
            resolverTypes = GetAllResolverTypes();
        }

        /// <summary>
        /// Tries to resolve references present on the target field.
        /// </summary>
        /// <param name="hostObject">The object this instance of the field is present on.</param>
        /// <param name="field">The field to resolve the reference on.</param>
        public void Resolve(object hostObject, FieldInfo field)
        {
            foreach (Type resolverType in resolverTypes)
            {
                IResolver resolver = (IResolver)field.GetCustomAttribute(resolverType);

                // check if resolver is present on the field
                if (resolver != null)
                {
                    resolver.Resolve(hostObject, field);
                }
            }
        }

        /// <summary>
        /// Gets all types that implement the interface <see cref="IResolver"/>.
        /// </summary>
        /// <returns>A <see cref="HashSet{T}"/>containing all resolver types.</returns>
        public HashSet<Type> GetAllResolverTypes()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where((t) => t.GetInterfaces().Contains(typeof(IResolver)));

            HashSet<Type> setTypes = new HashSet<Type>();

            foreach (Type type in types)
            {
                setTypes.Add(type);
            }

            return setTypes;
        }
    }
}
