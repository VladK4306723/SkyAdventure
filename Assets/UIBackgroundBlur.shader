Shader "UI/BackgroundBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0, 10)) = 3
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "UIBlur"
            ZTest Always
            ZWrite Off
            Cull Off

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

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _BlurSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 texel = _BlurSize / _ScreenParams.xy;

                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv);
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv + texel);
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv - texel);
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv + float2(texel.x, -texel.y));
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv + float2(-texel.x, texel.y));

                return col / 5;
            }
            ENDHLSL
        }
    }
}
