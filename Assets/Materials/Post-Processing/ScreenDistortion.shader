Shader "Custom/ScreenDistortion"
{
    Properties
    {
        _MainTex("Base (Camera Render)", 2D) = "white" {}
        _Distortion("Barrel Distortion", Range(0,1)) = 0.2
        _Vignette("Vignette Strength", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Distortion;
            float _Vignette;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 centered = uv - 0.5;
                float r2 = dot(centered, centered);

                // barrel distortion
                uv = 0.5 + centered * (1.0 + r2 * _Distortion);

                // clamp to avoid black edges
                uv = saturate(uv);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // vignette
                float vignette = 1.0 - _Vignette * dot(centered, centered) * 2.0;
                col.rgb *= saturate(vignette);

                return col;
            }
            ENDHLSL
        }
    }
}
