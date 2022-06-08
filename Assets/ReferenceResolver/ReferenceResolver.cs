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
            AutoReferenceAttribute.Resolve(hostObject, field);
            BindComponentAttribute.Resolve(hostObject, field);
        }
    }
}
