Shader "Hidden/Clarity"
{
    Properties
    {
        _BlurTex ("Texture", 2D) = "white" {}
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Overlay+10" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                #if UNITY_UV_STARTS_AT_TOP
                    //o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif

                return o;
            }

            sampler2D _MainTex;
            sampler2D _BlurTex;
            float _Clarity;
            float _Vibrance;

            half3 AdjustContrastCurve(half3 color, half contrast) {
                return pow(abs(color * 2 - 1), 1 / max(contrast, 0.0001)) * sign(color - 0.5) * 0.5 + 0.5;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half3 col = LinearToGammaSpace(tex2D(_MainTex, i.uv));
                half3 blurredCol = LinearToGammaSpace(tex2D(_BlurTex, i.uv));

                float colLuminance = col.r * 0.39 + col.g * 0.5 + col.b * 0.11;
                float blurLuminance = blurredCol.r * 0.39 + blurredCol.g * 0.5 + blurredCol.b * 0.11;

                float3 delta = col - blurredCol;
                float deltaLuminance = colLuminance - blurLuminance;
                float absLum = sqrt(abs(deltaLuminance));
                //col.rgb += delta * 0.2;

                //col.rgb *= (1 + (deltaLuminance > 0 ? absLum : -absLum) * _Clarity);
                //col.rgb *= (1 + deltaLuminance * _Clarity);

                //col.rgb = lerp(float3(0.5, 0.5, 0.5), col.rgb, 1 + _Clarity * 0.1);
                col = AdjustContrastCurve(col, _Clarity * 0.5 + 1);

                float v = pow(colLuminance, 0.5);
                col = lerp(colLuminance.xxx, col, 1 + v * _Vibrance);

                col = GammaToLinearSpace(col);

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
