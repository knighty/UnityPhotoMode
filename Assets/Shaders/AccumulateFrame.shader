Shader "Shaders/AccumulateFrame"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "" {}
    }

    CGINCLUDE
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
            return o;
        }

        sampler2D _MainTex;
        float _Magnitude;

        float4 frag(v2f i) : SV_Target
        {
            float4 color = tex2D(_MainTex, i.uv);
            color.rgb = color.rgb * _Magnitude;
            return color;
        }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend One One

        Pass
        {
            CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

            ENDCG
        }

        Pass
        {
            CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

            ENDCG
        }
    }
}