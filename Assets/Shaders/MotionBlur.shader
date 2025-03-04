Shader "Custom/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Strength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            sampler2D _PrevFrameTex;
            float _BlurAmount;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 現在のフレームの色
                fixed4 currentColor = tex2D(_MainTex, i.uv);
                // 1フレーム前の映像を取得
                fixed4 prevColor = tex2D(_PrevFrameTex, i.uv);
                // 2つをブレンドしてブラー効果を作成
                return lerp(currentColor, prevColor, _BlurAmount);
            }
            ENDCG
        }
    }
}
