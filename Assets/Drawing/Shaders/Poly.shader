Shader "Unlit/Poly"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        Cull Back
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
                float4 layer : TEXCOORD1;
            };

            struct v2f
            {
                float4 layer : TEXCOORD1;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 layerParams[10];
            float4 layerParams2[10];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.layer = v.layer;
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

            float shadow(float u, float v, float lineLength, float shadowWidth)
            {
                float d = abs(v);
                if (u < 0 || u > lineLength) {
                    if (u > 0) {
                        u -= lineLength;
                    }
                    //d = cap(u, v, lineWidth); 
                }
                d = max(0, shadowWidth - d) / shadowWidth;
                return d;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = float4(0,0,0,0);
                
                float antialias = 1;//sqrt(2) / 2;

                float2 uv = i.uv;
                float lineLength = i.uv.w;
                float lineWidth = i.uv.z;

                float fillAlpha = lineAlpha(uv.x, uv.y, lineLength, lineWidth, antialias);
                float4 fill = float4(i.color.rgb, i.color.a * fillAlpha);

                int layer = floor(i.layer.x + 0.5);
                float4 params = layerParams[layer];
                float4 params2 = layerParams2[layer];

                // Stroke
                if (params.x == 1) {
                    float strokeWidth = params.y;
                    float strokeAlpha = lineAlpha(uv.x, uv.y, lineLength, strokeWidth / 2, antialias) * params2.a;
                    col.rgb =  params2.rgb * i.color.rgb;
                    col.a = strokeAlpha;
                    //col = params2 * i.color;
                    //col = float4(strokeAlpha, 0, 0, 1);
                } 
                 // Stroke
                else if (params.x == 2) {
                    float strokeWidth = params.y;
                    float strokeAlpha = shadow(uv.x, uv.y, lineLength, strokeWidth / 2) * params2.a;
                    col.rgb =  params2.rgb * i.color.rgb;
                    col.a = strokeAlpha;
                    //col = params2 * i.color;
                    //col = float4(strokeAlpha, 0, 0, 1);
                } 
                // Fill
                else {
                    float alpha = lineAlpha(uv.x, uv.y, lineLength, 0, antialias);
                    col.rgb = params2.rgb * i.color.rgb;
                    col.a = (params2 * i.color).a * alpha;
                }
                
                //col = float4(params2.rgb, 1);
                //col.rgb = strokeAlpha;
                //col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}
