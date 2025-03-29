Shader "Custom/ClippingBoxShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;

        float3 _ClipBoxCenter;
        float3 _ClipBoxExtents;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldPos = IN.worldPos;

            float3 minBounds = _ClipBoxCenter - _ClipBoxExtents;
            float3 maxBounds = _ClipBoxCenter + _ClipBoxExtents;

            // Clip anything outside the box
            if (any(worldPos < minBounds) || any(worldPos > maxBounds))
                clip(-1);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
