Shader "Custom/BothGazeBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 10.0
        _GazeUV_L("Gaze UV Left", Vector) = (0.5, 0.5, 0, 0)
        _GazeUV_R("Gaze UV Right", Vector) = (0.5, 0.5, 0, 0)
        _ClearRadius("Clear Radius", Float) = 0.1
        _CurrentEye("Current Eye (0=Left, 1=Right)", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile_instancing
            #pragma multi_compile _ MULTIVIEW_ON

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
            float2 _GazeUV_L;
            float2 _GazeUV_R;
            float _ClearRadius;
            float _CurrentEye; // 0=左目, 1=右目

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 現在の目に対応するGazeUVを取得
                float2 gazeUV = (_CurrentEye < 0.5) ? _GazeUV_L : _GazeUV_R;

                // 視線位置からの距離を計算
                float dist = distance(i.uv, gazeUV);

                // 視線位置近辺は透明に
                if (dist < _ClearRadius)
                {
                    discard;
                }

                // 通常の水平ガウシアンブラー
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
