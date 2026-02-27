Shader "SnakePrototype/NeonGrid"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (0,1,0,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _GridThickness ("Grid Thickness", Range(0.01, 0.1)) = 0.02
        _CellSize ("Cell Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float4 _GridColor;
            float4 _BackgroundColor;
            float _GridThickness;
            float _CellSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate grid based on world position
                float2 pos = i.worldPos.xz;
                
                // Use fmod to create repeating pattern
                float2 grid = abs(frac(pos / _CellSize) - 0.5);
                float lineCheck = min(grid.x, grid.y);
                
                // Smoothstep for anti-aliasing equivalent
                float isLine = step(lineCheck, _GridThickness);

                return lerp(_BackgroundColor, _GridColor, isLine);
            }
            ENDCG
        }
    }
}
