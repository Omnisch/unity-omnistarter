Shader "Omnis/Glass"
{
    Properties
    {
        _MainTint("Main Tint", Color) = (1, 1, 1, 1)
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _BlurSize("BlurSize", Range(0,15)) = 4
    }
        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        GrabPass {"_GrabTex"}

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "OmnisCG.cginc"

            fixed4 _MainTint;
            sampler2D _NoiseTex;
            sampler2D _GrabTex;
            float _BlurSize;

            struct a2v
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                half2 uv : TEXCOORD0;
                float4 scrPos : TEXCOORD1;
            };

            v2f vert(a2v v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.scrPos = o.pos;

                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                fixed3 normal = UnpackNormal(tex2D(_NoiseTex, i.uv));

                float2 screenPos = i.scrPos.xy / i.scrPos.w;
                float depth = _BlurSize * 0.005 * normal.r * normal.b;

                screenPos.x = (screenPos.x + 1) * 0.5;
                screenPos.y = 1 - (screenPos.y + 1) * 0.5;

                float4 gaussian = ApplyGaussianKernel(_GrabTex, screenPos, depth);
                return gaussian * _MainTint;
            }

            ENDCG
        }
    }
        Fallback Off
}
