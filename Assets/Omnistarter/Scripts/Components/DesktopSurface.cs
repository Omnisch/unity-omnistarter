// author: Omnistudio
// version: 2025.08.27

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
        [SerializeField] private Transform lb;
        [SerializeField] private Transform rt;
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
        public bool Raycast(Ray ray, out Vector3 point, bool doClamp, bool liftUp)
        {
            Plane calcPlane = plane;

            if (liftUp)
                calcPlane.Translate(-liftUpHeight * inNormal);

            if (calcPlane.Raycast(ray, out float dist))
            {
                if (doClamp)
                    point = ray.GetPoint(dist).Clamp(lb.position, rt.position);
                else
                    point = ray.GetPoint(dist);
                return true;
            }
            point = Vector3.zero;
            return false;
        }

        public Vector3 AdsordToSurface(Vector3 position) => plane.ClosestPointOnPlane(position);

        public void MoveSelfWithAllDraggables(Vector3 delta)
        {
            foreach (var draggable in draggables)
            {
                if (draggable != null && draggable.transform.parent != this)
                    draggable.WorldPosition += delta;
            }
            transform.position += delta;
        }
        public void MoveSelfWithAllDraggablesTo(Vector3 dest) => MoveSelfWithAllDraggables(dest - transform.position);
        #endregion


        #region Unity Methods
        private void Awake()
        {
            plane = new(inNormal, transform.position + localInPoint);
        }
        #endregion
    }
}
