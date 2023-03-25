//https://github.com/remibodin/Unity3D-Blur/blob/master/UnityProject/Assets/Blur/GaussianBlur/Shaders/TwoPassGaussianBlur.shader

Shader "hidden/two_pass_gaussian_blur" 
{ 
	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma multi_compile LITTLE_KERNEL MEDIUM_KERNEL BIG_KERNEL
	#include "GaussianBlur.cginc"
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	uniform sampler2D _GrabTexture;
	uniform float4 _GrabTexture_TexelSize;
	uniform float _Sigma;

	struct appdata_t
    {
        float4 vertex   : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 vertex   : SV_POSITION;
        float4 worldPosition : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata_t v)
    {
        v2f OUT;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        OUT.worldPosition = v.vertex;
        OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
        return OUT;
    }

	
	float4 frag_horizontal (v2f_img i) : COLOR
	{
		pixel_info pinfo;
		pinfo.tex = _MainTex;
		pinfo.uv = i.uv;
		pinfo.texelSize = _MainTex_TexelSize;
		return GaussianBlur(pinfo, _Sigma, float2(1,0));
	}
	
	float4 frag_vertical (v2f_img i) : COLOR
	{				
		pixel_info pinfo;
		pinfo.tex = _GrabTexture;
		pinfo.uv = i.uv;
		pinfo.texelSize = _GrabTexture_TexelSize;
		return GaussianBlur(pinfo, _Sigma, float2(0,1));
	}
	ENDCG
	
	Properties 
	{ 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue" = "Overlay" }
		Lighting Off 
		Cull Off 
		ZWrite Off 
		ZTest Always 

	    Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag_horizontal
			ENDCG
		}
		
		GrabPass{ }
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag_vertical
			ENDCG
		}
	}
}