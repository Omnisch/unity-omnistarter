// author: Omnistudio
// version: 2024.12.15

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Use <i>FloatToPass</i> or <i>LerpTo</i> to pass a float to <i>material</i>
    /// </summary>
    public class PassFloatToMaterial : MonoBehaviour
    {
        #region Serialized fields
        public Material material;
        public string nameOfParam;
        [SerializeField] private float floatToPass;
        [Range(0.1f, 10f)] public float lerpSpeed = 1f;
        #endregion

        #region Properties
        public float FloatToPass
        {
            get => floatToPass;
            set
            {
                floatToPass = value;
                material.SetFloat(nameOfParam, floatToPass);
            }
        }
        #endregion

        #region Functions
        public void LerpTo(float value)
        {
            StopAllCoroutines();
            var startFloat = FloatToPass;
            StartCoroutine(YieldTweaker.Lerp((x) => FloatToPass = Mathf.Lerp(startFloat, value, x), lerpSpeed));
        }
        #endregion
    }
}
