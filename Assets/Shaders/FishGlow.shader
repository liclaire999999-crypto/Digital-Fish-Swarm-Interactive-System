Shader "Fish Swarm/Fish Glow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (0, 1, 1, 1)
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.0
        _RimPower ("Rim Power", Range(0.1, 10)) = 3.0
        _RimColor ("Rim Color", Color) = (0.5, 1, 1, 1)
        _RimIntensity ("Rim Intensity", Range(0, 2)) = 1.0
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.7
        _FresnelPower ("Fresnel Power", Range(0, 10)) = 3.0
        _WaterDepthFade ("Water Depth Fade", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionMap;
        fixed4 _Color;
        fixed4 _EmissionColor;
        fixed4 _RimColor;
        float _GlowIntensity;
        float _RimPower;
        float _RimIntensity;
        float _Metallic;
        float _Smoothness;
        float _FresnelPower;
        float _WaterDepthFade;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 基础纹理采样
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 baseColor = tex * _Color;

            // 发光贴图
            fixed4 emissionTex = tex2D(_EmissionMap, IN.uv_MainTex);
            fixed3 emission = emissionTex.rgb * _EmissionColor.rgb * _GlowIntensity;

            // 边缘光（Rim Light）
            float rimDot = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            float rimLight = pow(rimDot, _RimPower) * _RimIntensity;
            fixed3 rimColor = rimLight * _RimColor.rgb * _RimColor.a;

            // 菲涅尔效应（Fresnel Effect）
            float fresnel = pow(rimDot, _FresnelPower);
            emission += fresnel * _EmissionColor.rgb * 0.5;

            // 水深衰减（模拟水下环境）
            float depth = length(IN.worldPos - _WorldSpaceCameraPos);
            float depthFade = exp(-depth * _WaterDepthFade * 0.1);
            emission *= depthFade;

            // 输出
            o.Albedo = baseColor.rgb;
            o.Normal = o.Normal;  // 使用法线贴图（如有）
            o.Emission = emission + rimColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = baseColor.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
