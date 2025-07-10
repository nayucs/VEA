Shader "Custom/GazeBlurDistance"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 50.0
        _GazeUV("Gaze UV", Vector) = (0.5, 0.5, 0, 0)
        _ClearRadius("Clear Radius", Float) = 0.1
        _AspectRatio("Aspect Ratio (width/height)", Float) = 1.77778
        _Exponent("Blur Growth Exponent", Float) = 2.0 // 指数係数
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
            float _AspectRatio;
            float _Exponent;

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

                // 視線周辺（一定範囲内）はそのまま
                if (dist < _ClearRadius)
                {
                    return tex2D(_MainTex, i.uv);
                }

                // 距離に応じてブラー半径を指数関数で変化
                float linearFactor = saturate((dist - _ClearRadius) / (0.5 - _ClearRadius));
                float blurFactor = pow(linearFactor, _Exponent);  // 指数関数
                float dynamicBlurSize = _BlurSize * blurFactor;

                float2 offset = float2(dynamicBlurSize / 1024.0, dynamicBlurSize / 1024.0);

                fixed4 col = tex2D(_MainTex, i.uv);
                col += tex2D(_MainTex, i.uv + offset);
                col += tex2D(_MainTex, i.uv - offset);
                col += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y));
                col += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y));
                col /= 5.0;

                return col;
            }
            ENDCG
        }
    }
}
