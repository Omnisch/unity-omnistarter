// author: Omnistudio
// version: 2026.03.11

using Omnis.Utils;
using OmnisEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnis
{
    public class DesktopSurface : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Vector3 localInPoint = Vector3.zero;
        [SerializeField] private Vector3 inNormal = Vector3.up;
        [SerializeField] private Transform diagonalFrom;
        [SerializeField] private Transform diagonalTo;
        [SerializeField] private Transform vertexRef;
        [SerializeField] private float liftUpHeight = 0.2f;
        #endregion


        #region Fields
        [SerializeField][InspectorReadOnly] private List<DesktopDraggable> draggables = new();
        [SerializeField][InspectorReadOnly] private List<DesktopSlot> slots = new();
        public List<DesktopDraggable> Draggables => draggables;
        public List<DesktopSlot> Slots => slots;
        private Plane plane;
        #endregion


        #region Methods
        public RaycastResult MouseRaycast(bool doClamp = false, bool liftUp = false, float gridSnap = 0f) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Raycast(ray, doClamp, liftUp, gridSnap);
        }

        public RaycastResult Raycast(Ray ray, bool doClamp = false, bool liftUp = false, float gridSnap = 0f) {
            if (plane.Raycast(ray, out float dist)) {
                Vector3 point = ray.GetPoint(dist);

                bool clamped = false;
                if (doClamp) {
                    OrientedBox obb = OrientedBox.FromDiagonalAndRef(diagonalFrom.position, diagonalTo.position, vertexRef.position, inNormal);
                    var tmp = point;
                    point = obb.Clamp(point);
                    if (point != tmp) {
                        clamped = true;
                    }
                }

                point = point.GridSnap(gridSnap);

                if (liftUp) {
                    Plane calcPlane = plane;
                    calcPlane.Translate(-liftUpHeight * inNormal);

                    Ray rayRevert = new(point, (ray.origin - point).normalized);
                    if (calcPlane.Raycast(rayRevert, out float distLiftUp)) {
                        point = rayRevert.GetPoint(distLiftUp);
                    }
                }

                return new RaycastResult {
                    hit = true,
                    point = point,
                    clamped = clamped
                };
            } else {
                return new RaycastResult {
                    hit = false,
                    point = Vector3.zero,
                    clamped = false
                };
            }
        }

        public Vector3 AdsordToSurface(Vector3 position) => plane.ClosestPointOnPlane(position);

        public void MoveSelfWithAllDraggables(Vector3 delta) {
            foreach (var draggable in draggables) {
                if (draggable != null && draggable.transform.parent != this)
                    draggable.WorldPosition += delta;
            }
            transform.position += delta;
        }
        public void MoveSelfWithAllDraggablesTo(Vector3 dest) => MoveSelfWithAllDraggables(dest - transform.position);
        #endregion


        #region Unity Methods
        private void Awake() {
            plane = new(inNormal, transform.position + localInPoint);
        }
        #endregion


        #region Structs
        public struct RaycastResult
        {
            public bool hit;
            public Vector3 point;
            public bool clamped;
        }
        #endregion
    }
}
