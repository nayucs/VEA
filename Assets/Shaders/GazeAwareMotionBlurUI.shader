Shader "Unlit/GazeAwareMotionBlurUI"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _MotionAmp ("Motion Amp (px)", Float) = 8
        _Samples  ("Samples", Float) = 8
        _GazeUV   ("Gaze UV", Vector) = (0.5,0.5,0,0)
        _GazeFalloff ("Gaze Falloff", Float) = 0   // 0=無効。>0で注視中心シャープ化
    }
    SubShader{
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // (1/width,1/height, width, height)

            float2 _MotionDir;   // C#から：物体のスクリーン(=Viewport/UV)速度ベクトル
            float  _MotionAmp;   // ピクセル半径スケール
            float  _Samples;     // サンプル数（8-12推奨）

            float2 _GazeUV;      // 注視UV（任意）
            float  _GazeFalloff; // >0で周辺ほどブラー強化

            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(uint id:SV_VertexID){
                v2f o;
                float2 uv = float2((id<<1)&2, id&2); // 0..1 のフルスクリーン三角
                o.pos = float4(uv*2-1, 0, 1);
                o.uv  = uv;
                return o;
            }

            float4 frag(v2f i):SV_Target{
                // 方向＝速度ベクトルの向き、半径＝速度大きさ×係数
                float2 dir = normalize(_MotionDir + 1e-6);
                float speed = length(_MotionDir);
                float radiusPx = _MotionAmp * saturate(speed * 200.0); // 調整係数

                // テクスチャ座標での1サンプル刻み
                float2 stepUV = dir * (_MainTex_TexelSize.xy * radiusPx);

                // ガウス風ウェイトで線状サンプリング
                float N = max(1.0, _Samples);
                float halfN = (N-1.0)*0.5;
                float wsum = 0;
                float4 acc = 0;
                [loop] for (int k=0; k<32; k++){  // 上限32。実際の回数は_Samplesで制御
                    if (k >= (int)N) break;
                    float t = (k - halfN) / max(halfN, 1e-5); // -1..1
                    float w = exp(-t*t*2.5);                 // ガウス
                    acc += tex2D(_MainTex, i.uv + stepUV * t) * w;
                    wsum += w;
                }
                float4 blurred = acc / max(wsum, 1e-5);

                // 注視中心はシャープ、周辺はブラー（_GazeFalloff>0のときだけ）
                float sharpW = 1.0;
                if (_GazeFalloff > 0.0){
                    float d = length(i.uv - _GazeUV);
                    float s = saturate(1 - pow(d, _GazeFalloff) * 1.2);
                    sharpW = s;
                }

                float4 baseCol = tex2D(_MainTex, i.uv);
                return lerp(blurred, baseCol, sharpW);
            }
            ENDHLSL
        }
    }
}

