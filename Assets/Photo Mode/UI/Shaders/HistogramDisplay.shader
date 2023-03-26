Shader "UI/Histogram"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma multi_compile HISTOGRAM_COLOR HISTOGRAM_LUMINANCE

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            uniform StructuredBuffer<uint4> histogramBuffer : register(t1);
            uniform StructuredBuffer<uint> histogramMaxBuffer : register(t2);

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;
            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float4 normalizeHistogram(float4 hist) {
                return hist / (float)histogramMaxBuffer[0];
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                uint id = (uint)(IN.texcoord.x * 255);
                uint4 histColor = histogramBuffer[id];
                
                float4 histogram = float4(histColor.r, histColor.g, histColor.b, histColor.a);
                histogram = normalizeHistogram(histogram);

                #ifdef HISTOGRAM_COLOR
                    float4 color = float4(0,0,0,0);                
                    if (IN.texcoord.y + histogram.r > 1) {
                        color.r = histogram.r;
                        color.a = 1;
                    }
                    if (IN.texcoord.y + histogram.g > 1) {
                        color.g = histogram.g;
                        color.a = 1;
                    }
                    if (IN.texcoord.y + histogram.b > 1) {
                        color.b = histogram.b;
                        color.a = 1;
                    }
                #endif

                #ifdef HISTOGRAM_LUMINANCE
                    float4 color = float4(0,0,0,0);                
                    if (IN.texcoord.y + histogram.a > 1) {
                        color.rgb = 1;
                        color.a = 1;
                    }
                    color.rgb = histogram.a;
                    //color.a = 1;
                #endif

                color *= IN.color;
                //color.rgb = IN.texcoord.x;
                //color.a = 1;
                color.rgb = GammaToLinearSpace(color.rgb);

                return color;
            }
        ENDCG
        }
    }
}