// author: Omnistudio
// version: 2025.03.16

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis.Utils
{
    public static class YieldTweaker
    {
        #region Accumulations
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

        #region Loops
        /// <summary>
        /// It invokes <i>action</i> every <i>interval</i> seconds for <i>i</i> times.<br/>
        /// If <i>interval</i> leq to 0, it invokes <i>action</i> every frame.<br/>
        /// If <i>i</i> less than 1, it won't stop.
        /// </summary>
        public static IEnumerator Loop(UnityAction action, float interval = 0f, int i = 0)
        {
            if (action == null) yield break;
            var waitForIntervalSeconds = new WaitForSeconds(interval);
            if (i <= 0)
                while (true)
                {
                    action.Invoke();
                    yield return waitForIntervalSeconds;
                }
            else
                while (i > 0)
                {
                    action.Invoke();
                    i--;
                    yield return waitForIntervalSeconds;
                }
        }

        public static IEnumerator Loop(Func<IEnumerator> action, float interval = 0f, int i = 0)
        {
            if (action == null) yield break;
            var waitForIntervalSeconds = new WaitForSeconds(interval);
            if (i <= 0)
                while (true)
                {
                    yield return action();
                    yield return waitForIntervalSeconds;
                }
            else
                while (i > 0)
                {
                    yield return action();
                    i--;
                    yield return waitForIntervalSeconds;
                }
        }
        #endregion

        #region Wait until
        /// <summary>
        /// Do <i>action</i> when <i>condition</i> becomes true.
        /// </summary>
        public static IEnumerator DoWhen(System.Func<bool> condition, UnityAction action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }
        #endregion
    }
}
