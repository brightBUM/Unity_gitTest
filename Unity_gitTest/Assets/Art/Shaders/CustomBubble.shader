Shader "Custom/ThinFilmBubble_Animated_URP"
{
    Properties
    {
        _Cube ("Environment Cubemap", Cube) = "" {}
        _NoiseTex ("Noise", 2D) = "white" {}

        _ThicknessMin ("Thickness Min", Float) = 0.5
        _ThicknessMax ("Thickness Max", Float) = 1.5

        _DispersionMin ("Dispersion Min", Float) = 0.01
        _DispersionMax ("Dispersion Max", Float) = 0.05

        _RefractionStrength ("Refraction Strength", Float) = 0.2
        _PulseSpeed ("Pulse Speed", Float) = 1.0

        _Alpha ("Transparency", Range(0,1)) = 0.7
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Scene color
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            // Cubemap
            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            // Noise
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _ThicknessMin;
            float _ThicknessMax;

            float _DispersionMin;
            float _DispersionMax;

            float _RefractionStrength;
            float _PulseSpeed;
            float _Alpha;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
                float4 screenPos   : TEXCOORD3;
            };

            float fresnel(float3 V, float3 N)
            {
                return pow(1.0 - saturate(dot(V, N)), 5.0);
            }

            float3 thinFilm(float3 normal, float3 viewDir, float thickness)
            {
                float ndv = saturate(dot(normal, viewDir));

                float3 wavelengths = float3(0.65, 0.51, 0.475);
                float3 phase = thickness / wavelengths;

                return 0.5 + 0.5 * cos(phase * 25.0 * ndv);
            }

            Varyings vert(Attributes v)
            {
                Varyings o;

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                o.positionHCS = TransformWorldToHClip(worldPos);
                o.worldPos = worldPos;
                o.normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                o.screenPos = ComputeScreenPos(o.positionHCS);

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 N = normalize(i.normalWS);
                float3 V = normalize(i.viewDirWS);

                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                float time = _Time.y * _PulseSpeed;

                // ===== THICKNESS ANIMATION =====
                float pulse = 0.5 + 0.5 * sin(time);

                float variation = dot(N, float3(1.3, 2.1, 0.7));
                pulse += 0.1 * sin(time * 2.3 + variation);

                pulse = saturate(pulse);

                float animatedThickness = lerp(_ThicknessMin, _ThicknessMax, pulse);

                float2 uv = N.xy * 0.5 + 0.5;
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv).r;

                float thickness = noise * animatedThickness;

                // ===== DISPERSION ANIMATION =====
                float dispPulse = 0.5 + 0.5 * sin(time * 1.3 + 1.7);
                dispPulse += 0.1 * sin(dot(N, float3(2.0, 1.5, 3.0)) + time);

                dispPulse = saturate(dispPulse);

                float dispersion = lerp(_DispersionMin, _DispersionMax, dispPulse);

                float ior = 1.0;

                float3 refrR = refract(-V, N, 1.0 / (ior + dispersion));
                float3 refrG = refract(-V, N, 1.0 / (ior));
                float3 refrB = refract(-V, N, 1.0 / (ior - dispersion));

                float2 offsetR = refrR.xy * _RefractionStrength;
                float2 offsetG = refrG.xy * _RefractionStrength;
                float2 offsetB = refrB.xy * _RefractionStrength;

                float r = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetR).r;
                float g = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetG).g;
                float b = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetB).b;

                float3 refractedColor = float3(r, g, b);

                // ===== REFLECTION =====
                float3 R = reflect(-V, N);
                float3 env = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, R).rgb;

                // ===== THIN FILM =====
                float3 film = thinFilm(N, V, thickness);

                float f = fresnel(V, N);

                float3 finalColor = lerp(refractedColor, env * film, f);

                return float4(finalColor, _Alpha);
            }

            ENDHLSL
        }
    }
}