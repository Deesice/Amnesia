Shader "Hidden/Sepia"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ConvMap ("Convertion Map", 2D) = "white" {}
        _Offset ("Offset", Float) = 0
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
            sampler2D _ConvMap;
            float _Offset;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 vDiffuseColor = tex2D(_MainTex, i.uv);

                /*fixed4 vOutput = fixed4(tex1D(_ConvMap, vDiffuseColor.r * 255.0f / 256 + 1.0f / 512).r,
                    tex1D(_ConvMap, vDiffuseColor.g * 255.0f / 256 + 1.0f / 512).g,
                    tex1D(_ConvMap, vDiffuseColor.b * 255.0f / 256 + 1.0f / 512).b, 1);*/

                fixed3 vOutput = fixed3(tex2D(_ConvMap, float2(vDiffuseColor.r * 255 / 256 + 1 / 512, 0.5f)).r,
                    tex2D(_ConvMap, float2(vDiffuseColor.g * 255 / 256 + 1 / 512, 0.5f)).g,
                    tex2D(_ConvMap, float2(vDiffuseColor.b * 255 / 256 + 1 / 512, 0.5f)).b);

                vDiffuseColor.r = vOutput.r;
                vDiffuseColor.g = vOutput.g;
                vDiffuseColor.b = vOutput.b;

                return vDiffuseColor;
            }
            ENDCG
        }
    }
}
