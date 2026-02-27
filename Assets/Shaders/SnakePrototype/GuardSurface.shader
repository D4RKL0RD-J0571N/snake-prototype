Shader "SnakePrototype/GuardSurface"
{
    Properties
    {
        [MainColor] _BaseColor("Passive Color", Color) = (0.5, 0.5, 0.5, 1)
        _AlertColor("Alert Color", Color) = (1, 0, 0, 1)
        _AlertIntensity("Alert Intensity", Range(0.0, 1.0)) = 0.0
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _AlertColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _AlertIntensity)
            UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                float4 alertColor = UNITY_ACCESS_INSTANCED_PROP(Props, _AlertColor);
                float alertIntensity = UNITY_ACCESS_INSTANCED_PROP(Props, _AlertIntensity);

                float blink = sin(_Time.y * 10.0) * 0.5 + 0.5;
                half3 color = lerp(baseColor.rgb, alertColor.rgb * blink, alertIntensity);
                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
