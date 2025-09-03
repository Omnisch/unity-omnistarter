// author: Omnistudio
// version: 2025.09.03

using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnis
{
    public class DesktopDraggable : PointerBase
    {
        #region Serialized Fields
        [SerializeField] private DesktopSurface surface = null;
        [SerializeField] private float gridSnap = 0f;
        #endregion


        #region Fields
        private Vector3 mouseOffset;
        private Vector2 pressMousePosition;
        private bool moved;
        #endregion


        #region Properties
        public override bool LeftPressed
        {
            get => base.LeftPressed;
            protected set
            {
                if (value && !base.LeftPressed)
                {
                    var raycastResult = this.surface.MouseRaycast(false, false, gridSnap);
                    if (raycastResult.hit)
                    {
                        this.mouseOffset = this.WorldPosition - raycastResult.point;
                        this.pressMousePosition = Mouse.current.position.ReadValue();
                        this.moved = false;
                    }
                }
                else if (!value && base.LeftPressed)
                {
                    var raycastResult = this.surface.MouseRaycast(true, false, gridSnap);
                    if (this.Active && raycastResult.hit)
                    {
                        if (raycastResult.clamped) {
                            this.WorldPosition = raycastResult.point;
                        }
                        else {
                            this.WorldPosition = raycastResult.point + this.mouseOffset;
                        }
                    }
                }

                base.LeftPressed = value;
            }
        }

        public Vector3 WorldPosition
        {
            get => this.transform.position;
            set
            {
                if (this.TryGetComponent<DesktopSurface>(out var selfSurface))
                    selfSurface.MoveSelfWithAllDraggablesTo(value);
                else
                    this.transform.position = value;
            }
        }
        #endregion


        #region Methods
        public void Drop(Vector3 position)
        {
            this.LeftPressed = false;
            this.WorldPosition = position;
        }
        #endregion


        #region Unity Methods
        private void Start()
        {
            if (this.surface == null)
                this.surface = FindObjectOfType(typeof(DesktopSurface)) as DesktopSurface;

            if (this.surface == null)
                Debug.LogError("There MUST be a DesktopSurface for any DesktopDraggable.");
            else
            {
                this.surface.Draggables.Add(this);
                this.WorldPosition = this.surface.AdsordToSurface(this.WorldPosition);
            }
        }

        private void Update()
        {
            if (this.Active && this.LeftPressed) {
                var raycastResult = this.surface.MouseRaycast(false, true, gridSnap);

                if (raycastResult.hit) {
                    this.WorldPosition = Vector3.Lerp(this.WorldPosition, raycastResult.point + this.mouseOffset, 0.5f);
                }

                if (!this.moved && pressMousePosition != Mouse.current.position.ReadValue()) {
                    this.SendMessage("CancelClick", SendMessageOptions.DontRequireReceiver);
                    this.moved = true;
                }
            }
        }

        private void OnDestroy()
        {
            if (this.surface != null)
            {
                this.surface.Draggables.Remove(this);
                this.surface = null;
            }
        }
        #endregion
    }
}
