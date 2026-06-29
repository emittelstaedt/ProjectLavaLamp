Shader "Custom/URP_FullscreenCellOutline"
{
    Properties
    {
        [Header(Outline Settings)]
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0,10)) = 1
        _DepthSensitivity("Depth Sensitivity", Range(0,50)) = 10
        _NormalSensitivity("Normal Sensitivity", Range(0,50)) = 10
        _EdgeThreshold("Edge Threshold", Range(0,1)) = 0.5
        _OutlineSmoothness("Outline AA Smoothness", Range(0.001, 0.5)) = 0.05

        [Header(Cel Shading Settings)]
        _Steps("Cel Steps", Range(2, 100)) = 5
        _StepSmoothness("Step Smoothness", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        Cull Off 
        ZWrite Off 
        ZTest Always

  
        Pass
        {
            Name "FullscreenCelOutlinePass"

            HLSLPROGRAM
            #pragma vertex Vert 
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            // Uniform variables
            float4 _OutlineColor;
            float _OutlineThickness;
            float _DepthSensitivity;
            float _NormalSensitivity;
            float _EdgeThreshold;
            float _OutlineSmoothness;
            float _Steps;
            float _StepSmoothness;
                    
            // World Position Reconstruction
            float3 ReconstructionWorldPos(float2 uv, float depth)
            {
                float4 clipPos = float4(uv * 2.0 - 1.0, depth, 1.0);
                #if UNITY_UV_STARTS_AT_TOP
                    clipPos.y = -clipPos.y;
                #endif
                float4 worldPos = mul(UNITY_MATRIX_I_VP, clipPos);
                return worldPos.xyz / worldPos.w;
            }

            // Normal Reconstruction from Depth
            float3 ReconstructionNormalFromDepth(float2 uv, float2 tSize)
            {
                float depthC = SampleSceneDepth(uv);
                float depthL = SampleSceneDepth(uv + float2(-tSize.x, 0));
                float depthR = SampleSceneDepth(uv + float2(tSize.x, 0));
                float depthU = SampleSceneDepth(uv + float2(0, tSize.y));
                float depthD = SampleSceneDepth(uv + float2(0, -tSize.y));

                float3 posC = ReconstructionWorldPos(uv, depthC);
                float3 posL = ReconstructionWorldPos(uv + float2(-tSize.x, 0), depthL);
                float3 posR = ReconstructionWorldPos(uv + float2(tSize.x, 0), depthR);
                float3 posU = ReconstructionWorldPos(uv + float2(0, tSize.y), depthU);
                float3 posD = ReconstructionWorldPos(uv + float2(0, -tSize.y), depthD);
                
                float3 dx = (posR - posL) * 0.5;
                float3 dy = (posU - posD) * 0.5;

                return normalize(cross(dy, dx));
            }

            // Edge Detection (Depth)
            float DetectDepthEdge(float2 uv, float2 tSize)
            {
                float sobelX = 0.0;
                float sobelY = 0.0;

                float sobelXWeight[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
                float sumYWeight[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1}; 
                
                int index = 0;
                
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y) * tSize;
                        float depth = LinearEyeDepth(SampleSceneDepth(uv + offset), _ZBufferParams);
                        sobelX += depth * sobelXWeight[index];
                        sobelY += depth * sumYWeight[index];
                        index++;
                    }
                }
               
                return sqrt(sobelX * sobelX + sobelY * sobelY); 
            }

            // Edge Detection (Normal)
            float DetectNormalEdge(float2 uv, float2 tSize)
            {
                float3 normalC = ReconstructionNormalFromDepth(uv, tSize);
                float3 normalL = ReconstructionNormalFromDepth(uv + float2(-tSize.x, 0), tSize);
                float3 normalR = ReconstructionNormalFromDepth(uv + float2(tSize.x, 0), tSize);
                float3 normalU = ReconstructionNormalFromDepth(uv + float2(0, tSize.y), tSize);
                float3 normalD = ReconstructionNormalFromDepth(uv + float2(0, -tSize.y), tSize);

                float edgeX = length(normalR - normalL);
                float edgeY = length(normalU - normalD);
                return (edgeX + edgeY) * 0.5; 
            }

            float4 Frag(Varyings input) : SV_Target
            {
            
                float4 originalColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);

               
               // 2. APPLY FULLSCREEN CEL SHADING
                float luminance = dot(originalColor.rgb, float3(0.2126, 0.7152, 0.0722));
                float stepsCount = max(_Steps, 2.0);

                // Scale luminance to step space
                float scaledLuminance = luminance * (stepsCount - 1.0);
                float currentStep = floor(scaledLuminance);
                float fraction = frac(scaledLuminance);

                // Blend the edge transition using smoothstep
                // Lower _StepSmoothness values squeeze the transition, sharpening it
                float edgeSmooth = smoothstep(0.5 - _StepSmoothness * 0.5, 0.5 + _StepSmoothness * 0.5, fraction);
                float smoothedLuminance = (currentStep + edgeSmooth) / (stepsCount - 1.0);

                // Apply the smoothed, banded brightness to the original color
                float3 celColor = originalColor.rgb * (smoothedLuminance / max(luminance, 0.001));


                float2 customTexelSize = _OutlineThickness * float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float centerDepth = SampleSceneDepth(input.texcoord);

                if (centerDepth >= 0.99999)
                    return float4(celColor, originalColor.a);

                float depthEdge = DetectDepthEdge(input.texcoord, customTexelSize) * _DepthSensitivity;
                float normalEdge = DetectNormalEdge(input.texcoord, customTexelSize) * _NormalSensitivity;
                float combinedEdge = max(depthEdge, normalEdge);

                // ANTI-ALIASING CALCULATION
                // We use a wider smoothstep window to let the edges softly fade out over a few pixels
                float edgeMin = _EdgeThreshold - _OutlineSmoothness;
                float edgeMax = _EdgeThreshold + _OutlineSmoothness;
                combinedEdge = smoothstep(edgeMin, edgeMax, combinedEdge);
                combinedEdge = saturate(combinedEdge); // Clean 0-1 clamp

                float3 finalColor = lerp(celColor, _OutlineColor.rgb, combinedEdge);

                return float4(finalColor, originalColor.a);
            }
            ENDHLSL
        }
    }
}
