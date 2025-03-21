Shader "Omnis/Jittery"
{
    Properties
    {
        _MainTex ("Jitter Texture", 2D) = "white" {}
        _PostTex ("Stable Texture", 2D) = "white" {}
        // No noises at gray, +1 at white, -1 at black.
        _NoiseMap ("Noise Map", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(-0.5, 0.5)) = 0.01
        _Frequency("Frequency", Range(0.1, 1)) = 0.333
        _FrameCount("Loop Frame Count", Int) = 3
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "PreviewType"="Plane"
        }
        LOD 200

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _PostTex;
            sampler2D _NoiseMap;

            float _NoiseScale;
            float _Frequency;
            int _FrameCount;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 frag(v2f i) : SV_Target
            {
                float distort = (int)(_Time.y / _Frequency) % _FrameCount;
                float2 cycle = float2(cos(distort), sin(distort + 3.14));
                float4 cNoise = tex2D(_NoiseMap, i.uv + cycle);
                float4 cMain = tex2D(_MainTex, i.uv + 2 * (cNoise.rg - 0.5) * _NoiseScale);
                float4 cPost = tex2D(_PostTex, i.uv);
                float4 c = cMain * cPost;

                return c;
            }
            ENDCG
        }
    }
}
