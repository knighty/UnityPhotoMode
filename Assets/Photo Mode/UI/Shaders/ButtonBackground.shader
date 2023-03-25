Shader "UI/ButtonBackground"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Noise.cginc"

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
                float4 texcoord2  : TEXCOORD2;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
			float _UnscaledTime;
			float2 _MousePosition;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.texcoord2 = float4(v.vertex.x, v.vertex.y, OUT.vertex.x * 0.5 + 0.5, OUT.vertex.y * 0.5 + 0.5);

                OUT.color = v.color * _Color;
                return OUT;
            }

            float calculateNoise(float2 uv, float t) {

            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float state = IN.color.r;
                float selectedState = IN.color.g;
                float3 orange = pow(float3(239, 171, 40) / 255, 2.2);

                float mask = tex2D(_MainTex, IN.texcoord).a;
                float3 hazeColor = orange * 1.5;
                float pulse = 0.5 + 0.5 * sin(_Time.y * 4);
                float3 backgroundColor = orange * (0.6 + 0.4 * pulse) * 1 * (1 - selectedState * 0.5);// pow(float3(30, 30, 30) / 255, 2.2);
                
                float2 uv = IN.texcoord2.xy / 128;

                //color.rgb = snoise(float3(uv, _Time.y)) * 0.5f + 0.5f;
                float noise = perlin(float3(uv, _Time.y * 0.1), 6, 0.6) * 0.5 + 0.5;

				
                float4 color = float4(1,1,1,1);

                float animated = smoothstep(0.2 + 0.6 * noise, 1, pow(0.35 + 0.65 * mask, 1));
                animated = smoothstep(0, 1, animated);
                float3 animatedColor = lerp(backgroundColor, hazeColor, animated);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb = lerp(float3(0.1, 0.1, 0.1), animatedColor, state);


                return color;
            }
        ENDCG
        }
    }
}