// author: Omnistudio
// version: 2025.07.29

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
        [SerializeField] private DesktopSurface surface;
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
                        mouseOffset = (transform.position - mousePoint).xoz();
                        positionMouseDown = transform.position;
                    }
                }
                else if (!value && base.LeftPressed)
                {
                    if (MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: true, liftUp: false))
                    {
                        if (draggable)
                            transform.position = mousePoint + mouseOffset.xoz();

                        if (transform.position.xz() == positionMouseDown.xz())
                            pressCallback?.Invoke();
                    }
                }

                base.LeftPressed = value;
            }
        }
        #endregion

        #region Methods
        private bool MouseRayCastPointOnSurface(out Vector3 point, bool doClamp, bool liftUp)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (surface.Raycast(ray, out point, doClamp, liftUp))
                return true;

            point = Vector3.zero;
            return false;
        }

        public void Drop(Vector3 position)
        {
            LeftPressed = false;
            transform.position = position;
        }

        public void MoveAwayFrom(Collider other)
        {
            var otherParent = other.transform.parent;

            if (otherParent && otherParent.TryGetComponent<DesktopDraggable>(out _))
            {
                Vector3 dir = transform.position - otherParent.position;

                if (dir.sqrMagnitude.ApproxLoose(0f))
                    dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                else
                    dir = dir.normalized;

                transform.position += 0.2f * dir;
            }
        }
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            if (surface == null)
                surface = FindObjectOfType(typeof(DesktopSurface)) as DesktopSurface;

            if (surface == null)
                Debug.LogError("The surface of any draggable MUST be set.");
            else
                transform.position = surface.AdsordToSurface(transform.position);
        }

        private void Update()
        {
            if (!Interactable || !draggable)
                return;

            if (LeftPressed && MouseRayCastPointOnSurface(out Vector3 mousePoint, doClamp: false, liftUp: true))
                transform.position = Vector3.Lerp(transform.position, mousePoint + mouseOffset, 0.5f);
        }
        #endregion
    }
}
