// author: Omnistudio
// version: 2026.03.10

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// The base component of pointer-interactive 3D GameObjects.<br/>
    /// Needs an Omnis.InputHandler in the scene<br/>
    /// as well as an Omnis.PointerReceiver on this GameObject.
    /// </summary>
    [RequireComponent(typeof(PointerReceiver))]
    public abstract class PointerBase : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private bool active = true;
        #endregion

        #region Fields
        private bool pointed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool middlePressed = false;
        #endregion

        #region Properties
        public virtual bool Active {
            get => active;
            protected set => active = value;
        }
        public virtual bool LeftPressed {
            get => leftPressed;
            protected set => leftPressed = value;
        }
        public virtual bool RightPressed {
            get => rightPressed;
            protected set => rightPressed = value;
        }
        public virtual bool MiddlePressed {
            get => middlePressed;
            protected set => middlePressed = value;
        }
        public virtual bool Pointed {
            get => pointed;
            protected set => pointed = value;
        }
        #endregion

        #region Functions
        protected virtual void OnInteracted(List<Collider> siblings) { }
        #endregion

        #region Messages
        protected void InteractReceiver(List<Collider> siblings) { if (Active) OnInteracted(siblings); }
        private void LeftPressReceiver()      { if (Active) LeftPressed = true; }
        private void LeftReleaseReceiver()    { if (Active) LeftPressed = false; }
        private void RightPressReceiver()     { if (Active) RightPressed = true; }
        private void RightReleaseReceiver()   { if (Active) RightPressed = false; }
        private void MiddlePressReceiver()    { if (Active) MiddlePressed = true; }
        private void MiddleReleaseReceiver()  { if (Active) MiddlePressed = false; }
        private void PointerEnterReceiver()   { if (Active) Pointed = true; }
        private void PointerExitReceiver()    { if (Active) Pointed = false; }
        #endregion
    }
}
