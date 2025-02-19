Shader "Custom/FoveatedBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GazePos ("Gaze Position", Vector) = (0.5, 0.5, 0, 0)
        _BlurRadius ("Blur Radius", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _GazePos; // 視線の中心 (正規化座標)
            float _BlurRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 blur(float2 uv)
            {
                float2 offset = float2(0.005, 0.005);
                half4 color = tex2D(_MainTex, uv);
                color += tex2D(_MainTex, uv + offset);
                color += tex2D(_MainTex, uv - offset);
                return color / 3.0;
            }

            half4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _GazePos.xy); // 視線中心との距離
                half4 color = tex2D(_MainTex, i.uv);
                
                if (dist > _BlurRadius) {
                    color = blur(i.uv); // 外側をぼかす
                }
                return color;
            }
            ENDCG
        }
    }
}
