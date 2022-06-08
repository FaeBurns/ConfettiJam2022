using System.Reflection;

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

            ReferenceResolver referenceResolver = new ReferenceResolver();

            foreach (FieldInfo field in fields)
            {
                referenceResolver.Resolve(target, field);
            }
        }
    }
}