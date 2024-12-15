// author: Omnistudio
// version: 2024.10.28

using System.Collections;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Use SetFloat() or LerpTo() to pass a float to <i>material</i>
    /// </summary>
    public class PassFloatToMaterial : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Material material;
        [SerializeField] private string nameOfSetFloat;
        [SerializeField][Range(0.0001f, 1f)] private float lerpSpeed = 0.1f;
        #endregion

        #region Fields
        private float floatToPass;
        private float FloatToPass
        {
            get => floatToPass;
            set
            {
                floatToPass = value;
                material.SetFloat(nameOfSetFloat, floatToPass);
            }
        }
        #endregion

        #region Interfaces
        public void SetFloat(float value) => FloatToPass = value;
        public void LerpTo(float value)
        {
            StopAllCoroutines();
            StartCoroutine(Lerp(FloatToPass, value));
        }
        #endregion

        #region Functions
        private IEnumerator Lerp(float fromPercentage, float toPercentage)
        {
            FloatToPass = fromPercentage;

            while (Mathf.Abs(FloatToPass - toPercentage) > Mathf.Epsilon)
            {
                FloatToPass = Mathf.Lerp(FloatToPass, toPercentage, lerpSpeed);
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }

            FloatToPass = toPercentage;
        }
        #endregion
    }
}
