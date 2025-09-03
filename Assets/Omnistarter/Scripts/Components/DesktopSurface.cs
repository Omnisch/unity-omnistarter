// author: Omnistudio
// version: 2025.09.03

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField][Editor.InspectorReadOnly] private List<DesktopDraggable> draggables = new();
        [SerializeField][Editor.InspectorReadOnly] private List<DesktopSlot> slots = new();
        public List<DesktopDraggable> Draggables => draggables;
        public List<DesktopSlot > Slots => slots;
        private Plane plane;
        #endregion


        #region Methods
        public bool Raycast(Ray ray, out Vector3 point, bool doClamp = false, bool liftUp = false, float gridSnap = 0f)
        {
            if (this.plane.Raycast(ray, out float dist)) {
                point = ray.GetPoint(dist);

                if (doClamp) {
                    OrientedBox obb = OrientedBox.FromDiagonalAndRef(this.diagonalFrom.position, this.diagonalTo.position, this.vertexRef.position, this.inNormal);
                    point = obb.Clamp(point);
                }

                point = point.GridSnap(gridSnap);

                if (liftUp) {
                    Plane calcPlane = this.plane;
                    calcPlane.Translate(-this.liftUpHeight * this.inNormal);

                    Ray rayRevert = new(point, (ray.origin - point).normalized);
                    if (calcPlane.Raycast(rayRevert, out float distLiftUp)) {
                        point = rayRevert.GetPoint(distLiftUp);
                    }
                }

                return true;
            }
            else {
                point = Vector3.zero;
                return false;
            }
        }

        public Vector3 AdsordToSurface(Vector3 position) => this.plane.ClosestPointOnPlane(position);

        public void MoveSelfWithAllDraggables(Vector3 delta)
        {
            foreach (var draggable in this.draggables)
            {
                if (draggable != null && draggable.transform.parent != this)
                    draggable.WorldPosition += delta;
            }
            this.transform.position += delta;
        }
        public void MoveSelfWithAllDraggablesTo(Vector3 dest) => MoveSelfWithAllDraggables(dest - this.transform.position);
        #endregion


        #region Unity Methods
        private void Awake()
        {
            this.plane = new(this.inNormal, this.transform.position + this.localInPoint);
        }
        #endregion
    }
}
