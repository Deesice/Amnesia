Shader "Hidden/RadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _afSize("afSize", Float) = 0
        _afBlurStartDist("afBlurStartDist", Float) = 0
        _Aspect("Aspect", Float) = 1
        _VerticalScreenHalfSize("VerticalScreenHalfSize", Float) = 540

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            float _afSize;
            float _afBlurStartDist;
            float _Aspect;
            float _VerticalScreenHalfSize;

            fixed4 frag(v2f i) : SV_Target
            {
                float fTotalMul = 1.1f;
                float vColorMul[5];
                float vSizeMul[5];
                vColorMul[0] = 0.1; vColorMul[1] = 0.2; vColorMul[2] = 0.5; vColorMul[3] = 0.2; vColorMul[4] = 0.1;
                vSizeMul[0] = -1.0; vSizeMul[1] = -0.5; vSizeMul[2] = 0.0; vSizeMul[3] = 0.5; vSizeMul[4] = 1.0;

                float2 vScreenCoord = i.uv;
                vScreenCoord.y = 1 - vScreenCoord.y;
                float2 vDir = float2(_Aspect, 1) * _VerticalScreenHalfSize - vScreenCoord;
                float fDist = length(vDir) / (_Aspect * _VerticalScreenHalfSize);
                vDir = normalize(vDir);

                fDist = max(0.0f, fDist - _afBlurStartDist);

                vDir = vDir * fDist * _afSize;

                float3 vDiffuseColor = float3(0, 0, 0);

                vScreenCoord.y = 1 - vScreenCoord.y;

                for (int i = 0; i < 5; ++i)
                {
                    vDiffuseColor = vDiffuseColor + tex2D(_MainTex, vScreenCoord - vDir * vSizeMul[i]).xyz * vColorMul[i];
                }

                vDiffuseColor = vDiffuseColor / fTotalMul;

                return float4(vDiffuseColor, 1);
            }
            ENDCG
        }
    }
}
