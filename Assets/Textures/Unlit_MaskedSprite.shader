Shader "Unlit/PinkCircleWithMaskAndBorder" {
    Properties {
        _Color ("Pink Color", Color) = (0.937, 0.231, 0.616, 1)       // Adjust your pink fill color
        _BorderColor ("Border Color", Color) = (1, 1, 1, 1)             // White border
        _BorderThickness ("Border Thickness", Range(0, 0.5)) = 0.025      // Thickness of the border
        _Mask ("Mask Texture", 2D) = "white" {}                          // Your mask texture (e.g., the textured triangle)
        _MaskOpacity ("Mask Opacity", Range(0, 1)) = 0.5                 // Controls how opaque the mask overlay is
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // Disable backface culling and depth writes for proper transparency
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Exposed properties
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderThickness;
            sampler2D _Mask;
            float _MaskOpacity;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Pass UVs as-is (assuming they range from 0 to 1)
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Recenter UV so that (0.5, 0.5) is the circle’s center
                float2 centeredUV = i.uv - 0.5;
                // Compute the distance from the center; 0.5 is our radius (UV space)
                float dist = length(centeredUV);

                // Discard fragments outside the circle (hard edge)
                if (dist > 0.5)
                    discard;

                // If within the border ring, output the border color
                if (dist > 0.5 - _BorderThickness)
                    return _BorderColor;
                else {
                    // Inside the circle: sample the mask texture and blend
                    fixed4 maskSample = tex2D(_Mask, i.uv);
                    // Multiply the mask's alpha by the mask opacity parameter
                    float maskFactor = maskSample.a * _MaskOpacity;
                    // Lerp between the fill color and border color using the mask factor.
                    // (If maskFactor = 0, fully _Color; if 1, fully _BorderColor)
                    fixed4 outColor = lerp(_Color, _BorderColor, maskFactor);
                    return outColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
