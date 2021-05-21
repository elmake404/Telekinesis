Shader "Unlit/ApplyOutline"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _ColorFill ("ColorFill (RGBA)", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorFill;
            
            sampler2D _SelectionBuffer;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float maxAlpha = 0;
                maxAlpha = max(maxAlpha, tex2D(_SelectionBuffer, i.uv).a);
                
                
                fixed4 col = tex2D(_MainTex, i.uv);
                col = lerp(col * _ColorFill, col, maxAlpha);
                return col;
            }
            ENDCG
        }
    }
}