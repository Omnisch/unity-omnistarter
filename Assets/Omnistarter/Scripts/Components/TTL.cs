// author: Omnistudio
// version: 2026.03.13

using Omnis.Utils;
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
            this.Ease(
                value => OnLifeSpan?.Invoke(value),
                () => {
                    goodbyeCallback?.Invoke();
                    Destroy(gameObject);
                },
                Easing.Linear, lifeTime
            );
        }
        #endregion
    }
}
