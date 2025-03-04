Shader "Custom/GazeMask"
{
    Properties
    {
        _Radius ("Transparent Radius", Float) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        ZWrite Off
        Cull Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float3 _GazeHitPosition;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _GazeHitPosition);
                
                // 指定範囲内なら描画をスキップ（透明化）
                if (dist < _Radius)
                {
                    discard;
                }

                // 範囲外は黒で塗る
                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
