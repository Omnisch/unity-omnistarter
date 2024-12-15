// author: Omnistudio
// version: 2024.11.01

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// After Start(), live <i>lifeTime</i> and then destroy gameObject.<br />
    /// Use <i>OnLifeSpan</i> to set callbacks.
    /// </summary>
    public class TTLMonoBehaviour : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private float lifeTime = 1f;
        #endregion

        #region Fields
        private float life;
        private float Life
        {
            get => life;
            set
            {
                life = Mathf.Clamp01(value);
                OnLifeSpan.Invoke(value);
            }
        }
        #endregion

        #region Interfaces
        public TTLMonoBehaviour SetLifeTime(float value)
        {
            lifeTime = value;
            return this;
        }
        /// <summary>
        /// The float value would be 1 at the start, 0 at the end.
        /// </summary>
        public UnityAction<float> OnLifeSpan {  private get; set; }
        #endregion

        #region Life Span
        protected virtual void OnStart() { }
        private void Start()
        {
            life = 1f;
            OnStart();
            StartCoroutine(LifeSpan());
        }

        private IEnumerator LifeSpan()
        {
            var lifeTimeFixed = lifeTime;
            while (Life > 0f)
            {
                Life -= Time.deltaTime / lifeTimeFixed;
                yield return 0;
            }
            Destroy(gameObject);
        }
        #endregion
    }
}
