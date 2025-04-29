// author: Omnistudio
// version: 2024.10.28

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Use Invoke() to invoke the callback event.
    /// </summary>
    public class Logic : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent callback;
        
        [ContextMenu("Invoke")]
        public virtual void Invoke() => callback?.Invoke();
    }
}
