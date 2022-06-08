using System.Reflection;
using BeanLib.References.Exceptions;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a reference into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class AutoReferenceAttribute : System.Attribute, IResolver
    {
        /// <inheritdoc/>
        public void Resolve(object hostObject, FieldInfo field)
        {
            // throw if reference not found
            if (!ReferenceStore.ContainsReferenceType(field.FieldType))
            {
                throw new ReferenceNotFoundException($"Could not find reference of type {field.FieldType} in the reference store");
            }

            object reference = ReferenceStore.GetReference(field.FieldType);

            field.SetValue(hostObject, reference);
        }
    }
}