Shader "SnakePrototype/GridDynamic"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0, 0.5, 1, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 1)
        _Thickness("Thickness", Range(0.0, 0.5)) = 0.05
        _CellSize("Cell Size", Float) = 1.0
        _AlertIntensity("Alert Intensity", Range(0.0, 1.0)) = 0.0
        _PulseSpeed("Pulse Speed", Float) = 1.0
        _FlowSpeed("Flow Speed", Float) = 0.2
        _NoiseScale("Noise Scale", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BackgroundColor;
                float _Thickness;
                float _CellSize;
                float _AlertIntensity;
                float _PulseSpeed;
                float _FlowSpeed;
                float _NoiseScale;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Safety: Avoid div by zero
                float cellSize = max(0.01, _CellSize);
                float2 gridPos = input.worldPos.xz / cellSize;
                
                // Add flow
                float time = _Time.y;
                gridPos += float2(time * _FlowSpeed, time * _FlowSpeed * 0.5);

                float2 grid = abs(frac(gridPos - 0.5) - 0.5);
                float lineCheck = min(grid.x, grid.y);
                
                float isLine = 1.0 - smoothstep(0.0, _Thickness, lineCheck);
                
                // Pulsing
                float pulse = sin(time * _PulseSpeed) * 0.5 + 0.5;
                pulse = lerp(1.0, 1.0 + pulse * 0.5, _AlertIntensity);
                
                // Subtle noise background
                float noise = frac(sin(dot(input.uv * _NoiseScale, float2(12.9898, 78.233))) * 43758.5453);
                float3 background = _BackgroundColor.rgb + (noise * 0.02);

                float3 color = lerp(background, _BaseColor.rgb * pulse, isLine);
                
                // Alert tint
                color = lerp(color, color * float3(1, 0.2, 0.2), _AlertIntensity * 0.5);
                
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
