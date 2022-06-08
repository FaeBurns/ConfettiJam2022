using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References.Exceptions;

namespace BeanLib.References
{
    /// <summary>
    /// Object responsible for resolving references registered by <see cref="AutoReferenceAttribute"/> attributes.
    /// </summary>
    public class ReferenceResolver
    {
        private readonly object hostObject;
        private readonly FieldInfo field;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceResolver"/> class.
        /// </summary>
        /// <param name="field">The field to resolve the reference on.</param>
        /// <param name="hostObject">The object this instance of the field is present on.</param>
        public ReferenceResolver(object hostObject, FieldInfo field)
        {
            this.hostObject = hostObject;
            this.field = field;
        }

        /// <summary>
        /// Tries to resolve references present on the target field.
        /// </summary>
        public void Resolve()
        {
            AutoReferenceAttribute attribte = field.GetCustomAttribute<AutoReferenceAttribute>();

            // exit if attribute not found
            // field we're trying to resolve simply doesn't have the attribute and should be ignored
            if (attribte is null)
            {
                return;
            }

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
