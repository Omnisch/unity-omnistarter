// author: Omnistudio
// version: 2024.12.04

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    public abstract class ActorBase : MonoBehaviour
    {
        #region Fields
        private InputHandler handler;
        private bool interactable;

        private bool jumpable;
        private float jumpAxis;
        private float horizontalAxis;
        private float verticalAxis;
        #endregion

        #region Properties
        public virtual bool Interactable
        {
            get => interactable;
            set => interactable = value;
        }
        public virtual bool Jumpable
        {
            get => jumpable;
            set => jumpable = value;
        }
        public virtual float JumpAxis
        {
            get => jumpAxis;
            set
            {
                if (!Jumpable)
                    jumpAxis = 0f;
                else
                    jumpAxis = value;
            }
        }
        public virtual float HorizontalAxis
        {
            get => horizontalAxis;
            set => horizontalAxis = value;
        }
        public virtual float VerticalAxis
        {
            get => verticalAxis;
            set => verticalAxis = value;
        }
        #endregion

        #region Functions
        protected virtual void OnInteracted(List<ActorBase> siblings) {}
        #endregion

        #region Unity methods
        protected virtual void Start()
        {
            interactable = true;
            jumpable = true;

            handler = FindFirstObjectByType<InputHandler>();
            if (handler)
                handler.AddListener(gameObject);
            else
                Destroy(gameObject);
        }
        private void OnDestroy()
        {
            if (handler) handler.RemoveListener(gameObject);
        }
        #endregion

        #region Messages
        protected void OnInteract(List<ActorBase> siblings) { if (Interactable) OnInteracted(siblings); }
        private void OnMove(Vector2 value)
        {
            if (Interactable)
            {
                HorizontalAxis = value.x;
                VerticalAxis = value.y;
            }
        }
        private void OnJump(float value) { if (Interactable) JumpAxis = value; }
        #endregion
    }

    public enum MovingType
    {
        Normal3D,
        Plane2D,
        Platform2D,
    }
}
