// author: Omnistudio
// version: 2025.03.15

using System.Linq;
using UnityEngine;

namespace Omnis.Util
{
    [RequireComponent(typeof(Rigidbody))]
    public class ChildrenTremble : MonoBehaviour
    {
        #region Interfaces
        public void Stroke(float force = 100f) => Stroke(Vector3.one, force);
        public void Stroke(Vector3 direction, float force = 100f)
        {
            transform.GetComponentsInChildren<Rigidbody>().ToList().ForEach(child => child.AddForce(force * direction.normalized));
        }
        #endregion
    }
}
