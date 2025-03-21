Shader "Omnis/Aperture"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "black" {}
        _ShiftColor ("ShiftColor", Color) = (1, 1, 1, 1)
        _BlendColor ("Blend Color", Color) = (1, 1, 1, 1)
        _Radius ("Cutout Radius", Range(0, 0.708)) = 0.333
        _Stretch ("Height Stretch", Range(-0.5, 0.5)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _ShiftColor;
            fixed4 _BlendColor;
            float _Radius;
            float _Stretch;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 frag(v2f i) : SV_Target
            {
                // Transparent if out of pic.
                if (i.uv.y < _Stretch || i.uv.y >= 1 - _Stretch)
                    return fixed4(1, 1, 1, 0);
                if (i.uv.x < -_Stretch || i.uv.x >= 1 + _Stretch)
                    return fixed4(1, 1, 1, 0);
                // Transparent if in the circle.
                if (length(i.uv - fixed2(0.5, 0.5)) < _Radius)
                    return fixed4(1, 1, 1, 0);
                // Straight paint.
                fixed4 c;
                if (_Stretch >= 0)
                {
                    float aspectRatio = 1 / (1.0 - 2.0 * _Stretch);
                    c = tex2D(_MainTex, float2(i.uv.x, (i.uv.y - _Stretch) * aspectRatio));
                }
                else
                {
                    float aspectRatio = 1 / (1.0 + 2.0 * _Stretch);
                    c = tex2D(_MainTex, float2((i.uv.x + _Stretch) * aspectRatio, i.uv.y));
                }
                c = fixed4(1, 1, 1, 1) - (fixed4(1, 1, 1, 1) - c) * (fixed4(1, 1, 1, 1) - _ShiftColor);
                c *= _BlendColor;
                return c;
            }

            ENDCG
        }
    }
    FallBack Off
}
