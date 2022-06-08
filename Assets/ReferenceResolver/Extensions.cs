using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Global extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Resolves all references on <paramref name="target"/> with an <see cref="AutoReferenceAttribute"/> attribute.
        /// </summary>
        /// <param name="target">The target object to resolve the references on.</param>
        public static void ResolveReferences(this object target)
        {
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                ReferenceResolver referenceResolver = new ReferenceResolver(target, field);
                referenceResolver.Resolve();
            }
        }
    }
}