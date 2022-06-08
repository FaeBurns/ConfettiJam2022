using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Base class used.
    /// </summary>
    public abstract class ReferenceResolvedBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Unity Message Start.
        /// </summary>
        public virtual void Start()
        {
            RunResolve();
        }

        /// <summary>
        /// Resolves any specified references.
        /// </summary>
        protected void RunResolve()
        {
            this.ResolveReferences();
        }
    }
}
