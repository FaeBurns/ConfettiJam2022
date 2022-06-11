using System.Reflection;
using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a unity component into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class BindComponentAttribute : System.Attribute, IResolver
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
                Component component;

                if (Child)
                {
                    component = caller.GetComponentInChildren(ResolverUtility.GetMemberType(member));
                }
                else
                {
                    component = caller.GetComponent(ResolverUtility.GetMemberType(member));
                }

                ResolverUtility.SetMemberValue(hostObject, component, member);
            }
        }
    }
}