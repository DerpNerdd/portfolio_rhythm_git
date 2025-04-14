Shader "UI/Parallelogram" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Skew ("Skew Amount", Float) = 0.2
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            fixed4 _Color;
            float _Skew;
            
            v2f vert (appdata_t IN)
            {
                v2f OUT;
                // Create a skew effect: the offset depends on the Y UV coordinate.
                // When UV.y is 0 (bottom) offset is 0; when UV.y is 1 (top) offset is _Skew.
                float offset = _Skew * IN.texcoord.y;
                IN.vertex.x += offset;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }
            
            fixed4 frag (v2f IN) : SV_Target {
                // Simply output the final color.
                return IN.color;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
