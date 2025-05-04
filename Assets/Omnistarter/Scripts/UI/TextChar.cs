// author: Omnistudio
// version: 2025.05.04

using System;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    public static class TextChar
    {
        /// <summary>
        /// Note: Includes NO ForceUpdate() or UpdateGeometry().
        /// </summary>
        /// <param name="action">int: The index of the current character, [0, <i>toCharIndex</i> - <i>fromCharIndex</i>).</param>
        /// <param name="fromCharIndex">Inclusive.</param>
        /// <param name="toCharIndex">Exclusive.</param>
        public static void MoveChar(this TMP_Text text, Func<int, Vector3> action, int fromCharIndex, int toCharIndex)
        {
            if (action == null) return;

            var textInfo = text.textInfo;

            for (int i = fromCharIndex; i < toCharIndex; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                int matIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;

                int vertexIndex = charInfo.vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] += action.Invoke(i - fromCharIndex);
                }
            }
        }

        /// <inheritdoc cref="MoveChar(TMP_Text, Func{int, Vector3}, int, int)"/>
        public static void PaintChar(this TMP_Text text, Func<int, Color> action, int fromCharIndex, int toCharIndex)
        {
            if (action == null) return;

            var textInfo = text.textInfo;

            for (int i = fromCharIndex; i < toCharIndex; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                int matIndex = charInfo.materialReferenceIndex;
                Color32[] colors32 = textInfo.meshInfo[matIndex].colors32;

                int vertexIndex = charInfo.vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    colors32[vertexIndex + j] = (Color32)action.Invoke(i - fromCharIndex);
                }
            }
        }
    }
}
