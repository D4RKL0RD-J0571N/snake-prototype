Shader "SnakePrototype/ScreenOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _ScanlineCount ("Scanline Count", Float) = 500
        _VignetteStrength ("Vignette Strength", Range(0, 2)) = 1.0
        _FlickerSpeed ("Flicker Speed", Float) = 10.0
        _FlickerIntensity ("Flicker Intensity", Range(0, 0.1)) = 0.02
        _AbberationStrength ("Chromatic Abberation", Range(0, 0.02)) = 0.005
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            CBUFFER_START(UnityPerMaterial)
                float _ScanlineIntensity;
                float _ScanlineCount;
                float _VignetteStrength;
                float _FlickerSpeed;
                float _FlickerIntensity;
                float _AbberationStrength;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Simple Chromatic Abberation (Pseudo)
                float2 uv = input.uv;
                half4 r = tex2D(_MainTex, uv + float2(_AbberationStrength, 0));
                half4 g = tex2D(_MainTex, uv);
                half4 b = tex2D(_MainTex, uv - float2(_AbberationStrength, 0));
                
                half4 color = half4(r.r, g.g, b.b, g.a);

                // Scanlines
                float scanline = sin(uv.y * _ScanlineCount) * 0.5 + 0.5;
                color.rgb *= lerp(1.0, 1.0 - _ScanlineIntensity, scanline);

                // Vignette
                float2 dist = (uv - 0.5) * 2.0;
                float vignette = saturate(1.0 - dot(dist, dist) * 0.25 * _VignetteStrength);
                color.rgb *= vignette;

                // Flicker
                float flicker = sin(_Time.y * _FlickerSpeed) * _FlickerIntensity;
                color.rgb += flicker;

                return color;
            }
            ENDHLSL
        }
    }
}
