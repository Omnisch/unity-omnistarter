// author: Omnistudio
// version: 2026.03.05

using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    public abstract class ActorBase : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private bool interactable = true;
        [SerializeField] private bool jumpable = true;
        #endregion

        #region Fields
        private InputRouter router;
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
        protected virtual void OnInteracted(List<GameObject> fellows) {}
        #endregion

        #region Unity methods
        protected virtual void Start()
        {
            router = FindFirstObjectByType<InputRouter>();
            if (router)
                router.AddListener(gameObject);
            else
                Destroy(gameObject);
        }
        protected virtual void OnDestroy()
        {
            if (router) router.RemoveListener(gameObject);
        }
        #endregion

        #region Messages
        protected void OnInteract(List<GameObject> fellows) { if (Interactable) OnInteracted(fellows); }
        protected virtual void OnMove(Vector2 value)
        {
            if (Interactable)
            {
                HorizontalAxis = value.x;
                VerticalAxis = value.y;
            }
        }
        protected virtual void OnJump(float value) { if (Interactable) JumpAxis = value; }
        #endregion
    }

    public enum MovingType
    {
        Normal3D,
        Plane2D,
        Platform2D,
    }
}
