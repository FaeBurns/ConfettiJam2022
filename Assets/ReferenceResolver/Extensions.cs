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
            MemberInfo[] members = target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic);

            ReferenceResolver referenceResolver = new ReferenceResolver();

            foreach (MemberInfo member in members)
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                {
                    referenceResolver.Resolve(target, member);
                }
            }
        }
    }
}