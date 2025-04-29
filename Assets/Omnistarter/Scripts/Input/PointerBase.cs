// author: Omnistudio
// version: 2024.12.04

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    [RequireComponent(typeof(Collider))]
    public abstract class PointerBase : MonoBehaviour
    {
        #region Serialized fields
        public bool opaque;
        #endregion

        #region Fields
        private bool interactable;
        private bool pointed;
        private bool leftPressed;
        private bool rightPressed;
        private bool middlePressed;
        #endregion

        #region Properties
        public virtual bool Interactable
        {
            get => interactable;
            set => interactable = value;
        }
        public virtual bool LeftPressed
        {
            get => leftPressed;
            set => leftPressed = value;
        }
        public virtual bool RightPressed
        {
            get => rightPressed;
            set => rightPressed = value;
        }
        public virtual bool MiddlePressed
        {
            get => middlePressed;
            set => middlePressed = value;
        }
        public virtual bool Pointed
        {
            get => pointed;
            set => pointed = value;
        }
        #endregion

        #region Functions
        protected virtual void OnInteracted(List<Collider> siblings) {}
        #endregion

        #region Unity methods
        protected virtual void Start()
        {
            interactable = true;
            pointed = false;
        }
        #endregion

        #region Messages
        protected void OnInteract(List<Collider> siblings) { if (Interactable) OnInteracted(siblings); }
        private void OnLeftPress()      { if (Interactable) LeftPressed = true; }
        private void OnLeftRelease()    { if (Interactable) LeftPressed = false; }
        private void OnRightPress()     { if (Interactable) RightPressed = true; }
        private void OnRightRelease()   { if (Interactable) RightPressed = false; }
        private void OnMiddlePress()    { if (Interactable) MiddlePressed = true; }
        private void OnMiddleRelease()  { if (Interactable) MiddlePressed = false; }
        private void OnPointerEnter()   { if (Interactable) Pointed = true; }
        private void OnPointerExit()    { if (Interactable) Pointed = false; }
        #endregion
    }
}
