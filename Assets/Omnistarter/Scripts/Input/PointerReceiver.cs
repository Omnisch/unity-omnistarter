// author: Omnistudio
// version: 2025.09.03

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// PointerReceiver will send messages to all Omnis.PointerBase on self GameObject.<br/>
    /// Needs an Omnis.InputHandler in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider))] [DisallowMultipleComponent]
    public sealed class PointerReceiver : MonoBehaviour
    {
        #region Serialized fields
        public bool activated = true;
        public bool opaque = false;
        #endregion

        #region Messages
        private void OnInteract(List<Collider> siblings) { if (this.activated) this.SendMessage("InteractReceiver", siblings); }
        private void OnLeftPress() { if (this.activated) this.SendMessage("LeftPressReceiver"); }
        private void OnLeftRelease() { if (this.activated) this.SendMessage("LeftReleaseReceiver"); }
        private void OnRightPress() { if (this.activated) this.SendMessage("RightPressReceiver"); }
        private void OnRightRelease() { if (this.activated) this.SendMessage("RightReleaseReceiver"); }
        private void OnMiddlePress() { if (this.activated) this.SendMessage("MiddlePressReceiver"); }
        private void OnMiddleRelease() { if (this.activated) this.SendMessage("MiddleReleaseReceiver"); }
        private void OnPointerEnter() { if (this.activated) this.SendMessage("PointerEnterReceiver"); }
        private void OnPointerExit() { if (this.activated) this.SendMessage("PointerExitReceiver"); }
        #endregion
    }
}
