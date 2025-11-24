// author: Omnistudio
// version: 2025.11.24

using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnis
{
    public class DesktopDraggable : PointerBase
    {
        #region Serialized Fields
        [SerializeField] protected DesktopSurface surface = null;
        [SerializeField] protected float gridSnap = 0f;
        #endregion


        #region Fields
        protected Vector3 mouseOffset;
        protected Vector2 pressMousePosition;
        protected bool moved;
        #endregion


        #region Properties
        public override bool LeftPressed {
            get => base.LeftPressed;
            protected set {
                if (value && !base.LeftPressed) {
                    var raycastResult = surface.MouseRaycast(false, false, gridSnap);
                    if (raycastResult.hit) {
                        mouseOffset = WorldPosition - raycastResult.point;
                        pressMousePosition = Mouse.current.position.ReadValue();
                        moved = false;
                    }
                } else if (!value && base.LeftPressed) {
                    var raycastResult = surface.MouseRaycast(true, false, gridSnap);
                    if (Active && raycastResult.hit) {
                        if (raycastResult.clamped) {
                            WorldPosition = raycastResult.point;
                        } else {
                            WorldPosition = raycastResult.point + mouseOffset;
                        }
                    }
                }

                base.LeftPressed = value;
            }
        }

        public Vector3 WorldPosition {
            get => transform.position;
            set {
                if (TryGetComponent<DesktopSurface>(out var selfSurface)) {
                    selfSurface.MoveSelfWithAllDraggablesTo(value);
                } else {
                    transform.position = value;
                }
            }
        }
        #endregion


        #region Methods
        public void Drop(Vector3 position) {
            LeftPressed = false;
            WorldPosition = position;
        }
        #endregion


        #region Unity Methods
        protected virtual void Start() {
            if (surface == null) {
                surface = FindObjectOfType(typeof(DesktopSurface)) as DesktopSurface;
            }
            if (surface == null) {
                Debug.LogError("There MUST be a DesktopSurface for any DesktopDraggable.");
            } else {
                surface.Draggables.Add(this);
                WorldPosition = surface.AdsordToSurface(WorldPosition);
            }
        }

        protected virtual void Update() {
            if (Active && LeftPressed) {
                var raycastResult = surface.MouseRaycast(false, true, gridSnap);

                if (raycastResult.hit) {
                    WorldPosition = Vector3.Lerp(WorldPosition, raycastResult.point + mouseOffset, 0.5f);
                }

                if (!moved && pressMousePosition != Mouse.current.position.ReadValue()) {
                    SendMessage("CancelClick", SendMessageOptions.DontRequireReceiver);
                    moved = true;
                }
            }
        }

        private void OnDestroy() {
            if (surface != null) {
                surface.Draggables.Remove(this);
                surface = null;
            }
        }
        #endregion
    }
}
