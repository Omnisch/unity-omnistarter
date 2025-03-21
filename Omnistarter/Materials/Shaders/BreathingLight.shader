Shader "Omnis/BreathingLight"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Scale("Breathe Scale", Range(0, 1)) = 1
        _Velocity("Breathe Velocity", Float) = 1
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
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
            fixed4 _Color;
            float _Scale;
            float _Velocity;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                c *= fixed4(_Color.rgb, _Scale * 0.5 * (sin(_Velocity * _Time.y) + 1));
                return c;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
