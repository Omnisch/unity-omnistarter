// author: Omnistudio
// version: 2024.12.15

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// After Start(), live <i>lifeTime</i> and then destroy gameObject.<br />
    /// Use <i>OnStart</i> to initiate.<br />
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
            StartCoroutine(YieldTweaker.Linear((value) =>
            {
                OnLifeSpan?.Invoke(value);
                if (value == 1f) Destroy(gameObject);
            }, lifeTime));
        }
        #endregion
    }
}
