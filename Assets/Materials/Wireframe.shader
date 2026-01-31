Shader "URP/FullyLitWireframe_Transparent"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _WireColor("Wire Color", Color) = (0, 1, 0, 1)
        _OverlayColor("Overlay Color", Color) = (1, 1, 1, 1)
        _BaseAlpha("Base Alpha", Range(0, 1)) = 0.5
        _Thickness("Thickness", Range(0, 10)) = 1
        _WireEmission("Wire Emission", Range(0, 5)) = 1
        _AmbientStrength("Ambient Strength", Range(0, 1)) = 0.2
    }

    SubShader
    {
        // 1. Change Queue to Transparent
        Tags { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Transparent" 
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            // 2. Enable Alpha Blending and disable Z-Write (standard for transparent)
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

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
            float4 _OverlayColor;
            float _BaseAlpha;
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
                
                // Wireframe Mask
                float2 uv = input.uv;
                float2 fw = fwidth(uv) * _Thickness;
                float2 edge = smoothstep(fw, 0, abs(frac(uv - 0.5) - 0.5));
                float mask = max(edge.x, edge.y);

                // Lighting Setup
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                float3 lightAcc = mainLight.color * (saturate(dot(normalWS, mainLight.direction)) * mainLight.shadowAttenuation);
                
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < pixelLightCount; ++i)
                {
                    Light light = GetAdditionalLight(i, input.positionWS);
                    lightAcc += light.color * (saturate(dot(normalWS, light.direction)) * light.distanceAttenuation);
                }

                // Combine Color
                float4 texSample = tex2D(_MainTex, input.uv);
                float3 baseAlbedo = lerp(texSample.rgb, _WireColor.rgb, mask);
                                baseAlbedo *= _OverlayColor;

                float3 ambient = SampleSH(normalWS) * _AmbientStrength;
                float3 finalColor = baseAlbedo * (lightAcc + ambient);
                finalColor += _WireColor.rgb * mask * _WireEmission;

                // 3. Calculate Alpha
                // We use the wireframe mask to ensure the wires are always more opaque than the mesh faces
                float finalAlpha = lerp(_BaseAlpha * texSample.a, 1.0, mask);

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}