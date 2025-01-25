Shader "Custom/MouseMaskWithBlur" {
    Properties {
        _MaskRadius ("Mask Radius", Float) = 0.2
        _MousePos ("Mouse Position", Vector) = (0.5, 0.5, 0, 0)
        _Blur ("Blur", Float) = 10
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        GrabPass {
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _MaskRadius;
            float4 _MousePos;
            float _Blur;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 screenUV = i.grabPos.xy / i.grabPos.w; // 正規化されたスクリーン座標
                float2 mousePos = float2(_MousePos.x, 1.0 - _MousePos.y); // 正規化されたマウス位置（Y軸反転）

                // マウス位置との距離を計算
                float dist = length(screenUV - mousePos);

                // マスク半径内のピクセルを除外
                if (dist < _MaskRadius) {
                    discard;
                }

                // ぼかしの実装
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0, 0, 0, 0);
                float weight_total = 0;

                [loop]
                for (float x = -blur; x <= blur; x += 1) {
                    float distance_normalized = abs(x / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(x * _GrabTexture_TexelSize.x, 0, 0, 0)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }
    }
}
