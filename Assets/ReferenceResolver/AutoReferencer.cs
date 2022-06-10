using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ReferenceStore.ReplaceReference(component);
            }
        }
    }
}
