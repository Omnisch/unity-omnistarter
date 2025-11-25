// author: Omnistudio
// version: 2025.11.25

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Use <i>FloatToPass</i> or <i>LerpTo</i> to pass a float to <i>rendererToEdit</i>.
    /// </summary>
    public class PassValueToRenderer : MonoBehaviour
    {
        #region Serialized Fields
        public Renderer rendererToEdit;
        public string nameOfParam;
        [SerializeField] private float floatToPass;
        [Range(0.1f, 10f)] public float lerpSpeed = 1f;
        #endregion

        #region Fields
        private MaterialPropertyBlock mpb;
        #endregion

        #region Properties
        public float FloatToPass {
            get => floatToPass;
            set {
                floatToPass = value;
                rendererToEdit.GetPropertyBlock(mpb);
                mpb.SetFloat(nameOfParam, floatToPass);
                rendererToEdit.SetPropertyBlock(mpb);
            }
        }
        #endregion

        #region Methods
        public void LerpTo(float value) {
            StopAllCoroutines();
            var startFloat = FloatToPass;
            StartCoroutine(Utils.YieldHelper.Ease((x) => FloatToPass = Mathf.Lerp(startFloat, value, x), Utils.Easing.Linear));
        }
        #endregion

        #region Unity Methods
        private void Awake() {
            mpb = new MaterialPropertyBlock();
        }
        #endregion
    }
}
