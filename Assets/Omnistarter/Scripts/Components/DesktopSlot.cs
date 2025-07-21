// author: Omnistudio
// version: 2025.07.18

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    public class DesktopSlot : PointerBase
    {
        #region Serialized Fields
        [SerializeField] private Vector3 localSlotPoint = Vector3.zero;
        [SerializeField] private UnityEvent callback;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnInteracted(List<Collider> siblings)
        {
            base.OnInteracted(siblings);
            foreach (Collider c in siblings)
            {
                if (c.TryGetComponent<DesktopDraggable>(out var draggable))
                {
                    draggable.Drop(transform.localPosition + localSlotPoint);
                    callback?.Invoke();
                }
            }
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
