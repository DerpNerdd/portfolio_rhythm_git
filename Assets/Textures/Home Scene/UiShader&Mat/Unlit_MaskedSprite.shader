Shader "Unlit/PinkCircleWithMaskAndBorder" {
    Properties {
        _Color ("Pink Color", Color) = (0.937, 0.231, 0.616, 1)       // Adjust your pink fill color
        _BorderColor ("Border Color", Color) = (1, 1, 1, 1)             // White border
        _BorderThickness ("Border Thickness", Range(0, 0.5)) = 0.025      // Thickness of the border
        
        _Mask ("Top Mask Texture", 2D) = "white" {}                      // Top mask texture (rotated)
        _MaskOpacity ("Top Mask Opacity", Range(0, 1)) = 0.5             // Opacity for top mask
        _MaskRotation ("Top Mask Rotation", Float) = 0.0                 // Rotation for top mask (in radians)
        
        _Mask2 ("Second Mask Texture", 2D) = "white" {}                  // Underlying second mask texture
        _Mask2Opacity ("Second Mask Opacity", Range(0, 1)) = 0.5          // Opacity for second mask
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // Disable backface culling and depth writes for proper transparency.
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

            // Exposed properties.
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderThickness;
            sampler2D _Mask;
            float _MaskOpacity;
            float _MaskRotation;
            sampler2D _Mask2;
            float _Mask2Opacity;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Recenter the UV so (0.5, 0.5) is the circle's center.
                float2 centeredUV = i.uv - 0.5;
                // Compute the distance from the center; 0.5 in UV space is our radius.
                float dist = length(centeredUV);

                // Discard fragments outside the circle.
                if (dist > 0.5)
                    discard;

                // If within the border ring, output the border color.
                if (dist > 0.5 - _BorderThickness)
                    return _BorderColor;
                else {
                    // --- Top (rotated) Mask ---
                    float2 maskCenter = float2(0.5, 0.5);
                    float2 diff = i.uv - maskCenter;
                    float s = sin(_MaskRotation);
                    float c = cos(_MaskRotation);
                    float2 rotatedDiff = float2(diff.x * c - diff.y * s, diff.x * s + diff.y * c);
                    float2 rotatedUV = rotatedDiff + maskCenter;
                    fixed4 maskSample1 = tex2D(_Mask, rotatedUV);
                    float maskFactor1 = maskSample1.a * _MaskOpacity;

                    // --- Underlying Second Mask ---
                    fixed4 maskSample2 = tex2D(_Mask2, i.uv);
                    // Use luminance (a weighted sum of RGB) to capture the texture details.
                    float maskLuminance = dot(maskSample2.rgb, float3(0.299, 0.587, 0.114));
                    float maskFactor2 = maskLuminance * _Mask2Opacity;

                    // --- Blending Option 1: Sequential Blending ---
                    // First blend the fill color with the border color using the second mask.
                    fixed4 intermediateColor = lerp(_Color, _BorderColor, maskFactor2);
                    // Then further blend that result with the border using the top mask.
                    fixed4 outColor = lerp(intermediateColor, _BorderColor, maskFactor1);
                    
                    /* 
                    // --- Blending Option 2: Additive Blending ---
                    // Uncomment the following lines and comment out the above two lines if you prefer additive blending.
                    float combinedFactor = saturate(maskFactor1 + maskFactor2);
                    fixed4 outColor = lerp(_Color, _BorderColor, combinedFactor);
                    */

                    return outColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
