Shader "Custom/ActiveBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 10
        _Pos("Position", Vector) = (0.5, 0.5, 0, 0)
        _Radius("Radius for Blur Deactivation", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        GrabPass { }

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
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;

            float _Blur;
            float2 _Pos; // 座標（スクリーン空間正規化）
            float _Radius; // ぼかし解除の半径

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.grabPos.xy / i.grabPos.w; // 正規化されたスクリーン空間
                screenUV = screenUV * 0.5 + 0.5;

                // 座標からの距離
                float distToMouse = distance(screenUV, _Pos);

                // ぼかし解除の条件
                float blur = distToMouse < _Radius ? 0 : _Blur;
                blur = max(1, blur);

                fixed4 col = (0, 0, 0, 0);
                float weight_total = 0;

                // 高速化のためにループ回数を制限
                for (int x = -10; x <= 10; x++)
                {
                    for (int y = -10; y <= 10; y++)
                    {
                        float2 offset = float2(x, y) * _GrabTexture_TexelSize.xy;
                        float distance_normalized = length(offset / blur);
                        float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                        weight_total += weight;
                        col += tex2D(_GrabTexture, screenUV + offset) * weight;
                    }
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }
    }
}
