// author: Omnistudio
// version: 2025.09.02

using Omnis.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Omnis
{
    public class DesktopDraggable : PointerBase
    {
        #region Serialized Fields
        [SerializeField] private bool draggable = true;
        [SerializeField] private DesktopSurface surface = null;
        [SerializeField] private float gridSnap = 0f;
        [SerializeField] private UnityEvent pressCallback;
        #endregion


        #region Fields
        private Vector3 mouseOffset;
        private Vector3 positionMouseDown;
        #endregion


        #region Properties
        public override bool LeftPressed
        {
            get => base.LeftPressed;
            protected set
            {
                if (value && !base.LeftPressed)
                {
                    if (MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: false, liftUp: false))
                    {
                        mouseOffset = WorldPosition - mousePoint;
                        positionMouseDown = WorldPosition;
                    }
                }
                else if (!value && base.LeftPressed)
                {
                    if (MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: true, liftUp: false))
                    {
                        if (draggable)
                            WorldPosition = mousePoint + mouseOffset;

                        if (WorldPosition.xz() == positionMouseDown.xz())
                            pressCallback?.Invoke();
                    }
                }

                base.LeftPressed = value;
            }
        }

        public Vector3 WorldPosition
        {
            get => transform.position;
            set
            {
                if (TryGetComponent<DesktopSurface>(out var selfSurface))
                    selfSurface.MoveSelfWithAllDraggablesTo(value);
                else
                    transform.position = value;
            }
        }
        #endregion


        #region Methods
        private bool MouseRayCastPointOnSurface(out Vector3 point, bool doClamp, bool liftUp)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (surface.Raycast(ray, out point, doClamp, liftUp, gridSnap))
                return true;

            point = Vector3.zero;
            return false;
        }

        public void Drop(Vector3 position)
        {
            LeftPressed = false;
            WorldPosition = position;
        }
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            if (surface == null)
                surface = FindObjectOfType(typeof(DesktopSurface)) as DesktopSurface;

            if (surface == null)
                Debug.LogError("There MUST be a DesktopSurface for any DesktopDraggable.");
            else
            {
                surface.Draggables.Add(this);
                WorldPosition = surface.AdsordToSurface(WorldPosition);
            }
        }

        private void Update()
        {
            if (!Interactable || !draggable)
                return;

            if (LeftPressed && MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: false, liftUp: true)) {
                WorldPosition = Vector3.Lerp(WorldPosition, mousePoint + mouseOffset, 0.5f);
            }
        }

        private void OnDestroy()
        {
            if (surface != null)
            {
                surface.Draggables.Remove(this);
                surface = null;
            }
        }
        #endregion
    }
}
