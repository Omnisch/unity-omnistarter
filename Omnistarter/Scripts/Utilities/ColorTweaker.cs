// author: Omnistudio
// version: 2024.11.01

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Auxiliary functions of UnityEngine.Color.
    /// </summary>
    public class ColorTweaker
    {
        #region Common colors

        #region Tertiary colors
        /// <summary>Orange. RGBA is (1, 0.5, 0, 1).</summary>
        public static Color orange => new(1f, 0.5f, 0f, 1f);

        /// <summary>Chartreuse. RGBA is (0.5, 1, 0, 1).</summary>
        public static Color chartreuse => new(0.5f, 1f, 0f, 1f);

        /// <summary>Spring green. RGBA is (0, 1, 0.5, 1).</summary>
        public static Color springGreen => new(0f, 1f, 0.5f, 1f);

        /// <summary>Azure. RGBA is (0, 0.5, 1, 1).</summary>
        public static Color azure => new(0f, 0.5f, 1f, 1f);

        /// <summary>Violet. RGBA is (0.5, 0, 1, 1).</summary>
        public static Color violet => new(0.5f, 0f, 1f, 1f);

        /// <summary>Rose. RGBA is (1, 0, 0.5, 1).</summary>
        public static Color rose => new(1f, 0f, 0.5f, 1f);
        #endregion

        #region Quaternary colors
        /// <summary>Amber. RGBA is (1, 0.75, 0, 1).</summary>
        public static Color amber => new(1f, 0.75f, 0f, 1f);

        /// <summary>Lime. RGBA is (0.75, 1, 0, 1).</summary>
        public static Color lime => new(0.75f, 1f, 0f, 1f);

        /// <summary>Sky blue. RGBA is (0, 0.75, 1, 1).</summary>
        public static Color skyBlue => new(0f, 0.75f, 1f, 1f);

        /// <summary>Purple. RGBA is (0.75, 0, 1, 1).</summary>
        public static Color purple => new(0.75f, 0f, 1f, 1f);
        #endregion

        #region Other colors
        /// <summary>Gold. RGBA is (1, 0.84, 0, 1).</summary>
        public static Color gold => new(1f, 0.84f, 0f, 1f);

        /// <summary>Silver. RGBA is (0.75, 0.75, 0.75, 1).</summary>
        public static Color silver => new(0.75f, 0.75f, 0.75f, 1f);

        /// <summary>Bronze. RGBA is (0.8, 0.5, 0.2, 1).</summary>
        public static Color bronze => new(0.8f, 0.5f, 0.2f, 1f);

        /// <summary>Pink. RGBA is (1, 0.75, 0.8, 1).</summary>
        public static Color pink => new(1f, 0.75f, 0.8f, 1f);
        #endregion

        #endregion

        #region Color Lerp
        public static Color LerpFromColorToColor(Color fromColor, Color toColor, float t) => new(
            Mathf.Lerp(fromColor.r, toColor.r, t),
            Mathf.Lerp(fromColor.g, toColor.g, t),
            Mathf.Lerp(fromColor.b, toColor.b, t),
            Mathf.Lerp(fromColor.a, toColor.a, t));
        #endregion
    }
}
