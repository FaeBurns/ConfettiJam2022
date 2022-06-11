using System;
using System.Reflection;
using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a unity component into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class BindMultiComponentAttribute : System.Attribute, IResolver
    {
        /// <summary>
        /// Gets or Sets a value indicating whether this <see cref="IResolver"/> should look for the component on children instead.
        /// </summary>
        public bool Child { get; set; }

        /// <inheritdoc/>
        public void Resolve(object hostObject, MemberInfo member)
        {
            if (hostObject is Component caller)
            {
                Component[] components;

                Type collectionType = ResolverUtility.GetCollectionElementType(member);

                if (Child)
                {
                    components = caller.GetComponentsInChildren(collectionType);
                }
                else
                {
                    components = caller.GetComponents(collectionType);
                }

                object[] result = CastToType(components, collectionType);

                ResolverUtility.SetMemberValue(hostObject, result, member);
            }
        }

        private static T[] Convert<T>(Component[] components)
            where T : Component
        {
            return Array.ConvertAll(components, (component) => (T)component);
        }

        private object[] CastToType(Component[] components, Type targetType)
        {
            MethodInfo method = typeof(BindMultiComponentAttribute).GetMethod("Convert", BindingFlags.NonPublic | BindingFlags.Static);

            method = method.MakeGenericMethod(targetType);

            // must encapsulate argument in object array to stop issues from ocurring with params
            return (object[])method.Invoke(null, new object[] { components });
        }
    }
}