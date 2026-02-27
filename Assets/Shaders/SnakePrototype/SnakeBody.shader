Shader "SnakePrototype/SnakeBody"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 0, 0, 1)
        _EmissionColor("Emission Color", Color) = (1, 0, 0, 1)
        _GradientRatio("Gradient Ratio", Range(0.0, 1.0)) = 0.0
        _PulseIntensity("Pulse Intensity", Range(0.0, 2.0)) = 1.0
        _FresnelPower("Fresnel Power", Range(0.1, 8.0)) = 2.0
        _Dissolve("Dissolve", Range(0.0, 1.0)) = 0.0
        _ScanSpeed("Scan Speed", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            ENDHLSL

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _GradientRatio)
                UNITY_DEFINE_INSTANCED_PROP(float, _PulseIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _FresnelPower)
                UNITY_DEFINE_INSTANCED_PROP(float, _Dissolve)
                UNITY_DEFINE_INSTANCED_PROP(float, _ScanSpeed)
            UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.viewDirWS = GetWorldSpaceViewDir(worldPos);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                float4 emissionColor = UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionColor);
                float gradientRatio = UNITY_ACCESS_INSTANCED_PROP(Props, _GradientRatio);
                float pulseIntensity = UNITY_ACCESS_INSTANCED_PROP(Props, _PulseIntensity);
                float fresnelPower = UNITY_ACCESS_INSTANCED_PROP(Props, _FresnelPower);
                float dissolve = UNITY_ACCESS_INSTANCED_PROP(Props, _Dissolve);
                float scanSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _ScanSpeed);

                // Dissolve / Clipping
                float scan = frac(input.uv.y - _Time.y * scanSpeed);
                if (scan < dissolve) discard;

                float pulse = sin(_Time.y * 5.0) * 0.2 + 0.8;
                
                // Fresnel
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), fresnelPower);

                half3 finalColor = baseColor.rgb * lerp(1.0, 0.3, gradientRatio);
                half3 emissive = (emissionColor.rgb * pulseIntensity * pulse) + (baseColor.rgb * fresnel * 2.0);
                
                // Add a bright edge at the dissolve line
                float edge = step(scan, dissolve + 0.05) * (1.0 - step(scan, dissolve));
                emissive += baseColor.rgb * edge * 5.0;

                return half4(finalColor + emissive, 1.0);
            }
            ENDHLSL
        }
    }
}
