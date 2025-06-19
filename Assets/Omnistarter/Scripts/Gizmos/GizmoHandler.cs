// author: Omnistudio
// version: 2025.06.19

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Gizmos
{
    public partial class GizmoHandler : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private bool drawGizmos = true;
        #endregion

        #region Fields
        private List<KeyValuePair<GameObject, Action>> requests = new();
        #endregion

        #region Methods
        public void AddRequest(GameObject client, Action request)
            => requests.Add(new(client, request));
        public void RemoveRequests(GameObject client)
            => requests.RemoveAll(pair => pair.Key == client);
        public void RemoveRequests(GameObject client, Action request)
            => requests.RemoveAll(pair => pair.Equals(new KeyValuePair<GameObject, Action>(client, request)));
        #endregion

        #region Unity Methods
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            requests.RemoveAll(request =>
            {
                if (request.Key == null)
                    return true;

                request.Value?.Invoke();
                return false;
            });
        }
        #endregion
    }
}
