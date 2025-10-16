Shader "Custom/GlossyFloorWithNormals"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _SeamNormalMap ("Tile Seam Normal Map", 2D) = "bump" {}

        _FloorMinBounds ("Floor Min Bounds", Vector) = (0,0,0,0)
        _FloorMaxBounds ("Floor Max Bounds", Vector) = (1,1,0,0)

        _ReflectionOffset ("Reflection UV Offset (X,Y)", Vector) = (0, 0, 0, 0)
        _BaseColor ("Base Floor Color", Color) = (0.05, 0.05, 0.05, 1)
        _ReflectionIntensity ("Reflection Intensity", Float) = 1.0
        _EdgeFadeDistance ("Edge Fade Distance", Float) = 0.5

        _SeamNormalStrength ("Seam Normal Strength", Float) = 0.01
        _SeamThreshold ("Seam Highlight Threshold", Range(0,2)) = 0.3
        _SeamReflectionBrightness ("Seam Reflection Brightness", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _ReflectionTex;
            sampler2D _SeamNormalMap;
            sampler2D _ScratchMap;

            float4 _FloorMinBounds;
            float4 _FloorMaxBounds;

            float4 _ReflectionOffset;
            float _ReflectionIntensity;
            float _EdgeFadeDistance;
            fixed4 _BaseColor;

            float _SeamNormalStrength;
            float _SeamThreshold;
            float _SeamReflectionBrightness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.uv = v.uv;
                return o;
            }

            float EdgeFade(float2 pos)
            {
                float2 distMin = pos - _FloorMinBounds.xy;
                float2 distMax = _FloorMaxBounds.xy - pos;
                float minDist = min(min(distMin.x, distMin.y), min(distMax.x, distMax.y));
                return saturate(minDist / _EdgeFadeDistance);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float3 baseCol = _BaseColor.rgb;

                // Normalized floor-space position (used for reflection UVs)
                float2 floorSize = _FloorMaxBounds.xy - _FloorMinBounds.xy;
                float2 normPos = (IN.worldPos.xy - _FloorMinBounds.xy) / floorSize;
                float2 baseReflUV = saturate(normPos + _ReflectionOffset.xy);

                // Sample base reflection at unmodified UV
                fixed4 baseReflectionCol = tex2D(_ReflectionTex, baseReflUV);

                // Seam normal sampling
                float3 seamNormal = UnpackNormal(tex2D(_SeamNormalMap, IN.uv));
                float seamMag = length(seamNormal.xy);
                // Normalize seamMag to [0,1] based on threshold
                float seamStrength = saturate((seamMag - _SeamThreshold) / max(0.0001, 1.0 - _SeamThreshold));
                float2 seamOffset = seamNormal.xy * _SeamNormalStrength;

                fixed4 seamReflectionCol = tex2D(_ReflectionTex, saturate(baseReflUV + seamOffset)) * _SeamReflectionBrightness * seamStrength;

                // Combine reflections weighted
                float totalWeight = 1.0 + (_SeamReflectionBrightness * seamStrength) + 1e-5;

                float3 combinedReflection = (
                    baseReflectionCol.rgb * 1.0 +
                    seamReflectionCol.rgb
                ) / totalWeight;

                // Fresnel reflection falloff
                float3 floorNormalVec = float3(0,0,1);
                float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), floorNormalVec)), 3);

                // Edge fade
                float edgeFade = EdgeFade(IN.worldPos.xy);

                // Final reflection amount
                float reflectionAmount = _ReflectionIntensity * fresnel * edgeFade;

                // Final color lerp between base color and combined reflection
                float3 finalColor = lerp(baseCol, combinedReflection, reflectionAmount);

                return fixed4(finalColor, 1);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}
