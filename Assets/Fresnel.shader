Shader "Fresnel"
{
	Properties
	{
		_MainTex0 ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias ("Fresnel Bias", Float) = 0
		_FresnelScale ("Fresnel Scale", Float) = 1
		_FresnelPower ("Fresnel Power", Float) = 1
        _FatAmount ("Fat Amount", Range(0,1)) = 1

        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}

	}

	SubShader
	{
        
		Tags
		{
			"LightMode"="ForwardBase"
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}

		Cull Back


		Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            fixed _FatAmount;

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                v.vertex.xyz += v.normal * _FatAmount;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }

        Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma target 2.0

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				half3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float fresnel : TEXCOORD1;
			};

			sampler2D _MainTex0;
			float4 _MainTex0_ST;
			fixed4 _Color;
			fixed4 _FresnelColor;
			fixed _FresnelBias;
			fixed _FresnelScale;
			fixed _FresnelPower;
            fixed _FatAmount;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos.xyz + v.normal * _FatAmount);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex0);

				float3 i = normalize(ObjSpaceViewDir(v.pos));
				o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(i, v.normal), _FresnelPower);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex0, i.uv) * _Color;
                return lerp(c, _FresnelColor, 1 - i.fresnel);
			}
			ENDCG
		}
		Pass
        {
            
            Tags {"LightMode"="ForwardBase"}
            //Blend Zero
            Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            fixed _FatAmount;
            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
            };
            v2f vert (appdata_base v)
            {
                v2f o;
                v.vertex.xyz += v.normal * _FatAmount;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                //o.diff = nl * _LightColor0.rgb;
                o.diff = _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                // compute shadows data
                TRANSFER_SHADOW(o)
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = i.diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        
	}
}
 