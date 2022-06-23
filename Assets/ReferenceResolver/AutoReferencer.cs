using UnityEngine;

namespace BeanLib.References
{
    /// <summary>
    /// Component responsible for creating automatic references of other components.
    /// </summary>
    public class AutoReferencer : MonoBehaviour
    {
        [SerializeField] private Component[] components;

        private void Awake()
        {
            foreach (Component component in components)
            {
                ReferenceStore.RegisterReference(component);
            }
        }
    }
}
