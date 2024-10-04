// Copyright (c) Meta Platforms, Inc. and affiliates.

Shader "[Oculus Sample] Unlit, Alpha Clipped, Custom Motion Vectors"
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
            Tags{ "LightMode" = "MotionVectors"}
            Tags { "RenderType" = "Opaque" }

            HLSLPROGRAM

            // This is OculusMotionVectoreCore.hlsl from the 2021.3/oculus-app-spacewarp branch of https://github.com/Oculus-VR/Unity-Graphics,
            // modified to pass UV coordinates and sample and clip() based on them in the fragment shader.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 previousPositionOS : TEXCOORD4;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 curPositionCS : TEXCOORD8;
                float4 prevPositionCS : TEXCOORD9;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            bool IsSmoothRotation(float3 prevAxis1, float3 prevAxis2, float3 currAxis1, float3 currAxis2)
            {
                float angleThreshold = 0.984f; // cos(10 degrees)
                float2 angleDot = float2(dot(normalize(prevAxis1), normalize(currAxis1)), dot(normalize(prevAxis2), normalize(currAxis2)));
                return all(angleDot > angleThreshold);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
            
                float3 curWS = TransformObjectToWorld(input.positionOS.xyz);
                output.curPositionCS = TransformWorldToHClip(curWS);
                if (unity_MotionVectorsParams.y == 0.0)
                {
                    output.prevPositionCS = float4(0.0, 0.0, 0.0, 1.0);
                }
                else
                {
                    bool hasDeformation = unity_MotionVectorsParams.x > 0.0;
                    float3 effectivePositionOS = (hasDeformation ? input.previousPositionOS : input.positionOS.xyz);
                    float3 previousWS = TransformPreviousObjectToWorld(effectivePositionOS);
            
                    float4x4 previousOTW = GetPrevObjectToWorldMatrix();
                    float4x4 currentOTW = GetObjectToWorldMatrix();
                    if (!IsSmoothRotation(previousOTW._11_21_31, previousOTW._12_22_32, currentOTW._11_21_31, currentOTW._12_22_32))
                    {
                        output.prevPositionCS = output.curPositionCS;
                    }
                    else
                    {
                        output.prevPositionCS = TransformWorldToPrevHClip(previousWS);
                    }
                }
            
                return output;
            }
            
            sampler2D _MainTex;
            float _AlphaClippingThreshold;

            half4 frag(Varyings i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _AlphaClippingThreshold);

                float3 screenPos = i.curPositionCS.xyz / i.curPositionCS.w;
                float3 screenPosPrev = i.prevPositionCS.xyz / i.prevPositionCS.w;
                half4 color = (1);
                color.xyz = screenPos - screenPosPrev;
                return color;
            }
            
            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
