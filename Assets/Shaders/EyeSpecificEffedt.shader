// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/EyeSpecificEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                int eyeIndex : TEXCOORD1; // 目の識別用
            };

            sampler2D _MainTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.eyeIndex = unity_StereoEyeIndex; // 目のインデックスを渡す
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 目ごとに異なるエフェクトを適用
                if (i.eyeIndex == 0)  // Left Eye
                {
                    col.rgb *= float3(0.5, 0.8, 1.0); // 青っぽく
                }
                else if (i.eyeIndex == 1) // Right Eye
                {
                    col.rgb *= float3(1.0, 0.6, 0.6); // 赤っぽく
                }

                return col;
            }
            ENDCG
        }
    }
}
