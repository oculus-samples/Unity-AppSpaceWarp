Shader "[Oculus Sample] Transparent"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MinLightingVal("Minimum Lighting Amount", Range(0, 1)) = 0.05
        
        [Toggle(RGB_ON)] _RgbOn("Enable RGB", Float) = 1.0
        [Toggle] _ZWrite("Enable ZWrite", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite[_ZWrite]

            HLSLPROGRAM
            #pragma multi_compile _ RGB_ON

            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _MinLightingVal;
            
            v2f vert (appdata v)
            {
                #if RGB_ON
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                    o.normal = TransformObjectToWorldNormal(v.normal);
                    o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                    return o;
                #else
                    return (v2f)0;
                #endif
            }
            
            float4 frag (v2f i) : SV_Target 
            {
                #if RGB_ON
                    float4 col = tex2D(_BaseMap, i.uv);
                    col *= _BaseColor;
                    
                    float NdotL = saturate(dot(i.normal, _MainLightPosition.xyz));
                    NdotL = max(NdotL, _MinLightingVal);
                    col.xyz *= (float3)(_MainLightColor * NdotL);

                    return col;
                #else
                    return 0;
                #endif
            }
            ENDHLSL
        }

        //Transparent RenderQueue objects aren't even considered for motion vectors
        //So even if we have a MotionVector pass here, it wouldn't be run
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
