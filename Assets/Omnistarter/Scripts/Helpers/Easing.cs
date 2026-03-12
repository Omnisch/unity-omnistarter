// author: Omnistudio
// version: 2026.03.12

using System;
using UnityEngine;

namespace Omnis.Utils
{
    /// <summary>
    /// All methods are directed at x starting with 0 and ending with 1.
    /// Being out of range may lead to unexpected effects.
    /// </summary>
    public static class Easing
    {
        public static float Linear(float x) => x;


        public static float SineIn(float x) => 1f - Mathf.Cos(x * Mathf.PI / 2f);
        public static float SineOut(float x) => Mathf.Sin(x * Mathf.PI / 2f);
        public static float SineInOut(float x) => -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        public static float SineRaw(float x) => Mathf.Sin(2f * Mathf.PI * x);


        public static float QuadIn(float x) => x * x;
        public static float QuadOut(float x) => 1f - (1f - x) * (1f - x);


        public static float CubicIn(float x) => x * x * x;
        public static float CubicOut(float x) => 1f - (1f - x) * (1f - x) * (1f - x);


        public static float QuartIn(float x) => x * x * x * x;
        public static float QuartOut(float x) => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x);


        public static float QuintIn(float x) => x * x * x * x * x;
        public static float QuintOut(float x) => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x) * (1f - x);


        public static float ExpoIn(float x) => x == 0f ? 0f : Mathf.Pow(2f, 10f * (x - 1f));
        public static float ExpoOut(float x) => x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);


        /// <remarks>
        /// <para><strong>Note:</strong> It won't check if x is out of the domain and thus may return NAN.</para>
        /// </remarks>
        public static float CircIn(float x) => 1f - Mathf.Sqrt(1f - x * x);
        /// <inheritdoc cref="CircIn(float)"/>
        public static float CircOut(float x) => Mathf.Sqrt(1f - (x - 1f) * (x - 1f));


        public static float BackIn(float x) {
            const float c1 = 1.70158f, c3 = 2.70158f;
            return c3 * x * x * x - c1 * x * x;
        }
        
        public static float BackOut(float x) {
            const float c1 = 1.70158f, c3 = 2.70158f;
            return 1f + c3 * (x - 1f) * (x - 1f) * (x - 1f) + c1 * (x - 1f) * (x - 1f);
        }


        public static float ElasticIn(float x) {
            const float c4 = 2f * Mathf.PI / 3f;
            return x switch {
                0f => 0f,
                1f => 1f,
                _ => -Mathf.Pow(2f, 10f * (x - 1f)) * Mathf.Sin((10f * x - 10.75f) * c4)
            };
        }
        
        public static float ElasticOut(float x) {
            const float c4 = 2f * Mathf.PI / 3f;
            return x switch {
                0f => 0f,
                1f => 1f,
                _ => Mathf.Pow(2f, -10f * x) * Mathf.Sin((10f * x - 0.75f) * c4) + 1f
            };
        }


        public static float BounceIn(float x) {
            return 1f - BounceOut(1f - x);
        }
        
        public static float BounceOut(float x) {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            return x switch {
                < 1f / d1 => n1 * x * x,
                < 2f / d1 => n1 * (x -= 1.5f / d1) * x + 0.75f,
                < 2.5f / d1 => n1 * (x -= 2.25f / d1) * x + 0.9375f,
                _ => n1 * (x -= 2.625f / d1) * x + 0.984375f
            };
        }


        public static float Smoothstep(float x) => Mathf.SmoothStep(0f, 1f, x);
        public static float Smootherstep(float x) => x <= 0f ? 0f : (x >= 1f ? 1f : x * x * x * (6f * x * x - 15f * x + 10f));




        public enum EasingType
        {
            Linear,
            SineIn, SineOut, SineInOut, SineRaw,
            QuadIn, QuadOut, CubicIn, CubicOut, QuartIn, QuartOut, QuintIn, QuintOut,
            ExpoIn, ExpoOut,
            CircIn, CircOut,
            BackIn, BackOut,
            ElasticIn, ElasticOut,
            BounceIn, BounceOut,
            Smoothstep, Smootherstep
        }
        public static Func<float, float> Select(EasingType type)
            => type switch {
                EasingType.Linear => Linear,
                EasingType.SineIn => SineIn,
                EasingType.SineOut => SineOut,
                EasingType.SineInOut => SineInOut,
                EasingType.SineRaw => SineRaw,
                EasingType.QuadIn => QuadIn,
                EasingType.QuadOut => QuadOut,
                EasingType.CubicIn => CubicIn,
                EasingType.CubicOut => CubicOut,
                EasingType.QuartIn => QuartIn,
                EasingType.QuartOut => QuartOut,
                EasingType.QuintIn => QuintIn,
                EasingType.QuintOut => QuintOut,
                EasingType.ExpoIn => ExpoIn,
                EasingType.ExpoOut => ExpoOut,
                EasingType.CircIn => CircIn,
                EasingType.CircOut => CircOut,
                EasingType.BackIn => BackIn,
                EasingType.BackOut => BackOut,
                EasingType.ElasticIn => ElasticIn,
                EasingType.ElasticOut => ElasticOut,
                EasingType.BounceIn => BounceIn,
                EasingType.BounceOut => BounceOut,
                EasingType.Smoothstep => Smoothstep,
                EasingType.Smootherstep => Smootherstep,
                _ => Linear
            };
    }
}
