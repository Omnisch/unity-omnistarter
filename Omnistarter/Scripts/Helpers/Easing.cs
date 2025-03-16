// author: Omnistudio
// version: 2025.03.16

using UnityEngine;

namespace Omnis.Utils
{
    public static class Easing
    {
        public static float Linear(float x) => x;


        public static float InSine(float x) => 1f - Mathf.Cos(x * Mathf.PI / 2f);
        public static float OutSine(float x) => Mathf.Sin(x * Mathf.PI / 2f);


        public static float InQuad(float x) => x * x;
        public static float OutQuad(float x) => 1f - (1f - x) * (1f - x);


        public static float InCubic(float x) => x * x * x;
        public static float OutCubic(float x) => 1f - (1f - x) * (1f - x) * (1f - x);


        public static float InQuart(float x) => x * x * x * x;
        public static float OutQuart(float x) => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x);


        public static float InQuint(float x) => x * x * x * x * x;
        public static float OutQuint(float x) => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x) * (1f - x);


        public static float InExpo(float x) => x == 0f ? 0f : Mathf.Pow(2f, 10f * (x - 1f));
        public static float OutExpo(float x) => x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);


        public static float InCirc(float x) => 1f - Mathf.Sqrt(1f - x * x);
        public static float OutCirc(float x) => Mathf.Sqrt(1f - (x - 1f) * (x - 1f));


        public static float InBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * x * x * x - c1 * x * x;
        }
        public static float OutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * (x - 1f) * (x - 1f) * (x - 1f) + c1 * (x - 1f) * (x - 1f);
        }


        public static float InElastic(float x)
        {
            const float c4 = 2f * Mathf.PI / 3f;
            return x == 0f ? 0f : (x == 1f ? 1f : -Mathf.Pow(2f, 10f * (x - 1f)) * Mathf.Sin((10f * x - 10.75f) * c4));
        }
        public static float OutElastic(float x)
        {
            const float c4 = 2f * Mathf.PI / 3f;
            return x == 0f ? 0f : (x == 1f ? 1f : Mathf.Pow(2f, -10f * x) * Mathf.Sin((10f * x - 0.75f) * c4) + 1f);
        }


        public static float InBounce(float x)
        {
            return 1f - OutBounce(1f - x);
        }
        public static float OutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1f / d1) return n1 * x * x;
            else if (x < 2f / d1) return n1 * (x -= 1.5f / d1) * x + 0.75f;
            else if (x < 2.5f / d1) return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            else return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
}
