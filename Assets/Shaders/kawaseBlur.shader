Shader "Custom/kawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Cull Off ZWrite Off ZTest Always

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,0);
                float2 offset = _BlurSize * _MainTex_TexelSize.xy;
                
                col += tex2D(_MainTex, i.uv + float2(-offset.x, -offset.y)) * 0.25;
                col += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)) * 0.25;
                col += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y)) * 0.25;
                col += tex2D(_MainTex, i.uv + float2(offset.x, offset.y)) * 0.25;
                
                return col;
            }
            ENDHLSL
        }
    }
}
