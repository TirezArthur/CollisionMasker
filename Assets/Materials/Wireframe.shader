Shader "URP/FullyLitWireframe_HighContrast"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _WireColor("Wire Color", Color) = (0, 1, 0, 1)
        _BaseColor("Base Color", Color) = (0, 1, 0, 1)
        _Thickness("Thickness", Range(0, 10)) = 1
        _WireEmission("Wire Emission", Range(0, 5)) = 1
        _AmbientStrength("Ambient Strength", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // --- MANDATORY FOR SPOTLIGHTS & POINT LIGHTS ---
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            // -----------------------------------------------

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WireColor;
            float4 _BaseColor;
            float _Thickness;
            float _WireEmission;
            float _AmbientStrength;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                
                // 1. Wireframe Mask
                float2 uv = input.uv;
                float2 fw = fwidth(uv) * _Thickness;
                float2 edge = smoothstep(fw, 0, abs(frac(uv - 0.5) - 0.5));
                float mask = max(edge.x, edge.y);

                // 2. Lighting Setup
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                // Main Light
                Light mainLight = GetMainLight(shadowCoord);
                float3 lightAcc = mainLight.color * (saturate(dot(normalWS, mainLight.direction)) * mainLight.shadowAttenuation);
                
// --- FORCED ADDITIONAL LIGHTS ---
uint pixelLightCount = GetAdditionalLightsCount();
for (uint i = 0u; i < pixelLightCount; ++i)
{
    // We use the simple signature to avoid shadow-matching issues on Metal
    Light light = GetAdditionalLight(i, input.positionWS);
    
    // Check if the light is actually contributing
    float3 diffuse = light.color * (saturate(dot(normalWS, light.direction)) * light.distanceAttenuation);
    lightAcc += diffuse;
}

                // 3. Combine
                float3 texSample = tex2D(_MainTex, input.uv).rgb;
                float3 baseAlbedo = lerp(texSample, _WireColor.rgb, mask);
                baseAlbedo += float3(0.5,0.5,0.5);
                baseAlbedo *= _BaseColor;
                
                // Ambient
                float3 ambient = SampleSH(normalWS) * _AmbientStrength;
                
                // Multiply albedo by (Direct Light + Ambient)
                float3 finalColor = baseAlbedo * (lightAcc + ambient);
                
                // Add Glow
                finalColor += _WireColor.rgb * mask * _WireEmission;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}