// author: Omnistudio
// version: 2025.03.21

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
            => EaseRepeat(action, easingFunc, time, 1f, false, false, fixedUpdate);

        /// <summary>
        /// It takes <i>time</i> seconds to ease from 0 to 1, repeat <i>cycleCount</i> times.
        /// </summary>
        /// <param name="cycleCount">If less than 1, it won't stop.</param>
        /// <param name="pingPong">If true, it will use Mathf.PingPong() rather than Mathf.Repeat().</param>
        /// <param name="dampened">If true, it applys linear decay to the scale.</param>
        public static IEnumerator EaseRepeat(UnityAction<float> action, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
        {
            time = Mathf.Max(time, float.Epsilon);
            float life = 0f;
            float value = 0f;
            float scale = 1f;
            while (life < cycleCount || cycleCount < 1)
            {
                action?.Invoke(dampened ? scale * value : value);
                value = easingFunc(pingPong ? life.PingPong(1f) : life.Repeat(1f));
                if (dampened) scale = Mathf.Lerp(1f, 0f, life / cycleCount);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action?.Invoke(easingFunc(pingPong ? cycleCount.PingPong(1f) : cycleCount.RepeatCeil(1f)));
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

        #region Sequences
        /// <summary>
        /// It sequentially invokes <i>actions</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.
        /// </summary>
        /// <param name="i">If less than 1, it won't stop.</param>
        /// <param name="interval">If <i>interval</i> &lt; 0, it invokes <i>action</i> every frame.</param>
        public static IEnumerator Loop(int i = 3, float interval = 1f, params UnityAction[] actions)
        {
            var waitForIntervalSeconds = new WaitForSeconds(interval);
            int it = 0;
            do {
                actions[it]?.Invoke();
                it = (it + 1) % actions.Length;
                if (it == 0) i--;
                yield return waitForIntervalSeconds;
            } while (i != 0 || it != 0);
        }

        /// <summary>
        /// It sequentially starts <i>iEnums</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.
        /// </summary>
        /// <param name="i">If less than 1, it won't stop.</param>
        public static IEnumerator Loop(int i = 3, float interval = 1f, params Func<IEnumerator>[] iEnums)
        {
            var waitForIntervalSeconds = new WaitForSeconds(interval);
            int it = 0;
            do {
                yield return iEnums[it]();
                it = (it + 1) % iEnums.Length;
                if (it == 0) i--;
                yield return waitForIntervalSeconds;
            } while (i != 0 || it != 0);
        }

        /// <summary>
        /// It starts <i>iEnums</i> sequentially. It's the same with <i>Loop(1, 0, iEnums)</i>.
        /// </summary>
        public static IEnumerator DoSequence(params Func<IEnumerator>[] iEnums) => Loop(1, 0f, iEnums);

        /// <summary>
        /// It invokes <i>action</i> when <i>condition</i> becomes true.
        /// </summary>
        public static IEnumerator DoWhen(Func<bool> condition, UnityAction action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }
        #endregion
    }
}
