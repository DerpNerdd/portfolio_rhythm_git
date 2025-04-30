Shader "Custom/UnlitVerticalGradient"
{
    Properties
    {
        _TopColor    ("Top Color", Color) = (0.141,0.129,0.325,1)   // #242153
        _BottomColor ("Bottom Color", Color) = (0.502,0.173,0.455,1) // #802C74
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            struct v2f {
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Interpolate between bottom (uv.y=0) and top (uv.y=1)
                return lerp(_BottomColor, _TopColor, i.uv.y);
            }
            ENDCG
        }
    }
    FallBack Off
}
