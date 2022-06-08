using System.Reflection;

namespace BeanLib.References
{
    /// <summary>
    /// Interface responsible for implementing resolvers.
    /// </summary>
    internal interface IResolver
    {
        /// <summary>
        /// Tries to resolve the reference on the target object.
        /// </summary>
        /// <param name="hostObject">The object containing the field.</param>
        /// <param name="field">The field that requires the reference.</param>
        public void Resolve(object hostObject, FieldInfo field);
    }
}
