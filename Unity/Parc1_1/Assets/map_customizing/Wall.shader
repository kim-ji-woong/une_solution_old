Shader "Custom/Wall"
{
    Properties
    {
        _TopColor("TopColor", Color) = (0.18,0.18,0.18,0)
        _SideColor("SideColor", Color) = (0.588,0.588,0.588,0)
        _Glossiness("Smoothness", Range(0,1)) = 0.0
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Indicate("Indicate", Range(0,1)) = 0
        _IndicateColor("IndicateColor", Color) = (1.0, 0.7,0.3,0)
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        uniform float4 _TopColor;
        uniform float4 _SideColor;
        uniform float _Indicate;
        uniform float4 _IndicateColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float dotV = saturate(abs(dot(normalize(o.Normal), float3(0, 1, 0))));

            float3 col = float3(
                lerp(_SideColor.x, _TopColor.x, dotV),
                lerp(_SideColor.y, _TopColor.y, dotV),
                lerp(_SideColor.z, _TopColor.z, dotV)
                );

            o.Albedo = col;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
            o.Emission = _IndicateColor * _Indicate * 0.5f;
        }
        ENDCG
    }
        FallBack "Diffuse"
}