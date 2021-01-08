//shader by rob fichman @bobacupcake!!
//sparkles based on a pure noise texture, rim lighting has been added to add depth to the unlit shader but not necessary for the sparkles
//these concepts can be added to just about any shader!!
//mostly just for show, for a more realistic sparkle effect you will have to combine the sparkles with specular lighting
 
Shader "Unlit/SparkleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
       
        //sparkle variables
        _SparkleTex("Sparkle Texture", 2D) = "white" {} //the noise texture
        _Scale("Scale", Float) = 1 //the scale of the noise texture
        _SparkleColor("SparkleColor", Color) = (1,1,1,1) //color of the sparkle effect
        _Intensity("Intensity", Float) = 50 //intensity of the sparkle effect
 
        //rim lighting variables
        _RimColor("RimColor", Color) = (1,1,1,1) //color of rim lighting
        _RimPower("RimPower", Float) = 1 //intensity of rim lighting
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
           
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL; //passes vertex normal into vert shader
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1; //passes world position into frag shader
                float3 wNormal : TEXCOORD2; //passes world normal into frag shader
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            //sparkle variables
            sampler2D _SparkleTex;
            float _Scale;
            half4 _SparkleColor;
            float _Intensity;
 
            //rim lighting variables
            half4 _RimColor;
            float _RimPower;
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
 
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz; //convert vertex position to world position
                o.wNormal = UnityObjectToWorldNormal(v.normal); //convert vertex normal to world normal
 
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
 
                half3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos); //get the view direction
 
                //sparkle effect
                fixed3 sparklemap = tex2D(_SparkleTex, i.uv*_Scale); //sample the noise texture
                sparklemap -= half3(0.5,0.5,0.5); //change the noise texture into a random direction
                sparklemap = normalize(sparklemap);
                half sparkle = pow(saturate((dot(-viewDirection, normalize(sparklemap + i.wNormal)))),_Intensity); //get a value based on how close you are to looking at the normal offset by a random direction
                col += _SparkleColor*sparkle; //change the color based on this value
 
                //rim lighting
                half rim = pow(1.0 - saturate(dot(-viewDirection, i.wNormal)), _RimPower); //same concept, but the value is based on how far you are from looking at the normal
                col += _RimColor * rim;
 
                return col;
            }
            ENDCG
        }
    }
}