Shader "Custom/GlossyFloorWithNormalsUnifiedUV"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _SeamNormalMap ("Tile Seam Normal Map", 2D) = "bump" {}
        _BaseTex ("Base Texture", 2D) = "white" {}

        _FloorMinBounds ("Floor Min Bounds", Vector) = (0, 0, 0, 0)
        _FloorMaxBounds ("Floor Max Bounds", Vector) = (10, 10, 0, 0)

        _ReflectionOffset ("Reflection UV Offset (X,Y)", Vector) = (0, 0, 0, 0)
        _BaseColor ("Base Floor Color", Color) = (0.05, 0.05, 0.05, 1)
        _ReflectionIntensity ("Reflection Intensity", Float) = 1.0
        _EdgeFadeDistance ("Edge Fade Distance", Float) = 0.5

        _SeamNormalStrength ("Seam Normal Strength", Float) = 0.01
        _SeamThreshold ("Seam Highlight Threshold", Range(0, 2)) = 0.3
        _SeamReflectionBrightness ("Seam Reflection Brightness", Float) = 1.0

        _BaseTexTiling ("Base Texture Tiling (per meter)", Float) = 1.0
        _BaseTexOffset ("Base Texture Offset", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _ReflectionTex;
            sampler2D _SeamNormalMap;
            sampler2D _BaseTex;

            float4 _FloorMinBounds;
            float4 _FloorMaxBounds;
            float4 _ReflectionOffset;
            float4 _BaseColor;

            float _ReflectionIntensity;
            float _EdgeFadeDistance;

            float _SeamNormalStrength;
            float _SeamThreshold;
            float _SeamReflectionBrightness;

            float _BaseTexTiling;
            float4 _BaseTexOffset;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
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
                // Unified world-space UVs for base and normal maps
                float2 baseUV = (IN.worldPos.xy - _FloorMinBounds.xy) * _BaseTexTiling + _BaseTexOffset.xy;
                baseUV = frac(baseUV);

                // Base texture color
                float3 baseTexCol = tex2D(_BaseTex, baseUV).rgb;
                float3 baseCol = _BaseColor.rgb * baseTexCol;

                // Reflection UVs
                float2 floorSize = _FloorMaxBounds.xy - _FloorMinBounds.xy;
                float2 normPos = (IN.worldPos.xy - _FloorMinBounds.xy) / floorSize;
                float2 baseReflUV = saturate(normPos + _ReflectionOffset.xy);

                // Seam normal map affects reflection sampling
                float3 seamNormal = UnpackNormal(tex2D(_SeamNormalMap, baseUV));
                float seamMag = length(seamNormal.xy);
                float seamStrength = saturate((seamMag - _SeamThreshold) / max(0.0001, 1.0 - _SeamThreshold));
                float2 seamOffset = seamNormal.xy * _SeamNormalStrength;

                // Only modified reflection used
                fixed4 reflectionCol =
                    tex2D(_ReflectionTex, saturate(baseReflUV + seamOffset)) *
                    _SeamReflectionBrightness * seamStrength;

                // Fresnel and edge fade
                float3 floorNormalVec = float3(0, 0, 1);
                float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), floorNormalVec)), 3);
                float edgeFade = EdgeFade(IN.worldPos.xy);
                float reflectionAmount = _ReflectionIntensity * fresnel * edgeFade;

                // Mix base with modified reflection only
                float3 finalColor = lerp(baseCol, reflectionCol.rgb, reflectionAmount);
                return fixed4(finalColor, 1);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
