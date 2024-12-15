// author: Omnistudio
// version: 2024.10.28

using System.Collections;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Slightly move the transform to imitate effects like trembling.
    /// </summary>
    public class TransformTwist : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// In seconds
        /// </summary>
        [SerializeField] private float tick = 0.03f;
        #endregion

        #region Fields
        private WaitForSeconds WaitForTicks(int tickCount = 1) => new(tickCount * tick);
        #endregion

        #region Interfaces
        public void InstantMove(Vector3 dir)
            => transform.localPosition += dir;

        public void Vibrate(Vector3 dir, int times = 16, int intervalTicks = 1)
        {
            StopAllCoroutines();
            StartCoroutine(IVibrate(dir, times, intervalTicks));
        }
        public void Slide(Vector3 dir, float time = 1f)
        {
            StopAllCoroutines();
            StartCoroutine(ISlide(dir, time));
        }
        public void Attenuate(Vector3 dir, float speed = 1f)
        {
            StopAllCoroutines();
            StartCoroutine(IAttenuate(dir, speed));
        }
        public void KernelWave(Vector3 dir, float force = 1f)
        {
            StopAllCoroutines();
            StartCoroutine(IKernelWave(dir, force));
        }
        #endregion

        #region Functions
        private IEnumerator IVibrate(Vector3 dir, int times, int intervalTicks)
        {
            Vector3 oldPos = transform.localPosition;
            for (int i = 0; i < times; i++)
            {
                transform.localPosition = oldPos + dir;
                yield return WaitForTicks(intervalTicks);
                transform.localPosition = oldPos - dir;
                yield return WaitForTicks(intervalTicks);
            }
            transform.localPosition = oldPos;
        }
        private IEnumerator ISlide(Vector3 dir, float time)
        {
            for (float runtime = 0f; runtime < time; runtime += tick)
            {
                transform.position += dir;
                yield return WaitForTicks();
            }
        }
        private IEnumerator IAttenuate(Vector3 dir, float speed)
        {
            Vector3 oldPos = transform.localPosition;
            float t = 0f, A = 1f;
            while (A > 0.01f)
            {
                transform.localPosition = oldPos + A * Mathf.Sin(t) * dir;
                A *= 0.99f;
                t += speed * tick;
                yield return WaitForTicks();
            }
            transform.localPosition = oldPos;
        }
        private IEnumerator IKernelWave(Vector3 dir, float speed)
        {
            Vector3 oldPos = transform.localPosition;
            float t = 0f, A = 0f;
            while (A < 1f)
            {
                transform.localPosition = oldPos + A * Mathf.Sin(t) * dir;
                A *= 1.01f;
                t += speed * tick;
                yield return WaitForTicks();
            }
            while (A > 0.01f)
            {
                transform.localPosition = oldPos + A * Mathf.Sin(t) * dir;
                A *= 0.99f;
                t += speed * tick;
                yield return WaitForTicks();
            }
            transform.localPosition = oldPos;
        }
        #endregion
    }
}
