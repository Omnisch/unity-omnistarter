// author: Omnistudio
// version: 2026.04.13

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// PointerReceiver will send messages to all Omnis.PointerBase on self GameObject.<br/>
    /// Needs an Omnis.InputRouter in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public sealed class PointerReceiver : MonoBehaviour
    {
        #region Serialized Fields
        public bool activated = true;
        public bool opaque = false;
        #endregion


        #region Methods
        public void SetActivated(bool a) { activated = a; }
        public void SetOpaque(bool o) { opaque = o; }
        #endregion


        #region Messages
        private void OnInteractRaw(List<Collider> siblings) { if (activated) SendMessage("OnInteract", siblings); }
        private void OnLeftPressRaw()     { if (activated) SendMessage("OnLeftPress", SendMessageOptions.DontRequireReceiver); }
        private void OnLeftReleaseRaw()   { if (activated) SendMessage("OnLeftRelease", SendMessageOptions.DontRequireReceiver); }
        private void OnRightPressRaw()    { if (activated) SendMessage("OnRightPress", SendMessageOptions.DontRequireReceiver); }
        private void OnRightReleaseRaw()  { if (activated) SendMessage("OnRightRelease", SendMessageOptions.DontRequireReceiver); }
        private void OnMiddlePressRaw()   { if (activated) SendMessage("OnMiddlePress", SendMessageOptions.DontRequireReceiver); }
        private void OnMiddleReleaseRaw() { if (activated) SendMessage("OnMiddleRelease", SendMessageOptions.DontRequireReceiver); }
        private void OnPointerEnterRaw()  { if (activated) SendMessage("OnPointerEnter", SendMessageOptions.DontRequireReceiver); }
        private void OnPointerExitRaw()   { if (activated) SendMessage("OnPointerExit", SendMessageOptions.DontRequireReceiver); }
        #endregion
    }
}
