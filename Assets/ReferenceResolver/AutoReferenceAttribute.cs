using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Injects a reference into a field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class AutoReferenceAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoReferenceAttribute"/> class.
        /// </summary>
        /// <param name="fieldName">The field to reference.</param>
        public AutoReferenceAttribute()
        {
        }
    }
}