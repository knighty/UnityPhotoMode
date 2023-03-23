Shader "Hidden/RenderAccumulatedFrame"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "" {}
    }

    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        struct AttributesDefault2
        {
            float3 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        VaryingsDefault VertDefault2(AttributesDefault2 v)
        {
            VaryingsDefault o;
            o.vertex = float4(v.vertex.xy * 2 - 1, 0.0, 1.0);
            o.texcoord = v.uv;

        #if UNITY_UV_STARTS_AT_TOP
            o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif

            o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

            return o;
        }

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _TotalAccumulations;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord) / (_TotalAccumulations);
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault2
                #pragma fragment Frag

            ENDHLSL
        }
    }
}