// author: Omnistudio
// version: 2025.03.30

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis.Gizmos
{
    public partial class GizmoHandler : MonoBehaviour
    {
        #region Fields
        private List<KeyValuePair<GameObject, UnityAction>> requests = new();
        #endregion

        #region Methods
        public void AddRequest(GameObject client, UnityAction request)
            => requests.Add(new(client, request));
        public void RemoveRequests(GameObject client)
            => requests.RemoveAll(pair => pair.Key == client);
        public void RemoveRequests(GameObject client, UnityAction request)
            => requests.RemoveAll(pair => pair.Equals(new KeyValuePair<GameObject, UnityAction>(client, request)));
        #endregion

        #region Unity Methods
        private void OnDrawGizmos()
        {
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
