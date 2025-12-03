// Copyright (c) Meta Platforms, Inc. and affiliates.

Shader "[Oculus Sample] Unlit, Alpha Clipped, Motion Vectors"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaClippingThreshold("Alpha Clipping Threshold", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float _AlphaClippingThreshold;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _AlphaClippingThreshold);
                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Name "MotionVectors"
            Tags { "LightMode" = "MotionVectors"}

            HLSLPROGRAM

            // This is ObjectMotionVectors.hlsl from the 6000.0/oculus-app-spacewarp branch of https://github.com/Oculus-VR/Unity-Graphics,
            // modified to match this shader's input object names and to remove some unused cases for this shader.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag


            struct Attributes
            {
                float4 position             : POSITION;
                float2 uv                   : TEXCOORD0;
                float3 positionOld          : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS                 : SV_POSITION;
                float4 positionCSNoJitter         : POSITION_CS_NO_JITTER;
                float4 previousPositionCSNoJitter : PREV_POSITION_CS_NO_JITTER;
                float2 uv                         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            bool IsIdentity(in float4x4 modelMatrix)
            {
                return
                    modelMatrix[0][0] == 1.0 && modelMatrix[0][1] == 0.0 && modelMatrix[0][2] == 0.0 && modelMatrix[0][3] == 0.0 &&
                    modelMatrix[1][0] == 0.0 && modelMatrix[1][1] == 1.0 && modelMatrix[1][2] == 0.0 && modelMatrix[1][3] == 0.0 &&
                    modelMatrix[2][0] == 0.0 && modelMatrix[2][1] == 0.0 && modelMatrix[2][2] == 1.0 && modelMatrix[2][3] == 0.0 &&
                    modelMatrix[3][0] == 0.0 && modelMatrix[3][1] == 0.0 && modelMatrix[3][2] == 0.0 && modelMatrix[3][3] == 1.0;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                const VertexPositionInputs vertexInput = GetVertexPositionInputs(input.position.xyz);

                output.uv = input.uv;

                // We do not need jittered position in ASW
                output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.position));;
                output.positionCS = output.positionCSNoJitter;

                float4 prevPos = (unity_MotionVectorsParams.x == 1) ? float4(input.positionOld, 1) : input.position;

                // Particle System Workaround.
                // There is currently a bug in Unity that UNITY_PREV_MATRIX_M is relative to the particle system
                // transform, but UNITY_MATRIX_M is always identity, causing artifacts for particles with motion vectors.
                // We can avoid this bug by checking whether the current model matrix is the identity matrix, and if
                // so, simply use the unaltered previous position without multiplying by UNITY_PREV_MATRIX_M.
                if (!IsIdentity(UNITY_MATRIX_M))
                {
                    prevPos = mul(UNITY_PREV_MATRIX_M, prevPos);
                }

                output.previousPositionCSNoJitter = mul(_PrevViewProjMatrix, prevPos);

                return output;
            }
            
            void TransparentClipAlpha(half albedoAlpha, half4 color)
            {
                half alpha = albedoAlpha * color.a;

                // force cutoff to 0.5
                clip(alpha - 0.5);
            }

            sampler2D _MainTex;
            float _AlphaClippingThreshold;

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                TransparentClipAlpha(tex2D(_MainTex, input.uv).a, half4(1,1,1,1));

                return float4(CalcAswNdcMotionVectorFromCsPositions(input.positionCSNoJitter, input.previousPositionCSNoJitter), 1);
            }

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
