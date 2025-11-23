// author: WangNianyi2001
// version: 2025.11.23

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
