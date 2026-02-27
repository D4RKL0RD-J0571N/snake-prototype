Shader "SnakePrototype/CorePulse"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 0.8, 0, 1)
        _PulseSpeed("Pulse Speed", Float) = 2.0
        _GlowIntensity("Glow Intensity", Range(0.0, 5.0)) = 1.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend One One
        ZWrite Off

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
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _PulseSpeed)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlowIntensity)
            UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                float pulseSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _PulseSpeed);
                float glowIntensity = UNITY_ACCESS_INSTANCED_PROP(Props, _GlowIntensity);

                float pulse = sin(_Time.y * pulseSpeed) * 0.5 + 0.5;
                float circle = 1.0 - length(input.uv - 0.5) * 2.0;
                circle = saturate(circle);
                
                half3 color = baseColor.rgb * glowIntensity * pulse * circle;
                return half4(color, circle);
            }
            ENDHLSL
        }
    }
}
