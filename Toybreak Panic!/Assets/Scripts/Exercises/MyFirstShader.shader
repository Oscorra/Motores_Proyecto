Shader "Custom/MyFirstShader"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.8, 1.0, 1.0)
        _Speed ("Change Speed", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

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
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Speed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float wave = 0.5 + 0.5 * sin((IN.uv.x + IN.uv.y + _Time.y * _Speed) * 6.2831853);
                float3 finalColor = lerp(_Color.rgb * 0.25, _Color.rgb, wave);
                return half4(finalColor, _Color.a);
            }
            ENDHLSL
        }
    }
}
