// author: Omnistudio
// version: 2026.01.22

using Omnis.Utils;
using UnityEngine;

namespace Omnis
{
    public class ChangeRendererValue : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Renderer rendererToEdit;
        [Range(0.1f, 10f)]
        [SerializeField] private float lerpSpeed = 1f;
        #endregion

        #region Fields
        private MaterialPropertyBlock mpb;
        #endregion


        #region Methods
        public void Set<T>(string nameOfParam, T value) {
            rendererToEdit.GetPropertyBlock(mpb);
            switch (value) {
                case float f:
                    mpb.SetFloat(nameOfParam, f);
                    break;
                case int i:
                    mpb.SetInt(nameOfParam, i);
                    break;
                case bool b:
                    mpb.SetInt(nameOfParam, b ? 1 : 0);
                    break;
                case Color c:
                    mpb.SetColor(nameOfParam, c);
                    break;
                case Vector4 v4:
                    mpb.SetVector(nameOfParam, v4);
                    break;
                case Vector3 v3:
                    mpb.SetVector(nameOfParam, (Vector4)v3);
                    break;
                case Vector2 v2:
                    mpb.SetVector(nameOfParam, (Vector4)v2);
                    break;
                case Matrix4x4 m:
                    mpb.SetMatrix(nameOfParam, m);
                    break;
                case Texture tex:
                    mpb.SetTexture(nameOfParam, tex);
                    break;
                default:
                    throw new System.NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
            }
            rendererToEdit.SetPropertyBlock(mpb);
        }
        public void LerpTo(string nameOfParam, float f) {
            StopAllCoroutines();
            rendererToEdit.GetPropertyBlock(mpb);
            var startFloat = mpb.GetFloat(nameOfParam);
            StartCoroutine(YieldHelper.Ease((value) => Set(nameOfParam, Mathf.Lerp(startFloat, f, value)), Easing.Linear, lerpSpeed));
        }
        public void LerpTo(string nameOfParam, Color c) {
            StopAllCoroutines();
            rendererToEdit.GetPropertyBlock(mpb);
            var startColor = mpb.GetColor(nameOfParam);
            StartCoroutine(YieldHelper.Ease((value) => Set(nameOfParam, ColorHelper.Lerp(startColor, c, value)), Easing.Linear, lerpSpeed));
        }
        #endregion


        #region Unity Methods
        private void Awake() {
            mpb = new MaterialPropertyBlock();
        }
        #endregion
    }
}
