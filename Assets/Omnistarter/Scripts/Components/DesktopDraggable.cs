// author: Omnistudio
// version: 2025.07.21

using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnis
{
    public class DesktopDraggable : PointerBase
    {
        #region Serialized Fields
        [SerializeField] private DesktopSurface surface;
        #endregion

        #region Fields
        private Vector3 mouseOffset;
        #endregion

        #region Properties
        public override bool LeftPressed
        {
            get => base.LeftPressed;
            protected set
            {
                base.LeftPressed = value;
                Vector3 mousePoint;
                if (value)
                {
                    if (MouseRayCastPointOnSurface(out mousePoint, false))
                        mouseOffset = transform.position - mousePoint;
                }
                else 
                {
                    if (MouseRayCastPointOnSurface(out mousePoint, true))
                        transform.position = mousePoint + mouseOffset;
                }
            }
        }
        #endregion

        #region Methods
        private bool MouseRayCastPointOnSurface(out Vector3 point, bool doClamp)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (surface.Raycast(ray, out point, doClamp))
                return true;

            point = Vector3.zero;
            return false;
        }

        public void Drop(Vector3 position)
        {
            LeftPressed = false;
            transform.position = position;
        }
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();
            // Opaque apperantly should always be false,
            // so that ray can go through to surface underneath.
            opaque = false;
            if (surface == null)
                Debug.LogError("The surface of any draggable MUST be set.");
        }

        private void Update()
        {
            if (Interactable && LeftPressed)
            {
                if (MouseRayCastPointOnSurface(out Vector3 mousePoint, false))
                    transform.position = mousePoint + mouseOffset;
            }
        }
        #endregion
    }
}
