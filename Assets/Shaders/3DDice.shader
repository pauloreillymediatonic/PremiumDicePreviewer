Shader "Yux/3DDice" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LowlightMap ("Lowlight Cubemap(RGB)", CUBE) = "" { }
		_HighlightMap ("Highlight Cubemap(RGB)", CUBE) = "" { }
        _GlowColor ("Glow Color", Color) = (0,0,0,1)
        _GlowMap ("Glow Map", CUBE) = "" { }
	}
       
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			samplerCUBE _LowlightMap;
			samplerCUBE _HighlightMap;
            samplerCUBE _GlowMap;
			float4 _MainTex_ST;
			float4 _Color;
            float4 _GlowColor;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 cubenormal : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.cubenormal = mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(v.normal, 0.0))).xyz;
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
				fixed4 lowlight = texCUBE(_LowlightMap, i.cubenormal);
				fixed4 highlight = texCUBE(_HighlightMap, i.cubenormal);
                fixed4 glow = texCUBE(_GlowMap, i.cubenormal) * _GlowColor;
				fixed4 c = fixed4((2.0f * lowlight.rgb * col.rgb) + highlight + glow, col.a);
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG			
		}
	} 

	Fallback "VertexLit"
}
