Shader "Custom/ToonShaderWithShadows"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _ShadowColor;
            float _ShadowStrength;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // 조명 계산
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, i.normal));
                float ndotl = max(0, dot(worldNormal, lightDir));


                // 셀 셰이딩
                fixed4 finalColor = texColor * (ndotl > 0.5 ? 1 : 0.5);
                
                // 그림자 추가
                finalColor = lerp(finalColor, _ShadowColor, (1 - ndotl) * _ShadowStrength);



                return finalColor;
            }
            ENDCG
        }
    }
}
