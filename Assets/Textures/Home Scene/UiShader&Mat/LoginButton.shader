Shader "UI/RoundedRectFigma" {
    Properties {
        // Dummy texture property required by Unity UI.
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Fill Color", Color) = (1,1,1,1)
        // The aspect ratio is defined as (width ÷ height). For example, a value of 2 means the element is twice as wide as tall.
        _Aspect ("Aspect Ratio", Float) = 2.0
        // Border radius as a fraction of the height. A value of 0 means sharp (no rounding) and 0.5 gives full (pill shape) rounding.
        _BorderRadius ("Border Radius", Range(0,0.5)) = 0.5
        // AntiAlias controls the smoothness of the edge.
        _AntiAlias ("Anti Alias", Range(0,0.05)) = 0.005
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
            
            sampler2D _MainTex;
            fixed4 _Color;
            float _Aspect;
            float _BorderRadius;
            float _AntiAlias;
            
            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            
            // Standard capsule signed distance function.
            // This defines a horizontal capsule from point a to b with radius r.
            float sdCapsule(float2 p, float2 a, float2 b, float r) {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = clamp(dot(pa,ba) / dot(ba,ba), 0.0, 1.0);
                return length(pa - ba * h) - r;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                // Map UV to our desired rectangle dimensions.
                // We treat the height as 1 and width as _Aspect.
                float2 uv = i.uv;
                float w = _Aspect;
                float h = 1.0;
                // Center the coordinates: pos goes from (-w/2, -0.5) to (w/2, 0.5).
                float2 pos = float2(uv.x * w, uv.y * h) - float2(w/2.0, h/2.0);
                
                // For a capsule-like button, the left/right edges are rounded with radius r.
                // The endpoints for the capsule are along the horizontal center.
                float r = _BorderRadius;
                float2 a = float2(-w/2.0 + r, 0.0);
                float2 b = float2( w/2.0 - r, 0.0);
                
                // Compute the signed distance.
                float d = sdCapsule(pos, a, b, r);
                // Use smoothstep to anti-alias the edge.
                float alpha = smoothstep(_AntiAlias, -_AntiAlias, d);
                fixed4 col = _Color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
