using System;
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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IResolver"/> should look for the componet on siblings instead.
        /// </summary>
        public bool Sibling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IResolver"/> should look for the component on its parent instead.
        /// </summary>
        public bool Parent { get; set; }

        /// <inheritdoc/>
        public void Resolve(object hostObject, MemberInfo member)
        {
            if (hostObject is Component caller)
            {
                Component component;

                Type memberType = ResolverUtility.GetMemberType(member);

                if (Child)
                {
                    component = caller.GetComponentInChildren(memberType);
                }
                else if (Sibling)
                {
                    component = caller.transform.parent.gameObject.GetComponentInChildren(memberType);
                }
                else if (Parent)
                {
                    component = caller.GetComponentInParent(memberType);
                }
                else
                {
                    component = caller.GetComponent(memberType);
                }

                ResolverUtility.SetMemberValue(hostObject, component, member);
            }
        }
    }
}