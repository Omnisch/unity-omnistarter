// author: Omnistudio
// version: 2025.08.20

using UnityEngine;

namespace Omnis.Utils
{
    /// <summary>
    /// Auxiliary methods of UnityEngine.Color.
    /// </summary>
    public static class ColorHelper
    {
        #region Common colors

        #region Half colors
        /// <summary>RGBA is (0.5, 0, 0, 1).</summary>
        public static Color darkRed => new(0.5f, 0f, 0f, 1f);

        /// <summary>RGBA is (0, 0.5, 0, 1).</summary>
        public static Color darkGreen => new(0f, 0.5f, 0f, 1f);
        
        /// <summary>RGBA is (0, 0, 0.5, 1).</summary>
        public static Color darkBlue => new(0f, 0f, 0.5f, 1f);

        /// <summary>RGBA is (0, 0.5, 0.5, 1).</summary>
        public static Color darkCyan => new(0f, 0.5f, 0.5f, 1f);

        /// <summary>RGBA is (0.5, 0, 0.5, 1).</summary>
        public static Color darkMagenta => new(0.5f, 0f, 0.5f, 1f);

        /// <summary>RGBA is (0.5, 0.5, 0, 1).</summary>
        public static Color darkYellow => new(0.5f, 0.5f, 0f, 1f);
        #endregion

        #region Tertiary colors
        /// <summary>RGBA is (0, 1, 0.5, 1).</summary>
        public static Color springGreen => new(0f, 1f, 0.5f, 1f);
        /// <inheritdoc cref="springGreen"/>
        public static Color mint => springGreen;

        /// <summary>RGBA is (0, 0.5, 1, 1).</summary>
        public static Color azure => new(0f, 0.5f, 1f, 1f);

        /// <summary>RGBA is (0.5, 0, 1, 1).</summary>
        public static Color violet => new(0.5f, 0f, 1f, 1f);

        /// <summary>RGBA is (1, 0, 0.5, 1).</summary>
        public static Color rose => new(1f, 0f, 0.5f, 1f);

        /// <summary>RGBA is (1, 0.5, 0, 1).</summary>
        public static Color orange => new(1f, 0.5f, 0f, 1f);

        /// <summary>RGBA is (0.5, 1, 0, 1).</summary>
        public static Color chartreuse => new(0.5f, 1f, 0f, 1f);
        #endregion

        #region Quaternary colors
        /// <summary>RGBA is (0, 1, 0.75, 1).</summary>
        public static Color teal => new(0f, 1f, 0.75f, 1f);

        /// <summary>RGBA is (0, 0.75, 1, 1).</summary>
        public static Color skyBlue => new(0f, 0.75f, 1f, 1f);

        /// <summary>RGBA is (0.75, 0, 1, 1).</summary>
        public static Color purple => new(0.75f, 0f, 1f, 1f);

        /// <summary>RGBA is (1, 0, 0.75, 1)</summary>
        public static Color fuchsia => new(1f, 0f, 0.75f, 1f);

        /// <summary>RGBA is (1, 0.75, 0, 1).</summary>
        public static Color amber => new(1f, 0.75f, 0f, 1f);

        /// <summary>RGBA is (0.75, 1, 0, 1).</summary>
        public static Color lime => new(0.75f, 1f, 0f, 1f);
        #endregion

        #region Other colors
        /// <summary>RGBA is (1, 0.84, 0, 1).</summary>
        public static Color gold => new(1f, 0.84f, 0f, 1f);

        /// <summary>RGBA is (0.75, 0.75, 0.75, 1).</summary>
        public static Color silver => new(0.75f, 0.75f, 0.75f, 1f);

        /// <summary>RGBA is (0.8, 0.5, 0.2, 1).</summary>
        public static Color bronze => new(0.8f, 0.5f, 0.2f, 1f);

        /// <summary>RGBA is (1, 0.75, 0.8, 1).</summary>
        public static Color pink => new(1f, 0.75f, 0.8f, 1f);

        /// <summary>#F5F5F7. RGBA is (0.96, 0.96, 0.97).</summary>
        public static Color appleWhite => new(0.96f, 0.96f, 0.97f, 1f);

        /// <summary>#1D1D1F. RGBA is (0.11, 0.11, 0.12).</summary>
        public static Color appleBlack => new(0.11f, 0.11f, 0.12f, 1f);
        #endregion

        #endregion

        #region Change one value in Color
        /// <returns>(r, g, b, n)</returns>
        public static Color rgbn(this Color color, float n) => new(color.r, color.g, color.b, n);
        #endregion

        #region Color lerp
        public static Color Lerp(Color fromColor, Color toColor, float t) => new(
            Mathf.Lerp(fromColor.r, toColor.r, t),
            Mathf.Lerp(fromColor.g, toColor.g, t),
            Mathf.Lerp(fromColor.b, toColor.b, t),
            Mathf.Lerp(fromColor.a, toColor.a, t));
        #endregion
    }
}
