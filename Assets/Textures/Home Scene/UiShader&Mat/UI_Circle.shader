Shader "UI/Circle" {
    Properties {
        _Color("Fill Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Range(0,0.5)) = 0.05
    }
    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
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
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Compute distance from the center of the UV space (0.5, 0.5)
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float radius = 0.5;
                // Make a smooth edge at the border
                float edge = smoothstep(radius, radius - 0.01, dist);
                // Compute an outline, blending from the outline color to the fill color
                float outline = smoothstep(radius, radius - _OutlineThickness, dist);
                fixed4 col = lerp(_OutlineColor, _Color, outline);
                col.a *= edge;
                return col;
            }
            ENDCG
        }
    }
}
