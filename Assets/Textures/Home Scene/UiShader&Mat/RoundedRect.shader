Shader "UI/RoundedRect" {
    Properties {
        // Dummy texture property required by Unity UI.
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Fill Color", Color) = (1,1,1,1)
        // Corner radius as a fraction of the rect’s half-extent. Try a small value like 0.05.
        _CornerRadius("Corner Radius", Range(0,0.5)) = 0.05
        // AntiAlias controls the smoothness of the edge. A small value (e.g. 0.005) is typical.
        _AntiAlias("Anti Alias", Range(0,0.05)) = 0.005
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

            sampler2D _MainTex; // Required dummy texture property
            fixed4 _Color;
            float _CornerRadius;
            float _AntiAlias;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            
            // Signed distance function for a rounded rectangle.
            // p: the pixel position relative to the center (in UV space, where the image spans [0,1])
            // b: the half-size of the rectangle.
            // r: the corner radius.
            float sdRoundedRect(float2 p, float2 b, float r) {
                float2 d = abs(p) - b;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Transform uv from [0, 1] to [-0.5, 0.5] so (0,0) is the center.
                float2 p = i.uv - 0.5;
                // For a full-coverage UI image, the half-size is 0.5.
                float2 b = float2(0.5, 0.5);
                // Note: we subtract _CornerRadius from the half-size to define the inner rectangle.
                float d = sdRoundedRect(p, b - _CornerRadius, _CornerRadius);
                // Use smoothstep to smoothly transition the alpha (anti-alias the edge).
                float alpha = smoothstep(_AntiAlias, -_AntiAlias, d);
                return _Color * alpha;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
