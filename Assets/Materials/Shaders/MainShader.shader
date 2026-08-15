// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

// Шейдер взял отсюда https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/DefaultResourcesExtra/Mobile/Mobile-Bumped.shader

Shader "World Shader" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _TextureScale("Texture scale", float) = 1
    [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

    CGPROGRAM
    #pragma surface surf Lambert noforwardadd

    sampler2D _MainTex;
    float _TextureScale;
    // sampler2D _BumpMap;

    struct Input {
        float2 uv_MainTex; 
        float3 worldPos;
        float3 worldNormal;
    };

    void surf (Input IN, inout SurfaceOutput o) {
        float x = IN.worldPos.x*_TextureScale;
        float y = IN.worldPos.y*_TextureScale;
        float z = IN.worldPos.z*_TextureScale;

        float isUp = abs(IN.worldNormal.y);

        float2 offset = float2(fmod(z+x*(1-isUp),0.0625), fmod(y+x*isUp,0.0625));

        fixed4 c = tex2Dlod(_MainTex, float4(IN.uv_MainTex + offset, 0, 0));
        o.Albedo = c.rgb;
        o.Alpha = 1;
    }
    ENDCG
    }

    FallBack "Mobile/Diffuse"
}