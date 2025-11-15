Shader "Omnis/Image/Glow"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "black" {}
        _ShiftColor ("Shift Color", Color) = (0, 0, 0, 0)
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _Weight ("Outline Weight", Float) = 5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "OmnisCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            fixed4 _ShiftColor;
            fixed4 _OutlineColor;
            float _Weight;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 innerCol = col;
                innerCol.rgb = lerp(innerCol.rgb, _ShiftColor.rgb, _ShiftColor.a);

                fixed4 outlineCol = _OutlineColor;
                fixed4 blurred = GaussianBlur5x5(_MainTex, i.uv, _MainTex_TexelSize.xy * _Weight);
                outlineCol.a = blurred.a;

                if (col.a == 1.0) {
                    return innerCol;
                } else if (outlineCol.a > 0.0) {
                    return outlineCol;
                } else {
                    return fixed4(0, 0, 0, 0);
                }
            }

            ENDHLSL
        }
    }
    FallBack Off
}
