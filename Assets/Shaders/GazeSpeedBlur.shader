Shader "Custom/GazeSpeedBlur"
{
    Properties {}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        // --- â° ---
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragH
            #pragma target 3.0
            #pragma multi_compile __ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // x=1/w, y=1/h
            float _GSB_RadiusPx;
            int   _GSB_Samples;
            float _GSB_Enabled;

            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            v2f vert(uint id:SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id<<1)&2, id&2);
                o.pos = float4(uv*2-1, 0, 1);
                o.uv  = uv;
                UNITY_SETUP_INSTANCE_ID(o);
                return o;
            }

            fixed4 fragH(v2f i) : SV_Target
            {
                if (_GSB_Enabled < 0.5 || _GSB_RadiusPx <= 0.01)
                    return tex2D(_MainTex, i.uv);

                int N = max(1, _GSB_Samples);
                float halfN = (N-1)*0.5;
                float2 stepUV = float2(_MainTex_TexelSize.x * _GSB_RadiusPx, 0);

                float wsum = 0;
                float4 acc = 0;
                [loop] for (int k=0; k<32; k++){
                    if (k >= N) break;
                    float t = (k - halfN) / max(halfN, 1e-5);
                    float w = exp(-t*t*2.5);
                    acc += tex2D(_MainTex, i.uv + stepUV * t) * w;
                    wsum += w;
                }
                return acc / max(wsum, 1e-5);
            }
            ENDCG
        }

        // --- èc ---
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragV
            #pragma target 3.0
            #pragma multi_compile __ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _GSB_RadiusPx;
            int   _GSB_Samples;
            float _GSB_Enabled;

            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            v2f vert(uint id:SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id<<1)&2, id&2);
                o.pos = float4(uv*2-1, 0, 1);
                o.uv  = uv;
                UNITY_SETUP_INSTANCE_ID(o);
                return o;
            }

            fixed4 fragV(v2f i) : SV_Target
            {
                if (_GSB_Enabled < 0.5 || _GSB_RadiusPx <= 0.01)
                    return tex2D(_MainTex, i.uv);

                int N = max(1, _GSB_Samples);
                float halfN = (N-1)*0.5;
                float2 stepUV = float2(0, _MainTex_TexelSize.y * _GSB_RadiusPx);

                float wsum = 0;
                float4 acc = 0;
                [loop] for (int k=0; k<32; k++){
                    if (k >= N) break;
                    float t = (k - halfN) / max(halfN, 1e-5);
                    float w = exp(-t*t*2.5);
                    acc += tex2D(_MainTex, i.uv + stepUV * t) * w;
                    wsum += w;
                }
                return acc / max(wsum, 1e-5);
            }
            ENDCG
        }
    }
    Fallback Off
}
