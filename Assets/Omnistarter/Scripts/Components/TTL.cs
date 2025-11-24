// author: Omnistudio
// version: 2025.11.24

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// After Start(), live <i>lifeTime</i> and then destroy gameObject.<br />
    /// Set callbacks by <i>OnLifeSpan</i>.
    /// </summary>
    [DisallowMultipleComponent]
    public class TTL : MonoBehaviour
    {
        public float lifeTime = 1f;
        public TTL SetLifeTime(float value) {
            lifeTime = value;
            return this;
        }

        /// <summary>The float value would be 0 at the start, 1 at the end.</summary>
        public UnityAction<float> OnLifeSpan { private get; set; }
        public UnityEvent goodbyeCallback;

        #region Unity methods
        private void Start() {
            StartCoroutine(Utils.YieldHelper.Ease((value) => {
                OnLifeSpan?.Invoke(value);
                if (value == 1f) {
                    goodbyeCallback?.Invoke();
                    Destroy(gameObject);
                }
            }, Utils.Easing.Linear, lifeTime));
        }
        #endregion
    }
}
