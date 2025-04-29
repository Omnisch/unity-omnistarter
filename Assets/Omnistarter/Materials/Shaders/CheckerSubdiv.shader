Shader "Omnis/CheckerSubdiv"
{
    Properties
    {
        _KingTex ("King Texture", 2D) = "white" {}
        _KingTint ("King Tint", Color) = (0, 0, 0, 1)
        _QueenTex ("Queen Texture", 2D) = "white" {}
        _QueenTint ("Queen Tint", Color) = (1, 1, 1, 1)
        _SubdivisionLevel ("Subdivision Level", Range(0, 12)) = 1
        _AspectRatio ("Aspect Ratio", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "PreviewType"="Plane"
        }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _KingTex;
            sampler2D _QueenTex;
            float4 _KingTint;
            float4 _QueenTint;

            int _SubdivisionLevel;
            float _AspectRatio;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 frag(v2f i) : SV_Target
            {
                float2 scaledUv = float2(i.uv.x * _AspectRatio, i.uv.y);
                float4 cKing = tex2D(_KingTex, i.uv);
                float4 cQueen = tex2D(_QueenTex, i.uv);

                float mult = pow(2, _SubdivisionLevel);
                if ((floor(scaledUv.x * mult) + floor(scaledUv.y * mult)) % 2 == 0)
                    return cKing * _KingTint;
                else
                    return cQueen * _QueenTint;
            }
            ENDCG
        }
    }
}
