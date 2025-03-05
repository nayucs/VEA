Shader "Custom/GazeBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 50.0
        _GazeUV("Gaze UV", Vector) = (0.5, 0.5, 0, 0) // 視線UV座標
        _ClearRadius("Clear Radius", Float) = 0.1 // 視線周辺の透明領域
        _AspectRatio("Aspect Ratio (width/height)", Float) = 1.77778 // アスペクト比
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
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
            float _BlurSize;
            float2 _GazeUV;
            float _ClearRadius;
            float _AspectRatio;  // 横/縦アスペクト比

            v2f vert(appdata_t v)
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
                // アスペクト比補正を考慮した距離計算
                float2 aspectCorrectedUV = float2((i.uv.x - _GazeUV.x) * _AspectRatio, i.uv.y - _GazeUV.y);
                float dist = length(aspectCorrectedUV);

                // 視線周辺（一定範囲内）は透明にする
                if (dist < _ClearRadius)
                {
                    discard;
                }

                float blur = max(1, _BlurSize);
                float weight_total = 0.0;
                fixed4 col = fixed4(0, 0, 0, 0);

                [loop]
                for (float x = -blur; x <= blur; x += 0.5)
                {
                    float weight = exp(-0.5 * pow(abs(x / blur), 2) * 2.0);
                    weight_total += weight;
                    col += tex2D(_MainTex, i.uv + float2(x * _MainTex_TexelSize.x, 0)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }
    }
}
