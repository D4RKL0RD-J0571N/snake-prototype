Shader "SnakePrototype/PulseRipple"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (0, 1, 1, 1)
        _Thickness ("Thickness", Range(0.001, 0.5)) = 0.05
        _Softness ("Softness", Range(0.001, 0.5)) = 0.05
        _Lifetime ("Lifetime (0-1)", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Blend One One
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
                UNITY_DEFINE_INSTANCED_PROP(float, _Thickness)
                UNITY_DEFINE_INSTANCED_PROP(float, _Softness)
                UNITY_DEFINE_INSTANCED_PROP(float, _Lifetime)
            UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                float lifetime = UNITY_ACCESS_INSTANCED_PROP(Props, _Lifetime);
                float thickness = UNITY_ACCESS_INSTANCED_PROP(Props, _Thickness);
                float softness = UNITY_ACCESS_INSTANCED_PROP(Props, _Softness);

                float dist = length(input.uv - 0.5) * 2.0; // 0 to 1
                
                // Ring expands with lifetime
                float radius = lifetime;
                float ring = 1.0 - abs(dist - radius) / thickness;
                ring = saturate(ring);
                ring = pow(ring, 1.0 / softness);
                
                // Fade out over lifetime
                float alpha = 1.0 - lifetime;
                
                return baseColor * ring * alpha;
            }
            ENDHLSL
        }
    }
}
