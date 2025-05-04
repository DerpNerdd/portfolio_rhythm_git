Shader "Custom/GradientBG"
{
    Properties
    {
        _TopColor    ("Top Color",    Color) = (0,0,0,1)
        _BottomColor("Bottom Color", Color) = (0.05,0.05,0.05,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            fixed4 _TopColor, _BottomColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Map vertex XY (−1→1) to UV (0→1)
                o.uv  = v.vertex.xy * 0.5 + 0.5;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Lerp bottom→top by screen-space Y
                return lerp(_BottomColor, _TopColor, i.uv.y);
            }
            ENDCG
        }
    }
}
