Shader "Custom/FloorReflectionUnlit"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _FloorMinBounds ("Floor Min Bounds", Vector) = (0,0,0,0)
        _FloorMaxBounds ("Floor Max Bounds", Vector) = (1,1,0,0)
        _DistortionStrength ("Distortion Strength", Float) = 0.05
        _ReflectionIntensity ("Reflection Intensity", Float) = 0.5
        _EdgeFadeDistance ("Edge Fade Distance", Float) = 0.5
        _BaseColor ("Base Floor Color", Color) = (0,0,0,1)
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
            float4 _FloorMinBounds;
            float4 _FloorMaxBounds;
            float _DistortionStrength;
            float _ReflectionIntensity;
            float _EdgeFadeDistance;
            fixed4 _BaseColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
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

                // Get world position
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Calculate view direction
                float3 viewPos = _WorldSpaceCameraPos;
                o.viewDir = normalize(viewPos - o.worldPos);

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
                // Base floor color
                float3 baseCol = _BaseColor.rgb;

                // Map worldPos to reflection UVs (normalized inside floor bounds)
                float2 floorSize = _FloorMaxBounds.xy - _FloorMinBounds.xy;
                float2 normPos = (IN.worldPos.xy - _FloorMinBounds.xy) / floorSize;

                // Simple procedural distortion using sine waves
                float2 distortion = float2(
                    sin(normPos.y * 20.0) * 0.01,
                    cos(normPos.x * 20.0) * 0.01
                ) * _DistortionStrength;

                float2 reflUV = clamp(normPos + distortion, 0, 1);

                // Sample reflection texture with distortion
                fixed4 reflectionCol = tex2D(_ReflectionTex, reflUV);

                // Fresnel effect (view angle with upward normal)
                float3 floorNormal = float3(0,0,1);
                float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), floorNormal)), 3);

                // Edge fade
                float edgeFade = EdgeFade(IN.worldPos.xy);

                // Combine reflection with base color
                float reflectionAmount = _ReflectionIntensity * fresnel * edgeFade;
                float3 finalColor = lerp(baseCol, reflectionCol.rgb, reflectionAmount);

                return fixed4(finalColor, 1);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}
