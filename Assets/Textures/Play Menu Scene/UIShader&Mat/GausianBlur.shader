Shader "UI/GaussianBlur"
{
    Properties
    {
        _MainTex   ("Texture", 2D) = "white" {}
        _BlurSize  ("Blur Size", Range(0.5, 5.0)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "Queue"         = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"    = "Transparent"
            "PreviewType"   = "Plane"
        }
        LOD 100

        Pass
        {
            // First pass: horizontal blur
            Name "HORIZONTAL"
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragH
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_TexelSize; // x = 1/width, y = 1/height
            float     _BlurSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                fixed4 col  : COLOR;
            };

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.uv  = IN.uv;
                OUT.col = IN.color;
                return OUT;
            }

            fixed4 fragH(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 off = float2(_MainTex_TexelSize.x, 0) * _BlurSize;

                fixed4 sum = fixed4(0,0,0,0);
                sum += tex2D(_MainTex, uv + off * -4) * 0.05;
                sum += tex2D(_MainTex, uv + off * -3) * 0.09;
                sum += tex2D(_MainTex, uv + off * -2) * 0.12;
                sum += tex2D(_MainTex, uv + off * -1) * 0.15;
                sum += tex2D(_MainTex, uv             ) * 0.16;
                sum += tex2D(_MainTex, uv + off *  1) * 0.15;
                sum += tex2D(_MainTex, uv + off *  2) * 0.12;
                sum += tex2D(_MainTex, uv + off *  3) * 0.09;
                sum += tex2D(_MainTex, uv + off *  4) * 0.05;
                return sum * IN.col;
            }
            ENDCG
        }

        Pass
        {
            // Second pass: vertical blur
            Name "VERTICAL"
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragV
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_TexelSize;
            float     _BlurSize;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f    { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; fixed4 col:COLOR; };

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.uv  = IN.uv;
                OUT.col = IN.color;
                return OUT;
            }

            fixed4 fragV(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 off = float2(0, _MainTex_TexelSize.y) * _BlurSize;

                fixed4 sum = fixed4(0,0,0,0);
                sum += tex2D(_MainTex, uv + off * -4) * 0.05;
                sum += tex2D(_MainTex, uv + off * -3) * 0.09;
                sum += tex2D(_MainTex, uv + off * -2) * 0.12;
                sum += tex2D(_MainTex, uv + off * -1) * 0.15;
                sum += tex2D(_MainTex, uv             ) * 0.16;
                sum += tex2D(_MainTex, uv + off *  1) * 0.15;
                sum += tex2D(_MainTex, uv + off *  2) * 0.12;
                sum += tex2D(_MainTex, uv + off *  3) * 0.09;
                sum += tex2D(_MainTex, uv + off *  4) * 0.05;
                return sum * IN.col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
