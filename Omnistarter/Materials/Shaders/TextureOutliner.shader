Shader "Omnis/TextureOutliner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", COLOR) = (0, 0, 0, 1)
        _Weight ("Outline Weight", Range(0, 1)) = 0.02
        _AspectRatio ("Aspect Ratio", Range(0.1, 10)) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "PreviewType" = "Plane"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #include "OmnisCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _Weight;
            float _AspectRatio;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                if (ColorsAreSame(step(float4(0.5, 0.5, 0.5, 0), c), float4(0, 0, 0, 1)))
                    [unroll] for (float angle = 0; angle < TAU; angle += TAU / 24.0)
                    {
                        float4 adjacentColor;
                        if (_AspectRatio >= 1)
                            adjacentColor = tex2D(_MainTex, i.uv + float2(cos(angle) * _Weight / _AspectRatio, sin(angle) * _Weight));
                        else
                            adjacentColor = tex2D(_MainTex, i.uv + float2(cos(angle) * _Weight, sin(angle) * _Weight * _AspectRatio));
                        if (ColorsAreSame(step(float4(0.5, 0.5, 0.5, 0), adjacentColor), float4(1, 1, 1, 1)))
                            return _OutlineColor;
                    }
                return float4(1, 0, 0, 0);
            }
            ENDCG
        }
    }
}
