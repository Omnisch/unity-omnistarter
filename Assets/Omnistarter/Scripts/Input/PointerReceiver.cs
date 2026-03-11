// author: Omnistudio
// version: 2026.03.11

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// PointerReceiver will send messages to all Omnis.PointerBase on self GameObject.<br/>
    /// Needs an Omnis.InputHandler in the scene.
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
        private void OnInteract(List<Collider> siblings) { if (activated) SendMessage("InteractReceiver", siblings); }
        private void OnLeftPress() { if (activated) SendMessage("LeftPressReceiver"); }
        private void OnLeftRelease() { if (activated) SendMessage("LeftReleaseReceiver"); }
        private void OnRightPress() { if (activated) SendMessage("RightPressReceiver"); }
        private void OnRightRelease() { if (activated) SendMessage("RightReleaseReceiver"); }
        private void OnMiddlePress() { if (activated) SendMessage("MiddlePressReceiver"); }
        private void OnMiddleRelease() { if (activated) SendMessage("MiddleReleaseReceiver"); }
        private void OnPointerEnter() { if (activated) SendMessage("PointerEnterReceiver"); }
        private void OnPointerExit() { if (activated) SendMessage("PointerExitReceiver"); }
        #endregion
    }
}
