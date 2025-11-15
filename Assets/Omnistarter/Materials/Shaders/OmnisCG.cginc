#ifndef OmnisCG
#define OmnisCG

#include "UnityCG.cginc"

#define PI 3.1415926
#define TAU 6.2831853

bool ColorsAreSame(float4 c1, float4 c2)
{
    return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
}

// 1-dimensional Gaussian blur: dir = (1,0) horizontal; (0,1) vertical; or any direction
float4 GaussianBlur9(sampler2D tex, float2 uv, float2 texelSize, float2 dir)
{
    float2 step = dir * texelSize;

    float4 color = 0;

    const float w0 = 0.227027f; // center
    const float w1 = 0.1945946f;
    const float w2 = 0.1216216f;
    const float w3 = 0.0540540f;
    const float w4 = 0.0162160f;

    color += tex2D(tex, uv)              * w0;
    color += tex2D(tex, uv + step * 1.0) * w1;
    color += tex2D(tex, uv - step * 1.0) * w1;
    color += tex2D(tex, uv + step * 2.0) * w2;
    color += tex2D(tex, uv - step * 2.0) * w2;
    color += tex2D(tex, uv + step * 3.0) * w3;
    color += tex2D(tex, uv - step * 3.0) * w3;
    color += tex2D(tex, uv + step * 4.0) * w4;
    color += tex2D(tex, uv - step * 4.0) * w4;

    return color;
}

float4 GaussianBlur5x5(sampler2D tex, float2 uv, float2 texel)
{
    const float kernel[5][5] = {
        { 1,  4,  7,  4, 1 },
        { 4, 16, 26, 16, 4 },
        { 7, 26, 41, 26, 7 },
        { 4, 16, 26, 16, 4 },
        { 1,  4,  7,  4, 1 }
    };

    float sumWeight = 273.0;
    float4 c = 0;

    [unroll]
    for (int y = -2; y <= 2; y++)
    {
        [unroll]
        for (int x = -2; x <= 2; x++)
        {
            float w = kernel[y+2][x+2];
            c += tex2D(tex, uv + texel * float2(x, y)) * w;
        }
    }

    return c / sumWeight;
}

#endif