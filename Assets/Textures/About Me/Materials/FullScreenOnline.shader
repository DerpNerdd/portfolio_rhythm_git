Shader "Custom/FullScreenOutline"
{ 
  Properties {
    _OutlineColor    ("Outline Color", Color) = (0,0.5,1,1)
    _OutlineThickness("Outline Thickness", Range(0.0,0.1)) = 0.02
    _GlowColor       ("Glow Color", Color) = (0,0.5,1,0.5)
    _GlowThickness   ("Glow Thickness", Range(0.0,0.2)) = 0.05
    _GlowIntensity   ("Glow Intensity", Float) = 1.0
  }
  SubShader {
    Tags { "Queue"="Overlay" "RenderType"="Transparent" }
    Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
      struct v2f     { float2 uv:TEXCOORD0; float4 pos:SV_POSITION; };

      fixed4 _OutlineColor;
      float  _OutlineThickness;
      fixed4 _GlowColor;
      float  _GlowThickness;
      float  _GlowIntensity;

      v2f vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = v.uv;
        return o;
      }

      fixed4 frag(v2f i) : SV_Target {
        float2 uv = i.uv;
        float d = min(min(uv.x,1-uv.x), min(uv.y,1-uv.y));

        float outline = smoothstep(_OutlineThickness, _OutlineThickness - 0.001, d);
        float glow    = smoothstep(_OutlineThickness + _GlowThickness, _OutlineThickness, d);

        fixed4 col = _OutlineColor * outline;
        col.rgb += _GlowColor.rgb * glow * _GlowIntensity;
        col.a    = max(outline * _OutlineColor.a, glow * _GlowColor.a * _GlowIntensity);

        if(col.a <= 0.001) discard;
        return col;
      }
      ENDCG
    }
  }
  FallBack Off
}
