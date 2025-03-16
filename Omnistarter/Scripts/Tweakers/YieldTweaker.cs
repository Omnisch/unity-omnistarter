// author: Omnistudio
// version: 2025.03.16

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis.Util
{
    public class YieldTweaker
    {
        #region Accumulation
        /// <summary>
        /// It takes <i>time</i> seconds to accumulate from 0 to 1.
        /// </summary>
        public static IEnumerator Linear(UnityAction<float> action, float time = 1f, bool fixedUpdate = false)
        {
            var life = 0f;
            while (life < 1f)
            {
                action?.Invoke(life);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It continuously lerps from 0 to 1 with <i>speed</i>.
        /// </summary>
        public static IEnumerator Lerp(UnityAction<float> action, float speed = 1f, bool fixedUpdate = false)
        {
            var life = 0f;
            while (1f - life > 0.01f)
            {
                action?.Invoke(life);
                life = Mathf.Lerp(life, 1f, speed * (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime));
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It imitates an elastic motion from 0 to 1.
        /// </summary>
        public static IEnumerator Elastic(UnityAction<float> action, float speed = 1f, float drag = 0.1f, bool fixedUpdate = false)
        {
            var amp = 1f;
            var life = 0f;
            var value = 0f;
            while (amp > 0.01f)
            {
                action?.Invoke(value);
                value = amp * Mathf.Sin(speed * life);
                amp = Mathf.Exp(-drag * life);
                Debug.Log(amp);
                life += fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It draws a sine wave from x=0.
        /// </summary>
        public static IEnumerator SineWave(UnityAction<float> action, float amplitude = 1f, float frequency = 1f, float cycleCount = 1f, bool fixedUpdate = false)
        {
            var life = 0f;
            var value = 0f;
            while (life < cycleCount / frequency)
            {
                action?.Invoke(value);
                value = amplitude * Mathf.Sin(frequency * life * 2f * Mathf.PI);
                life += fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(amplitude * Mathf.Sin(cycleCount * 2f * Mathf.PI));
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
