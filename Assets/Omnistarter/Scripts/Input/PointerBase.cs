// author: Omnistudio
// version: 2025.09.03

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
        public virtual bool Active
        {
            get => this.active;
            protected set => this.active = value;
        }
        public virtual bool LeftPressed
        {
            get => this.leftPressed;
            protected set => this.leftPressed = value;
        }
        public virtual bool RightPressed
        {
            get => this.rightPressed;
            protected set => this.rightPressed = value;
        }
        public virtual bool MiddlePressed
        {
            get => this.middlePressed;
            protected set => this.middlePressed = value;
        }
        public virtual bool Pointed
        {
            get => this.pointed;
            protected set => this.pointed = value;
        }
        #endregion

        #region Functions
        protected virtual void OnInteracted(List<Collider> siblings) { }
        #endregion

        #region Messages
        protected void InteractReceiver(List<Collider> siblings) { if (this.Active) OnInteracted(siblings); }
        private void LeftPressReceiver()      { if (this.Active) this.LeftPressed = true; }
        private void LeftReleaseReceiver()    { if (this.Active) this.LeftPressed = false; }
        private void RightPressReceiver()     { if (this.Active) this.RightPressed = true; }
        private void RightReleaseReceiver()   { if (this.Active) this.RightPressed = false; }
        private void MiddlePressReceiver()    { if (this.Active) this.MiddlePressed = true; }
        private void MiddleReleaseReceiver()  { if (this.Active) this.MiddlePressed = false; }
        private void PointerEnterReceiver()   { if (this.Active) this.Pointed = true; }
        private void PointerExitReceiver()    { if (this.Active) this.Pointed = false; }
        #endregion
    }
}
