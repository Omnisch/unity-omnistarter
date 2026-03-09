// author: Omnistudio
// version: 2026.03.09

using System;
using System.Collections;
using UnityEngine;

namespace Omnis.Utils
{
    public static class YieldHelper
    {
        #region Accumulations
        /// <summary>It takes <i>time</i> seconds to ease from 0 to 1, where <i>easingFunc</i> determines the curve.</summary>
        public static Coroutine Ease(this MonoBehaviour mono, Action<float> action, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => mono.EaseRepeat(action, null, easingFunc, time, 1f, false, false, fixedUpdate);


        /// <inheritdoc cref="Ease(MonoBehaviour, Action{float}, Func{float, float}, float, bool)"/>
        /// <param name="action">Action&lt;value, delta&gt;: The second param is delta value of easingFunc.</param>
        public static Coroutine Ease(this MonoBehaviour mono, Action<float, float> action, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => mono.EaseRepeat(action, null, easingFunc, time, 1f, false, false, fixedUpdate);


        /// <inheritdoc cref="Ease(MonoBehaviour, Action{float}, Func{float, float}, float, bool)"/>
        public static Coroutine Ease(this MonoBehaviour mono, Action<float> action, Action final, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => mono.EaseRepeat(action, final, easingFunc, time, 1f, false, false, fixedUpdate);


        /// <inheritdoc cref="Ease(MonoBehaviour, Action{float}, Func{float, float}, float, bool)"/>
        /// <param name="action">Action&lt;value, delta&gt;: The second param is delta value of easingFunc.</param>
        public static Coroutine Ease(this MonoBehaviour mono, Action<float, float> action, Action final, Func<float, float> easingFunc, float time = 1f, bool fixedUpdate = false)
            => mono.EaseRepeat(action, final, easingFunc, time, 1f, false, false, fixedUpdate);


        /// <inheritdoc cref="EEaseRepeat(Action{float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        public static Coroutine EaseRepeat(this MonoBehaviour mono, Action<float> action, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
            => mono.EaseRepeat(action, null, easingFunc, time, cycleCount, pingPong, dampened, fixedUpdate);


        /// <inheritdoc cref="EEaseRepeat(Action{float, float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        public static Coroutine EaseRepeat(this MonoBehaviour mono, Action<float, float> action, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
            => mono.EaseRepeat(action, null, easingFunc, time, cycleCount, pingPong, dampened, fixedUpdate);


        /// <inheritdoc cref="EEaseRepeat(Action{float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        public static Coroutine EaseRepeat(this MonoBehaviour mono, Action<float> action, Action final, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
            => mono.StartCoroutine(EEaseRepeat(action, final, easingFunc, time, cycleCount, pingPong, dampened, fixedUpdate));


        /// <inheritdoc cref="EEaseRepeat(Action{float, float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        public static Coroutine EaseRepeat(this MonoBehaviour mono, Action<float, float> action, Action final, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
            => mono.StartCoroutine(EEaseRepeat(action, final, easingFunc, time, cycleCount, pingPong, dampened, fixedUpdate));



        /// <summary>It takes <i>time</i> seconds to ease from 0 to 1, repeat <i>cycleCount</i> times.</summary>
        /// <param name="cycleCount">If less than 1, it won't stop.</param>
        /// <param name="pingPong">If true, it will use Mathf.PingPong() rather than Mathf.Repeat().</param>
        /// <param name="dampened">If true, it applys linear decay to the scale.</param>
        public static IEnumerator EEaseRepeat(Action<float> action, Action final, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
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


        /// <inheritdoc cref="EEaseRepeat(Action{float}, Action, Func{float, float}, float, float, bool, bool, bool)"/>
        /// <param name="action">Action&lt;value, delta&gt;: The second param is delta value of easingFunc.</param>
        public static IEnumerator EEaseRepeat(Action<float, float> action, Action final, Func<float, float> easingFunc, float time = 1f, float cycleCount = 3f, bool pingPong = false, bool dampened = false, bool fixedUpdate = false)
        {
            if (action == null) yield break;

            time = Mathf.Max(time, float.Epsilon);
            float life = 0f;
            float value = 0f;
            float lastValue = 0f;
            float scale = 1f;
            while (life < cycleCount || cycleCount < 1)
            {
                action.Invoke(dampened ? scale * value : value, value - lastValue);
                lastValue = value;
                value = easingFunc(pingPong ? life.PingPong(1f) : life.Repeat(1f));
                if (dampened) scale = Mathf.Lerp(1f, 0f, life / cycleCount);
                life += (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime) / time;
                yield return fixedUpdate ? new WaitForFixedUpdate() : null;
            }
            action.Invoke(easingFunc(pingPong ? cycleCount.PingPong(1f) : cycleCount.RepeatCeil(1f)), 1f - lastValue);
            final?.Invoke();
        }


        /// <summary>It performs Mathf.SmoothDamp().</summary>
        public static IEnumerator ESmoothDamp(Action<float> action, float time = 1f, bool fixedUpdate = false)
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

        /// <summary>It performs Mathf.SmoothDamp().</summary>
        public static Coroutine SmoothDamp(this MonoBehaviour mono, Action<float> action, float time = 1f, bool fixedUpdate = false)
            => mono.StartCoroutine(ESmoothDamp(action, time, fixedUpdate));
        #endregion




        #region Delta time
        /// <summary>
        /// It gives the delta time through <i>frameInterval</i> frames back to <i>action</i>.<br/>
        /// NOTE: Floating-point errors may occur.
        /// </summary>
        public static IEnumerator EGiveDeltaTime(Action<float> action, Action final = null, float time = 1f, int frameInterval = 1, bool fixedUpdate = false)
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

        /// <inheritdoc cref="GiveDeltaTime(Action{float}, Action, float, int, bool)"/>
        public static Coroutine GiveDeltaTime(this MonoBehaviour mono, Action<float> action, Action final = null, float time = 1f, int frameInterval = 1, bool fixedUpdate = false)
            => mono.StartCoroutine(EGiveDeltaTime(action, final, time, frameInterval, fixedUpdate));
        #endregion




        #region Sequences
        /// <summary>It sequentially invokes <i>actions</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.</summary>
        /// <param name="i">If less than 1, it won't stop.</param>
        /// <param name="interval">If <i>interval</i> &lt; 0, it invokes <i>action</i> every frame.</param>
        public static IEnumerator ELoop(int i = 3, float interval = 1f, params Action[] actions)
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

        /// <inheritdoc cref="ELoop(int, float, Action[])"/>
        public static Coroutine Loop(this MonoBehaviour mono, int i = 3, float interval = 1f, params Action[] actions)
            => mono.StartCoroutine(ELoop(i, interval, actions));


        /// <summary>
        /// It sequentially starts <i>iEnums</i> for <i>i</i> times, waiting <i>interval</i> seconds in between.
        /// </summary>
        /// <param name="i">If less than 1, it won't stop.</param>
        public static IEnumerator ELoop(int i = 3, float interval = 1f, params Func<IEnumerator>[] iEnums)
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

        /// <inheritdoc cref="ELoop(int, float, Func{IEnumerator}[])"/>
        public static Coroutine Loop(this MonoBehaviour mono, int i = 3, float interval = 1f, params Func<IEnumerator>[] iEnums)
            => mono.StartCoroutine(ELoop(i, interval, iEnums));



        /// <summary>
        /// It sequentially invokes <i>actions</i>, waiting <i>interval</i> seconds in between.<br/>
        /// It's the same with <i>Loop(1, interval, actions)</i>.
        /// </summary>
        public static Coroutine DoSequence(this MonoBehaviour mono, float interval, params Action[] actions)
            => mono.Loop(1, interval, actions);

        /// <summary>
        /// It starts <i>iEnums</i> sequentially.<br/>
        /// It's the same with <i>Loop(1, 0, iEnums)</i>.
        /// </summary>
        public static Coroutine DoSequence(this MonoBehaviour mono, params Func<IEnumerator>[] iEnums)
            => mono.Loop(1, 0f, iEnums);



        /// <summary>It invokes <i>action</i> when <i>condition</i> becomes true.</summary>
        public static IEnumerator EDoWhen(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }

        /// <inheritdoc cref="EDoWhen(Func{bool}, Action)"/>
        public static Coroutine DoWhen(this MonoBehaviour mono, Func<bool> condition, Action action)
            => mono.StartCoroutine(EDoWhen(condition, action));


        /// <summary>It invokes <i>action</i> after <i>waitSeconds</i> seconds.</summary>
        public static IEnumerator EDoAfterSeconds(float waitSeconds, Action action)
        {
            yield return new WaitForSeconds(waitSeconds);
            action?.Invoke();
        }

        /// <inheritdoc cref="EDoAfterSeconds(float, Action)"/>
        public static Coroutine DoAfterSeconds(this MonoBehaviour mono, float waitSeconds, Action action)
            => mono.StartCoroutine(EDoAfterSeconds(waitSeconds, action));
        #endregion
    }
}
