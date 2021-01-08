Shader "Yux/PremiumDice"
{
	Properties
	{
		[Header(Main Texture)]
		[Space]
		[NoScaleOffset]
		_MainTex("Texture", 2D) = "white" {}

		[Space(20)]
		[Header(________________________________________________________________)]
		[Header(Emission and Specular)]
		[Space]
		[NoScaleOffset]
		_SpecTex("R = Emission, G = Specular, B = Edge Mask", 2D) = "black" {}
		[Toggle(SPECULAR_ON)]
        _SPECULAR_ON("Specular ON", Float) = 1
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		[Space(10)]
		[Toggle(EMISSIVE_ON)]
        _EMISSIVE_ON("Emissive ON", Float) = 1
		_EmissionColor("Emissive Color", Color) = (1, 1, 1, 1)

		[Space(20)]
		[Header(________________________________________________________________)]
		[Header(Normal Map)]
		[Space]
		[Toggle(NORMAL_ON)]
        _NORMAL_ON("Normal ON", Float) = 1
		[NoScaleOffset]
		_NormalTex("Normal Map", 2D) = "black" {}

		[Space(20)]
		[Header(________________________________________________________________)]
		[Header(Rim Lighting)]
		[Space]
		[Toggle(RIM_ON)]
        _RIM_ON("Rim Lighting ON", Float) = 1
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)

		[Space(20)]
		[Header(________________________________________________________________)]
		[Header(Alpha)]
		[Space]
		_MaskAlpha("Mask Alpha", Float) = 1.0
			
		[Space(20)]
		[Header(________________________________________________________________)]
		[Header(Diffuse Shadow)]
		[Space]
		_ShadowIntensity("Shadow Intensity", Range(0, 1)) = 0.3
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature SPECULAR_ON
            #pragma shader_feature NORMAL_ON

			#include "UnityStandardBRDF.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 tbn[3] : TEXCOORD3; // TEXCOORD4; TEXCOORD5;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _SpecTex;
			float _Smoothness;

			sampler2D _NormalTex;
			
			float4 _EmissionColor;

			float _MaskAlpha;
			float _ShadowIntensity;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = UnityObjectToWorldNormal(v.normal);
				o.normal = UnityObjectToWorldNormal(v.normal);

				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.diff = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));

				//Normal Map
				#ifdef NORMAL_ON
				float3 normal = UnityObjectToWorldNormal(v.normal);
				float3 tangent = UnityObjectToWorldNormal(v.tangent);
				float3 bitangent = cross(tangent, normal);
				o.tbn[0] = tangent;
				o.tbn[1] = bitangent;
				o.tbn[2] = normal;
				#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// Specular
				fixed4 specularTex = tex2D(_SpecTex, i.uv);
				#ifdef SPECULAR_ON
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 halfVector = normalize(lightDir + viewDir);
				float specularMap = specularTex.g;
				float3 specular = pow(DotClamped(halfVector, i.normal), _Smoothness * 100) * specularMap;
				#else
				float3 specular = 0;
                #endif

				//Normal Map
				#ifdef NORMAL_ON
				float3 tangentNormal = tex2D(_NormalTex, i.uv) * 2 - 1;
				float3 surfaceNormal = i.tbn[2];
				float3 worldNormal = float3(i.tbn[0] * tangentNormal.r + i.tbn[1] * tangentNormal.g + i.tbn[2] * tangentNormal.b);
				float normalMap = clamp(DotClamped(worldNormal, _WorldSpaceLightPos0), 0.4, 0.6);
				float finalNormal = normalMap - 0.5;
				#else 
                float finalNormal = 0;
                #endif

				//Emissive
				col += (specularTex.r * _EmissionColor);

				// Diffuse Shadow
				float3 clampedShadow = clamp(i.diff, (1 - _ShadowIntensity), 1.0);

				return float4((col * clampedShadow) + finalNormal + specular, _MaskAlpha);
			}
			ENDCG
		}

		Pass
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend  OneMinusDstColor One  
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature RIM_ON

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				#ifdef RIM_ON
				o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal.xyz));
				o.viewDir = normalize(_WorldSpaceCameraPos - mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
				#endif

				return o;
			}

			fixed4 _RimColor;
			float _MaskAlpha;

			fixed4 frag(v2f i) : COLOR
			{
				#ifdef RIM_ON
				float val = 1 - abs(dot(i.viewDir, i.normal));
				float4 col = _RimColor * _RimColor.a * val * val;
				return col * _MaskAlpha;
				#else
                float4 col = (0,0,0,0);
                return col;
                #endif

				
			}

			ENDCG
		}
	}
}
