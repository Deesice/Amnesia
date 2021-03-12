Shader "Custom/Fog"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    sampler2D _MainTex;
    sampler2D _CameraDepthTexture;
    float _CameraClipPlane;
    float _Start;
    float _End;
    float4 _Color;
    float _Exp;
    float _Strength;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float depth = tex2D(_CameraDepthTexture, i.texcoord).r;
        depth = Linear01Depth(depth) *_CameraClipPlane;
        depth = min(depth - _Start, _End - _Start);
        depth = pow(max(depth / (_End - _Start), 0), _Exp);
        return lerp(tex2D(_MainTex, i.texcoord), _Color, depth * _Strength);
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always

            Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}