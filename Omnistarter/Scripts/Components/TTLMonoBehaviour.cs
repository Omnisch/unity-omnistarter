// author: Omnistudio
// version: 2025.03.16

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// After Start(), live <i>lifeTime</i> and then destroy gameObject.<br />
    /// Override <i>OnStart()</i> to initiate.<br />
    /// Use <i>OnLifeSpan</i> to set callbacks.
    /// </summary>
    public class TTLMonoBehaviour : MonoBehaviour
    {
        public float lifeTime = 1f;
        public TTLMonoBehaviour SetLifeTime(float value)
        {
            lifeTime = value;
            return this;
        }

        /// <summary>
        /// The float value would be 0 at the start, 1 at the end.
        /// </summary>
        public UnityAction<float> OnLifeSpan { private get; set; }

        #region Unity methods
        protected virtual void OnStart() { }
        protected void Start()
        {
            OnStart();
            StartCoroutine(Utils.YieldHelper.Ease((value) =>
            {
                OnLifeSpan?.Invoke(value);
                if (value == 1f) Destroy(gameObject);
            }, Utils.Easing.Linear, lifeTime));
        }
        #endregion
    }
}
