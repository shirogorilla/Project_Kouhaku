Shader "Custom/URP_Snow_Lit_ShadowCaster"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _SnowAmount ("Snow Amount", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        // =============================
        // ShadowCaster Pass（影を落とす用）
        // =============================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_shadow
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag_shadow() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        // =============================
        // ForwardLit Pass（通常描画）
        // =============================
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog   // ← Fog対応を有効化

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float  fogFactor  : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _SnowAmount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                // Fog計算
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).rgb * _BaseColor.rgb;
                float upFactor = saturate(dot(normalize(IN.normalWS), float3(0,1,0)));
                float3 snowColor = float3(1,1,1);
                float3 finalColor = lerp(baseCol, snowColor, upFactor * _SnowAmount);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo     = finalColor;
                surfaceData.alpha      = 1.0;
                surfaceData.normalTS   = float3(0,0,1);
                surfaceData.emission   = float3(0,0,0);
                surfaceData.specular   = float3(0.04,0.04,0.04);
                surfaceData.occlusion  = 1.0;
                surfaceData.metallic   = 0.0;
                surfaceData.smoothness = 0.2;

                InputData inputData = (InputData)0;
                inputData.positionWS      = IN.positionWS;
                inputData.normalWS        = normalize(IN.normalWS);
                inputData.viewDirectionWS = normalize(_WorldSpaceCameraPos - IN.positionWS);
                inputData.shadowCoord     = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord        = IN.fogFactor;
                inputData.vertexLighting  = 0;
                inputData.bakedGI         = 0;

                // PBR計算
                half4 col = UniversalFragmentPBR(inputData, surfaceData);

                // Fog適用
                col.rgb = MixFog(col.rgb, inputData.fogCoord);

                return col;
            }

            ENDHLSL
        }
    }
}
