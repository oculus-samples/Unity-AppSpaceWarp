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
        
        // This pass is copy/pasted from Shaders/Unlit.shader, 
        // from the 2021.3/oculus-app-spacewarp branch of https://github.com/Oculus-VR/Unity-Graphics
        Pass
        {
            Name "MotionVectors"
            Tags{ "LightMode" = "MotionVectors"}
            Tags { "RenderType" = "Opaque" }

            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/OculusMotionVectorCore.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
