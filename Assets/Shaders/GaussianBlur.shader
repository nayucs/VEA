Shader "Custom/GaussianBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 20.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float blur = max(1, _BlurSize); // 最低限のブラーサイズ
                float weight_total = 0.0;
                fixed4 col = fixed4(0, 0, 0, 0);

                // 水平方向のブラー
                [loop]
                for (float x = -blur; x <= blur; x += 1.0)
                {
                    float distance_normalized = abs(x / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    col += tex2D(_MainTex, i.uv + float2(x * _MainTex_TexelSize.x, 0)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float blur = max(1, _BlurSize);
                float weight_total = 0.0;
                fixed4 col = fixed4(0, 0, 0, 0);

                // 垂直方向のブラー
                [loop]
                for (float y = -blur; y <= blur; y += 1.0)
                {
                    float distance_normalized = abs(y / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    col += tex2D(_MainTex, i.uv + float2(0, y * _MainTex_TexelSize.y)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }
    }
}
