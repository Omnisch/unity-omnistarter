// author: Omnistudio
// version: 2025.11.23

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// Use Invoke() to invoke the callback event.
    /// </summary>
    public class SwitchLogic : MonoBehaviour
    {
        public bool initState;
        public UnityEvent onCallback;
        public UnityEvent offCallback;

        private bool state;
        private bool State {
            get => state;
            set {
                if (value && !state) {
                    onCallback?.Invoke();
                } else if (!value && state) {
                    offCallback?.Invoke();
                }
                state = value;
            }
        }
        
        [ContextMenu("Invoke")]
        public void Invoke() => State = !State;

        [ContextMenu("Switch On")]
        public void SwitchOn() => State = true;

        [ContextMenu("Switch Off")]
        public void SwitchOff() => State = false;

        private void Start() {
            // Invoke as initState on start
            state = !initState;
            State = initState;
        }
    }
}
