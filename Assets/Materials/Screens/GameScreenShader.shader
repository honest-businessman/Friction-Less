Shader "Unlit/RenderTextureAspectWithBarsAndScale"
{
    Properties
    {
        _MainTex("Render Texture", 2D) = "white" {}
        _TexAspect("Texture Aspect", Float) = 1.777
        _ScreenAspect("Screen Aspect", Float) = 1.0
        _Scale("RenderTexture Scale", Range(0.1, 5.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite On
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _TexAspect;
            float _ScreenAspect;
            float _Scale;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float texAspect = _TexAspect;
                float screenAspect = _ScreenAspect;

                float scaleX = 1.0;
                float scaleY = 1.0;

                // Compute aspect-fit scale
                if (screenAspect > texAspect)
                {
                    scaleX = texAspect / screenAspect;
                }
                else
                {
                    scaleY = screenAspect / texAspect;
                }

                // Apply uniform scaling (divide to make <1 shrink)
                scaleX /= _Scale;
                scaleY /= _Scale;

                // Center UVs
                float2 uv = (i.uv - 0.5) * float2(scaleX, scaleY) + 0.5;

                // Black bars outside scaled region
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return float4(0,0,0,1);

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
