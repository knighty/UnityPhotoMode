Shader "Unlit/Line"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float cap( float u, float v, float t )
            {
                return sqrt(u*u+v*v);
            }

            float lineAlpha(float u, float v, float lineLength, float lineWidth, float antialias)
            {
                float d = abs(v);
                if (u < 0 || u > lineLength) {
                    if (u > 0) {
                        u -= lineLength;
                    }
                    d = cap(u, v, lineWidth); 
                }
                d -= lineWidth;
                d /= antialias;
                d = d < 0 ? 1 : exp(-d*d);
                return d;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float antialias = sqrt(2);

                float2 uv = i.uv;
                float lineLength = i.uv.w;
                float lineWidth = i.uv.z;
                // sample the texture
                float d = abs(uv.y);
                float t = lineWidth - antialias;

                float4 col = i.color;
                col.a *= lineAlpha(uv.x, uv.y, lineLength, lineWidth, antialias);

                float strokeWidth = 2;
                //col.rgb = LinearToGammaSpace(col.rgb);
                //col.rgb *= 1 - smoothstep(-lineWidth, 0, d);
                //col.rgb *= 1 - smoothstep(-strokeWidth-antialias, -strokeWidth, d) * 0.6;
                //col.rgb = GammaToLinearSpace(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}
