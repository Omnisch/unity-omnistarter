Shader "Omnis/Image/ProgressCircle"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ShiftColor ("Shift Color", Color) = (1, 1, 1, 1)
        _InnerRadius ("Inner Rim Radius", Range(0, 0.5)) = 0.3
        _OuterRadius ("Outer Rim Radius", Range(0, 0.71)) = 0.5
        _StartAngle ("Start Angle (Deg)", Range(0, 360)) = 90
        _Progress ("Progress Value", Range(0, 1)) = 0.75
        [MaterialToggle] _Clockwise ("Clockwise", Float) = 1
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

            sampler2D _MainTex;
            fixed4 _ShiftColor;
            float _InnerRadius;
            float _OuterRadius;
            float _StartAngle;
            float _Progress;
            float _Clockwise;

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

                float2 v = i.uv - 0.5;

                float d = length(v);
                float ringMask = step(_InnerRadius, d) * step(d, _OuterRadius);

                float rad = atan2(v.y, v.x);
                float t = rad / (2.0 * UNITY_PI);
                t = frac(t + 1.0);

                float start = _StartAngle / 360.0;
                t = frac(t - start);

                if (_Clockwise)
                    t = 1.0 - t;

                float progMask = step(t, _Progress);
                float mask = ringMask * progMask;

                clip(mask - 0.5);

                return innerCol;
            }

            ENDHLSL
        }
    }
    FallBack Off
}
