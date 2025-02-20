Shader "Custom/VRBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur ("Blur Amount", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Blur;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float blur = max(1, _Blur);
                fixed4 col = fixed4(0, 0, 0, 0);
                float weight_total = 0;

                // êÖïΩï˚å¸ÉuÉâÅ[
                [loop]
                for (float x = -blur; x <= blur; x += 1)
                {
                    float weight = exp(-0.5 * pow(abs(x / blur), 2) * 5.0);
                    weight_total += weight;
                    float2 sampleUV = i.uv + float2(x * _MainTex_TexelSize.x, 0);
                    col += tex2D(_MainTex, sampleUV) * weight;
                }
                col /= weight_total;

                return col;
            }
            ENDCG
        }
    }
}
