Shader "UI/Window"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_DataTex ("Data Texture", 2D) = "white" {}
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
                float4 worldPosition : TEXCOORD1;
                float4 texcoord2  : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
			sampler2D _DataTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float4 _DataTex_ST;
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

            fixed4 frag(v2f IN) : SV_Target
            {
				float2 uv = IN.texcoord.xy;

                float4 textureColor = tex2D(_DataTex, uv);
				float4 backgroundColor = float4(pow(35.0/256.0, 2.2).xxx, 1);

				float t = _UnscaledTime * 0.35;
				float persistence = 0.7;
				float noiseValue = perlin(float3(uv * 5, t * 0.15), 6, 0.7);

                float mouseMod = 1 - saturate(5 * length((_MousePosition.xy - IN.texcoord2.zw) * float2(2,1)));
				//noiseValue += 1 - saturate(5 * length((_MousePosition.xy - IN.texcoord2.zw) * float2(2,1)));

				float offset = 0.2;
				//float animated = smoothstep(offset, offset + 0.03, smoothstep(-1, 1, noiseValue) * saturate(textureColor.r));
                noiseValue = noiseValue * 0.5 + 0.5;// smoothstep(-1, 1, noiseValue);
                float animated = smoothstep(noiseValue, noiseValue + 0.02, textureColor.r * (1.3 + mouseMod));
				backgroundColor.rgb += pow(animated * 0.1, 2.2);

				float4 color = backgroundColor * IN.color;

				#ifdef UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
                #endif

                //color.rgb = smoothstep(-1, 1, noiseValue);

                return color;
            }
        ENDCG
        }
    }
}