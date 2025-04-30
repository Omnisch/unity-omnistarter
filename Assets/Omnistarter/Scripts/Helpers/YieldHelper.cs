// author: Omnistudio
// version: 2025.04.30

using System;
using System.Collections;
using UnityEngine;

namespace Omnis.Utils
{
    public static class YieldHelper
    {
        #region Accumulations
        /// <summary>
        /// <inheritdoc cref="Ease(Action{float}, UnityAction, Func{float, float}, float, bool)"/>
        /// </summary>
        public static IEnumerator Ease(Action<float> action, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => EaseRepeat(action, null, easingFunc, time, 1f, false, false, fixedUpdate);

        /// <summary>
        /// It takes <i>time</i> seconds to ease from 0 to 1, where <i>easingFunc</i> determines the curve.
        /// </summary>
        public static IEnumerator Ease(Action<float> action, Action final, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => EaseRepeat(action, final, easingFunc, time, 1f, false, false, fixedUpdate);

        /// <summary>
        /// <inheritdoc cref="EaseRepeat(Action{float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        /// </summary>
        public static IEnumerator EaseRepeat(Action<float> action, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
            => EaseRepeat(action, null, easingFunc, time, cycleCount, pingPong, dampened, fixedUpdate);

        /// <summary>
        /// It takes <i>time</i> seconds to ease from 0 to 1, repeat <i>cycleCount</i> times.
        /// </summary>
        /// <param name="cycleCount">If less than 1, it won't stop.</param>
        /// <param name="pingPong">If true, it will use Mathf.PingPong() rather than Mathf.Repeat().</param>
        /// <param name="dampened">If true, it applys linear decay to the scale.</param>
        public static IEnumerator EaseRepeat(Action<float> action, Action final, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
        {
            if (action == null) yield break;

            time = Mathf.Max(time, float.Epsilon);
            float life = 0f;
            float value = 0f;
            float scale = 1f;
            while (life < cycleCount || cycleCount < 1)
            {
                action.Invoke(dampened ? scale * value : value);
                value = easingFunc(pingPong ? life.PingPong(1f) : life.Repeat(1f));
                if (dampened) scale = Mathf.Lerp(1f, 0f, life / cycleCount);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action.Invoke(easingFunc(pingPong ? cycleCount.PingPong(1f) : cycleCount.RepeatCeil(1f)));
            final?.Invoke();
        }

        /// <summary>
        /// It performs Mathf.SmoothDamp().
        /// </summary>
        public static IEnumerator SmoothDamp(Action<float> action, float time = 1f, bool fixedUpdate = false)
        {
            if (action == null) yield break;

            time = Mathf.Max(time, float.Epsilon);
            float value = 0f;
            float velocity = 0f;
            float smoothTime = 0.3f * time;
            while (!value.ApproxLoose(1f))
            {
                action.Invoke(value);
                value = Mathf.SmoothDamp(value, 1f, ref velocity, smoothTime);
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action.Invoke(1f);
        }
        #endregion

        #region Delta time
        /// <summary>
        /// <inheritdoc cref="GiveDeltaTime(UnityAction{float}, UnityAction, float, int, bool)"/>
        /// </summary>
        public static IEnumerator GiveDeltaTime(Action<float> action, float time = 1f, int frameInterval = 1, bool fixedUpdate = false)
            => GiveDeltaTime(action, null, time, frameInterval, fixedUpdate);

        /// <summary>
        /// It gives the delta time through <i>frameInterval</i> frames back to <i>action</i>.<br/>
        /// NOTE: Floating-point errors may occur.
        /// </summary>
        public static IEnumerator GiveDeltaTime(Action<float> action, Action final = null, float time = 1f, int frameInterval = 1, bool fixedUpdate = false)
        {
            if (action == null) yield break;

            frameInterval = Mathf.Max(frameInterval, 1);
            float life = 0f;
            float period = 0f;
            int periodFrame = 0;
            if (fixedUpdate)
                while (life < time)
                {
                    if (periodFrame == frameInterval)
                    {
                        action.Invoke(period);
                        period = 0f;
                        periodFrame = 0;
                    }
                    periodFrame++;
                    period += Time.fixedDeltaTime;
                    life += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            else
                while (life < time)
                {
                    if (periodFrame == frameInterval)
                    {
                        action.Invoke(period);
                        period = 0f;
                        periodFrame = 0;
                    }
                    periodFrame++;
                    period += Time.deltaTime;
                    life += Time.deltaTime;
                    yield return null;
                }
            action.Invoke(time + period - life);
            final?.Invoke();
        }
        #endregion

        #region Sequences
        /// <summary>
        /// It sequentially invokes <i>actions</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.
        /// </summary>
        /// <param name="i">If less than 1, it won't stop.</param>
        /// <param name="interval">If <i>interval</i> &lt; 0, it invokes <i>action</i> every frame.</param>
        public static IEnumerator Loop(int i = 3, float interval = 1f, params Action[] actions)
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
        /// It sequentially invokes <i>actions</i>, waiting <i>interval</i> seconds in between.<br/>
        /// It's the same with <i>Loop(1, interval, actions)</i>.
        /// </summary>
        public static IEnumerator DoSequence(float interval, params Action[] actions) => Loop(1, interval, actions);
        /// <summary>
        /// It starts <i>iEnums</i> sequentially.<br/>
        /// It's the same with <i>Loop(1, 0, iEnums)</i>.
        /// </summary>
        public static IEnumerator DoSequence(params Func<IEnumerator>[] iEnums) => Loop(1, 0f, iEnums);

        /// <summary>
        /// It invokes <i>action</i> when <i>condition</i> becomes true.
        /// </summary>
        public static IEnumerator DoWhen(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }
        #endregion
    }
}
