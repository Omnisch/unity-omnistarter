// author: Omnistudio
// version: 2025.03.17

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis.Utils
{
    public static class YieldHelper
    {
        #region Accumulations
        /// <summary>
        /// It takes <i>time</i> seconds to ease from 0 to 1, where <i>easingFunc</i> determines the curve.
        /// </summary>
        public static IEnumerator Ease(UnityAction<float> action, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
        {
            time = Mathf.Max(time, float.Epsilon);
            float life = 0f;
            float value = 0f;
            while (life < 1f)
            {
                action?.Invoke(value);
                value = easingFunc(life);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(1f);
        }

        /// <summary>
        /// It takes <i>time</i> seconds to ease from 0 to 1, repeat <i>cycleCount</i> times.
        /// </summary>
        /// <param name="pingPong">If true, it will use Mathf.PingPong() rather than Mathf.Repeat().</param>
        /// <param name="dampened">If true, it applys linear decay to the scale.</param>
        public static IEnumerator EaseRepeat(UnityAction<float> action, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
        {
            time = Mathf.Max(time, float.Epsilon);
            float life = 0f;
            float value = 0f;
            float scale = 1f;
            while (life < cycleCount)
            {
                action?.Invoke(dampened ? scale * value : value);
                value = easingFunc(pingPong ? life.PingPong(1f) : life.Repeat(1f));
                if (dampened) scale = Mathf.Lerp(1f, 0f, life / cycleCount);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(easingFunc(cycleCount.Repeat(1f)));
        }

        /// <summary>
        /// It performs Mathf.SmoothDamp().
        /// </summary>
        public static IEnumerator SmoothDamp(UnityAction<float> action, float time = 1f, bool fixedUpdate = false)
        {
            time = Mathf.Max(time, float.Epsilon);
            float value = 0f;
            float velocity = 0f;
            float smoothTime = 0.3f * time;
            while (!value.ApproxLoose(1f))
            {
                action?.Invoke(value);
                value = Mathf.SmoothDamp(value, 1f, ref velocity, smoothTime);
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(1f);
        }
        #endregion

        #region Loops
        /// <summary>
        /// It invokes <i>actions</i> sequentially every <i>interval</i> seconds for <i>i</i> times.<br/>
        /// If <i>interval</i> &lt; 0, it invokes <i>action</i> every frame.<br/>
        /// If <i>i</i> less than 1, it won't stop.
        /// </summary>
        public static IEnumerator Loop(int i = 3, float interval = 1f, params UnityAction[] actions)
        {
            var waitForIntervalSeconds = new WaitForSeconds(interval);
            int it = 0;
            if (i <= 0)
                while (true)
                {
                    actions[it]?.Invoke();
                    it = (it + 1) % actions.Length;
                    yield return waitForIntervalSeconds;
                }
            else
                while (i > 0)
                {
                    actions[it]?.Invoke();
                    it = (it + 1) % actions.Length;
                    if (it == 0) i--;
                    yield return waitForIntervalSeconds;
                }
        }

        /// <summary>
        /// It starts coroutine <i>action</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.<br/>
        /// If <i>i</i> less than 1, it won't stop.
        /// </summary>
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
        public static IEnumerator DoWhen(Func<bool> condition, UnityAction action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }

        /// <summary>
        /// Do <i>coroutines</i> sequentially.
        /// </summary>
        public static IEnumerator DoSequentially(params Func<IEnumerator>[] coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                yield return coroutine();
            }
        }
        #endregion
    }
}
