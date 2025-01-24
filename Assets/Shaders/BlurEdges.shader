Shader "Custom/BlurEdges"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Screen Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Effect Radius", Float) = 0.5
        _BlurStrength ("Blur Strength", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float4 _MainTex_ST;

            float2 _Center;
            float _Radius;
            float _BlurStrength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float dist = distance(uv, _Center);
                float blur = smoothstep(_Radius, _Radius + 0.1, dist) * _BlurStrength;

                fixed4 col = tex2D(_MainTex, uv);
                if (blur > 0.0)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            col += tex2D(_MainTex, uv + float2(x, y) * 0.005 * blur);
                        }
                    }
                    col /= 9.0;
                }
                return col;
            }
            ENDCG
        }
    }
}
