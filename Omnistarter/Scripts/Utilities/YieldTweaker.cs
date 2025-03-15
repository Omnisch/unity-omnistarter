// author: Omnistudio
// version: 2025.03.15

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis.Util
{
    public class YieldTweaker
    {
        #region Linear Accumulation
        /// <summary>
        /// It takes <i>time</i> seconds to accumulate from 0 to 1.
        /// </summary>
        public static IEnumerator Linear(UnityAction<float> action, float time = 1f)
        {
            var life = 0f;
            while (life < 1f)
            {
                action?.Invoke(life);
                life += Time.deltaTime / time;
                yield return null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It takes <i>time</i> seconds to accumulate from 0 to 1.
        /// </summary>
        public static IEnumerator LinearFixed(UnityAction<float> action, float time = 1f)
        {
            var life = 0f;
            while (life < 1f)
            {
                action?.Invoke(life);
                life += Time.fixedDeltaTime / time;
                yield return new WaitForFixedUpdate();
            }
            action?.Invoke(1f);
        }
        #endregion

        #region Lerp Accumulation
        /// <summary>
        /// It continuously lerps from 0 to 1 with <i>speed</i>.
        /// </summary>
        public static IEnumerator Lerp(UnityAction<float> action, float speed = 1f)
        {
            var life = 0f;
            while (1f - life > 0.01f)
            {
                action?.Invoke(life);
                life = Mathf.Lerp(life, 1f, speed * Time.deltaTime);
                yield return null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It continuously lerps from 0 to 1 with <i>speed</i>.
        /// </summary>
        public static IEnumerator LerpFixed(UnityAction<float> action, float speed = 1f)
        {
            var life = 0f;
            while (1f - life > 0.01f)
            {
                action?.Invoke(life);
                life = Mathf.Lerp(life, 1f, speed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            action?.Invoke(1f);
        }
        #endregion

        #region Infinite Loop
        /// <summary>
        /// It invokes <i>action</i> every frame and won't stop by itself.
        /// </summary>
        public static IEnumerator InfiniteLoop(UnityAction action)
        {
            if (action == null) yield break;
            while (true)
            {
                action.Invoke();
                yield return null;
            }
        }

        /// <summary>
        /// It invokes <i>action</i> every <i>interval</i> seconds and won't stop by itself.
        /// </summary>
        public static IEnumerator InfiniteLoop(UnityAction action, float interval)
        {
            if (action == null) yield break;
            var wfsInterval = new WaitForSeconds(interval);
            while (true)
            {
                action.Invoke();
                yield return wfsInterval;
            }
        }

        /// <summary>
        /// It invokes <i>action</i> every fixed update and won't stop by itself.
        /// </summary>
        public static IEnumerator InfiniteLoopFixed(UnityAction action)
        {
            if (action == null) yield break;
            while (true)
            {
                action.Invoke();
                yield return new WaitForFixedUpdate();
            }
        }
        #endregion

        #region Wait Until
        /// <summary>
        /// Do <i>action</i> until <i>condition</i> becomes true.
        /// </summary>
        public static IEnumerator DoAfter(System.Func<bool> condition, UnityAction action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }
        #endregion
    }
}
