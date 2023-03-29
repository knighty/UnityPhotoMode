Shader "Photo Mode/SampleDepth"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }

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
                o.uv = o.vertex.xy / o.vertex.w * 0.5 + 0.5;
                #if UNITY_UV_STARTS_AT_TOP
                    //o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif

                return o;
            }

            sampler2D _CameraDepthTexture;
            float4 _MousePosition = float4(0.5, 0.5, 0, 0);

            fixed4 frag (v2f i) : SV_Target
            {
                float rawZ = tex2D(_CameraDepthTexture, _MousePosition);
                float sceneZ = LinearEyeDepth(rawZ);

                //float depth = tex2D(_CameraDepthTexture, i.uv).r;
                return float4(sceneZ.xxx, 1);
                //return float4(i.uv, 1, 1);
            }
            ENDCG
        }
    }
}
