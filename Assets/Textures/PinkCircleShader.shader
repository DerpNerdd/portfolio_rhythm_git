Shader "Unlit/PinkCircleWithBorder"
{
    Properties
    {
        _Color ("Pink Color", Color) = (0.937, 0.231, 0.616, 1)
        _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
        _BorderThickness ("Border Thickness", Range(0, 0.5)) = 0.025
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // Turn off culling and depth writes for a transparent overlay
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
            };

            fixed4 _Color;
            fixed4 _BorderColor;
            float  _BorderThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;  // pass the UV unmodified
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Center the UV so (0.5, 0.5) is the circle center
                float2 centeredUV = i.uv - 0.5;
                float dist = length(centeredUV);

                // Discard if outside the circle (radius is 0.5)
                if (dist > 0.5)
                    discard;

                // Check if within the border thickness
                if (dist > 0.5 - _BorderThickness)
                    return _BorderColor;

                // Otherwise, inside the circle
                return _Color;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Texture"
}
