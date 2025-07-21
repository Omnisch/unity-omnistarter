using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    public class DesktopSlot : PointerBase
    {
        #region Serialized Fields
        [SerializeField] private Vector3 localSlotPoint = Vector3.zero;
        #endregion

        #region Fields
        [HideInInspector] public DesktopDraggable currGem;
        private Transform currGemLastParent;
        #endregion

        #region Methods
        protected override void OnInteracted(List<Collider> siblings)
        {
            // If left button been pressed and is about to release.
            if (LeftPressed)
            {
                foreach (Collider c in siblings)
                {
                    if (c.TryGetComponent<DesktopDraggable>(out var gem))
                    {
                        currGem = gem;

                        if (currGem.transform.parent == transform)
                        {
                            currGem.transform.parent = currGemLastParent;
                        }
                        else
                        {
                            currGemLastParent = currGem.transform.parent;
                            currGem.transform.parent = transform;
                            StartCoroutine(LateSetPosition(c.transform));
                        }
                    }
                }
            }
        }

        private IEnumerator LateSetPosition(Transform go)
        {
            yield return null;
            go.position = transform.position + localSlotPoint;
        }
        #endregion
    }
}
