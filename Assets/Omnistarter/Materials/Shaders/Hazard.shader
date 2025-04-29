Shader "Omnis/Hazard"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (0, 0, 0, 1)
        _SubColor ("Sub Color", Color) = (1, 0.9, 0, 1)
        _Density ("Density", Range(0,100)) = 33.3
        _Width ("Width", Range(0,1)) = 0.5
        _TimeScale ("Speed", Range(0,10)) = 5
        _Horizontal ("Horizontal", Range(-1,1)) = 1
        _Vertical ("Vertical", Range(-1,1)) = -1
    }
    SubShader
    {
        Tags
        {
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "Queue"="AlphaTest"
            "RenderType"="Transparent"
        }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _SubColor;
            float _Density;
            float _Width;
            float _TimeScale;
            float _Horizontal;
            float _Vertical;

            float4 frag(v2f_img i) : SV_Target
            {
                float4 c = tex2D(_MainTex, i.uv) * _Color;
                float4 subc = tex2D(_MainTex, i.uv) * _SubColor;
                
                if (100 * (1 - _Horizontal * i.uv.x - _Vertical * i.uv.y + _TimeScale * _Time.x) % _Density < _Density * _Width)
                    return subc;
                else
                    return c;
            }

            ENDCG
        }
    }
    FallBack Off
}
