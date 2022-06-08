using System.Reflection;
using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a unity component into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class BindComponentAttribute : System.Attribute, IResolver
    {
        /// <summary>
        /// Gets or Sets a value indicating whether this <see cref="IResolver"/> should look for the component on children instead.
        /// </summary>
        public bool Child { get; set; }

        /// <inheritdoc/>
        public void Resolve(object hostObject, FieldInfo field)
        {
            if (hostObject is Component caller)
            {
                Component component;

                if (Child)
                {
                    component = caller.GetComponentInChildren(field.FieldType);
                }
                else
                {
                    component = caller.GetComponent(field.FieldType);
                }

                field.SetValue(hostObject, component);
            }
        }
    }
}