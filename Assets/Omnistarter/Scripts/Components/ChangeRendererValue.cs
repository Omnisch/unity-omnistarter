// author: Omnistudio
// version: 2026.02.12

using Omnis.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    public class ChangeRendererValue : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Renderer rendererToEdit;
        [Range(0.1f, 10f)]
        public float lerpSpeed = 1f;
        #endregion

        #region Fields
        private MaterialPropertyBlock mpb;
        private Dictionary<string, Coroutine> paramDict;
        #endregion


        #region Methods
        public T GetShared<T>(string nameOfParam) {
            Type t = typeof(T);

            if (t == typeof(float))     return (T)(object)rendererToEdit.sharedMaterial.GetFloat(nameOfParam);
            if (t == typeof(int))       return (T)(object)rendererToEdit.sharedMaterial.GetInt(nameOfParam);
            if (t == typeof(bool))      return (T)(object)rendererToEdit.sharedMaterial.GetInt(nameOfParam);
            if (t == typeof(Color))     return (T)(object)rendererToEdit.sharedMaterial.GetColor(nameOfParam);
            if (t == typeof(Texture))   return (T)(object)rendererToEdit.sharedMaterial.GetTexture(nameOfParam);
            if (t == typeof(Vector2))   return (T)(object)rendererToEdit.sharedMaterial.GetVector(nameOfParam);
            if (t == typeof(Vector3))   return (T)(object)rendererToEdit.sharedMaterial.GetVector(nameOfParam);
            if (t == typeof(Vector4))   return (T)(object)rendererToEdit.sharedMaterial.GetVector(nameOfParam);
            if (t == typeof(Matrix4x4)) return (T)(object)rendererToEdit.sharedMaterial.GetMatrix(nameOfParam);

            throw new NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
        }
        public T Get<T>(string nameOfParam) {
            if (!paramDict.ContainsKey(nameOfParam)) return GetShared<T>(nameOfParam);

            rendererToEdit.GetPropertyBlock(mpb);
            Type t = typeof(T);

            if (t == typeof(float))     return (T)(object)mpb.GetFloat(nameOfParam);
            if (t == typeof(int))       return (T)(object)mpb.GetInt(nameOfParam);
            if (t == typeof(bool))      return (T)(object)mpb.GetInt(nameOfParam);
            if (t == typeof(Color))     return (T)(object)mpb.GetColor(nameOfParam);
            if (t == typeof(Texture))   return (T)(object)mpb.GetTexture(nameOfParam);
            if (t == typeof(Vector2))   return (T)(object)mpb.GetVector(nameOfParam);
            if (t == typeof(Vector3))   return (T)(object)mpb.GetVector(nameOfParam);
            if (t == typeof(Vector4))   return (T)(object)mpb.GetVector(nameOfParam);
            if (t == typeof(Matrix4x4)) return (T)(object)mpb.GetMatrix(nameOfParam);

            throw new NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
        }
        public void Set<T>(string nameOfParam, T value) {
            if (paramDict.TryGetValue(nameOfParam, out var last) && last != null) {
                StopCoroutine(last);
            }

            PrivateSet(nameOfParam, value);
        }
        private void PrivateSet<T>(string nameOfParam, T value) {
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
                case Texture tex:
                    mpb.SetTexture(nameOfParam, tex);
                    break;
                case Vector2 v2:
                    mpb.SetVector(nameOfParam, (Vector4)v2);
                    break;
                case Vector3 v3:
                    mpb.SetVector(nameOfParam, (Vector4)v3);
                    break;
                case Vector4 v4:
                    mpb.SetVector(nameOfParam, v4);
                    break;
                case Matrix4x4 m:
                    mpb.SetMatrix(nameOfParam, m);
                    break;
                default:
                    throw new NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
            }
            rendererToEdit.SetPropertyBlock(mpb);
        }


        public void LerpTo(string nameOfParam, float f, Action callback = null) {
            if (paramDict.TryGetValue(nameOfParam, out var last) && last != null) {
                StopCoroutine(last);
            }

            var startFloat = Get<float>(nameOfParam);
            paramDict[nameOfParam] = StartCoroutine(YieldHelper.Ease((value) => {
                PrivateSet(nameOfParam, Mathf.Lerp(startFloat, f, value));

                if (value == 1f) {
                    callback?.Invoke();
                }
            }, Easing.Linear, lerpSpeed));
        }
        public void LerpTo(string nameOfParam, Color c, Action callback = null) {
            if (paramDict.TryGetValue(nameOfParam, out var last) && last != null) {
                StopCoroutine(last);
            }

            var startColor = Get<Color>(nameOfParam);
            paramDict[nameOfParam] = StartCoroutine(YieldHelper.Ease((value) => {
                PrivateSet(nameOfParam, ColorHelper.Lerp(startColor, c, value));

                if (value == 1f) {
                    callback?.Invoke();
                }
            }, Easing.Linear, lerpSpeed));
        }
        #endregion


        #region Unity Methods
        private void Awake() {
            mpb = new();
            paramDict = new();
        }
        #endregion
    }
}
