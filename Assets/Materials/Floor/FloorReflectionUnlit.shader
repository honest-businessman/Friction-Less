Shader "Custom/FloorReflectionUnlit"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _FloorMinBounds ("Floor Min Bounds", Vector) = (0,0,0,0)
        _FloorMaxBounds ("Floor Max Bounds", Vector) = (1,1,0,0)
        _ReflectionIntensity ("Reflection Intensity", Range(0,3)) = 1.0
        _EdgeFadeDistance ("Edge Fade Distance", Float) = 0.5
        _BaseColor ("Base Floor Color", Color) = (0,0,0,1)
        _TileSize ("Tile Size", Float) = 1.0
        _SeamSharpness ("Tile Seam Sharpness", Float) = 10.0
        _SeamDistortion ("Seam Distortion Strength", Float) = 0.002
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
            float _ReflectionIntensity;
            float _EdgeFadeDistance;
            fixed4 _BaseColor;
            float _TileSize;
            float _SeamSharpness;
            float _SeamDistortion;

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
                float3 baseCol = _BaseColor.rgb;

                float2 floorSize = _FloorMaxBounds.xy - _FloorMinBounds.xy;
                float2 normPos = (IN.worldPos.xy - _FloorMinBounds.xy) / floorSize;
                float2 reflUV = saturate(normPos);

                // --- Procedural tile seam distortion ---
                float2 tileCoord = IN.worldPos.xy / _TileSize;
                float2 tileFrac = frac(tileCoord);
                float2 seamMask = smoothstep(0.0, 1.0 / _SeamSharpness, min(tileFrac, 1.0 - tileFrac));
                float seamEffect = (1.0 - min(seamMask.x, seamMask.y));
                float2 offset = seamEffect * _SeamDistortion;

                reflUV += offset;

                fixed4 reflectionCol = tex2D(_ReflectionTex, reflUV);

                // Fresnel effect
                float3 floorNormal = float3(0,0,1);
                float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), floorNormal)), 3);

                // Edge fade
                float edgeFade = EdgeFade(IN.worldPos.xy);

                // Final blend
                float reflectionAmount = _ReflectionIntensity * fresnel * edgeFade;
                float3 finalColor = lerp(baseCol, reflectionCol.rgb, reflectionAmount);

                return fixed4(finalColor, 1);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}
