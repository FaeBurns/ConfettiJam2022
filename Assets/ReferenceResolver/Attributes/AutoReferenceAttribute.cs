using System.Reflection;
using BeanLib.References.Exceptions;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a reference into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AutoReferenceAttribute : System.Attribute, IResolver
    {
        /// <inheritdoc/>
        public void Resolve(object hostObject, MemberInfo member)
        {
            // throw if reference not found
            if (!ReferenceStore.ContainsReferenceType(ResolverUtility.GetMemberType(member)))
            {
                throw new ReferenceNotFoundException($"Could not find reference of type {ResolverUtility.GetMemberType(member)} in the reference store");
            }

            object reference = ReferenceStore.GetReference(ResolverUtility.GetMemberType(member));

            ResolverUtility.SetMemberValue(hostObject, reference, member);
        }
    }
}