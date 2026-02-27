Shader "SnakePrototype/DetectionCone"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (0, 1, 0, 0.3)
        _ScanLineCount ("Scan Line Count", Float) = 20
        _ScanSpeed ("Scan Speed", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _ScanLineCount;
                float _ScanSpeed;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Scanning lines based on UV distance from center (radial)
                // Actually the DetectionConeView uses a LineRenderer, so UVs are along the edge.
                // If it's a mesh, we could do more.
                
                float scan = sin(input.uv.x * _ScanLineCount - _Time.y * _ScanSpeed) * 0.5 + 0.5;
                half4 col = _BaseColor;
                col.a *= lerp(0.5, 1.0, scan);
                
                // Add a bit of additive glow to the scan lines
                half3 glow = _BaseColor.rgb * scan * 0.5;
                
                return half4(col.rgb + glow, col.a);
            }
            ENDHLSL
        }
    }
}
