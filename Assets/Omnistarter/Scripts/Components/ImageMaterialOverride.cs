// author: Omnistudio
// version: 2026.02.12

using Omnis.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Omnis
{
    public class ImageMaterialOverride : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image imageToEdit;
        [Range(0.1f, 10f)]
        public float lerpSpeed = 1f;
        #endregion

        #region Fields
        private Material instancedMat;
        private Dictionary<string, Coroutine> paramDict;
        #endregion


        #region Methods
        private void EnsureMaterialInstance() {
            if (instancedMat != null) return;

            var source = imageToEdit.material != null ? imageToEdit.material : imageToEdit.materialForRendering;
            if (source == null) return;

            instancedMat = Instantiate(source);
            instancedMat.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            imageToEdit.material = instancedMat;
        }

        public T Get<T>(string nameOfParam) {
            Type t = typeof(T);

            if (t == typeof(float))     return (T)(object)instancedMat.GetFloat(nameOfParam);
            if (t == typeof(int))       return (T)(object)instancedMat.GetInt(nameOfParam);
            if (t == typeof(bool))      return (T)(object)instancedMat.GetInt(nameOfParam);
            if (t == typeof(Color))     return (T)(object)instancedMat.GetColor(nameOfParam);
            if (t == typeof(Texture))   return (T)(object)instancedMat.GetTexture(nameOfParam);
            if (t == typeof(Vector2))   return (T)(object)instancedMat.GetVector(nameOfParam);
            if (t == typeof(Vector3))   return (T)(object)instancedMat.GetVector(nameOfParam);
            if (t == typeof(Vector4))   return (T)(object)instancedMat.GetVector(nameOfParam);
            if (t == typeof(Matrix4x4)) return (T)(object)instancedMat.GetMatrix(nameOfParam);

            throw new NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
        }
        public void Set<T>(string nameOfParam, T value) {
            if (paramDict.TryGetValue(nameOfParam, out var last) && last != null) {
                StopCoroutine(last);
            }

            PrivateSet(nameOfParam, value);
        }
        private void PrivateSet<T>(string nameOfParam, T value) {
            switch (value) {
                case float f:
                    instancedMat.SetFloat(nameOfParam, f);
                    break;
                case int i:
                    instancedMat.SetInt(nameOfParam, i);
                    break;
                case bool b:
                    instancedMat.SetInt(nameOfParam, b ? 1 : 0);
                    break;
                case Color c:
                    instancedMat.SetColor(nameOfParam, c);
                    break;
                case Texture tex:
                    instancedMat.SetTexture(nameOfParam, tex);
                    break;
                case Vector2 v2:
                    instancedMat.SetVector(nameOfParam, (Vector4)v2);
                    break;
                case Vector3 v3:
                    instancedMat.SetVector(nameOfParam, (Vector4)v3);
                    break;
                case Vector4 v4:
                    instancedMat.SetVector(nameOfParam, v4);
                    break;
                case Matrix4x4 m:
                    instancedMat.SetMatrix(nameOfParam, m);
                    break;
                default:
                    throw new NotSupportedException($"MaterialPropertyBlock does not support type: {typeof(T).FullName} (param: {nameOfParam})");
            }
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


        #region Unity Messages
        private void Awake() {
            Debug.Assert(imageToEdit != null);
            paramDict = new();
            EnsureMaterialInstance();
        }

        private void OnEnable() {
            EnsureMaterialInstance();
        }

        private void OnDestroy() {
            if (instancedMat != null) {
                Destroy(instancedMat);
                instancedMat = null;
            }
        }
        #endregion
    }
}
