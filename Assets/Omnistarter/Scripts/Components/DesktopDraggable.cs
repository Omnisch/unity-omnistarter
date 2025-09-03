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
        private Vector3 pressMousePosition;
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
                    if (this.MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: false, liftUp: false))
                    {
                        this.mouseOffset = this.WorldPosition - mousePoint;
                        this.pressMousePosition = Input.mousePosition;
                        this.moved = false;
                    }
                }
                else if (!value && base.LeftPressed)
                {
                    if (this.MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: true, liftUp: false))
                    {
                        if (this.Active)
                            this.WorldPosition = mousePoint + this.mouseOffset;
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
        private bool MouseRayCastPointOnSurface(out Vector3 point, bool doClamp, bool liftUp)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (this.surface.Raycast(ray, out point, doClamp, liftUp, this.gridSnap))
                return true;

            point = Vector3.zero;
            return false;
        }

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
            if (!this.Active)
                return;

            if (this.LeftPressed && this.MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: false, liftUp: true)) {
                this.WorldPosition = Vector3.Lerp(this.WorldPosition, mousePoint + this.mouseOffset, 0.5f);

                if (!this.moved && pressMousePosition != Input.mousePosition) {
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
