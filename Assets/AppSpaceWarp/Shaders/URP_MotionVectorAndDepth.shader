// Copyright (c) Meta Platforms, Inc. and affiliates.

Shader "[Oculus Sample] Motion Vectors + Depth"
{
    Properties
    {
        [Toggle(MOTIONVECTORS_ON)] _MotionVectorsOn("Enable Motion Vectors", Float) = 1.0
        [Toggle] _ZWrite("Enable ZWrite", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "MotionVectors"
            Tags{ "LightMode" = "MotionVectors"}

            ZWrite[_ZWrite]

            HLSLPROGRAM
            #pragma multi_compile _ MOTIONVECTORS_ON

            #if MOTIONVECTORS_ON
                #define APPLICATION_SPACE_WARP_MOTION
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ObjectMotionVectors.hlsl"
            #else
                #pragma vertex vert
                #pragma fragment frag

                struct Attributes
                {
                    float4 positionOS : POSITION;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                };

                Varyings vert(Attributes input)
                {
                    return (Varyings)0;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    return 0;
                }
            #endif
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
