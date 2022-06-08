using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BeanLib.References
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class BindComponentAttribute : System.Attribute
    {
        public static void Resolve(object hostObject, FieldInfo field)
        {
            BindComponentAttribute attribte = field.GetCustomAttribute<BindComponentAttribute>();

            // exit if attribute not found
            // field we're trying to resolve simply doesn't have the attribute and should be ignored
            if (attribte is null)
            {
                return;
            }

            if (hostObject is Component caller)
            {
                Component component;

                if (attribte.Child)
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

        public bool Child { get; set; }
    }
}