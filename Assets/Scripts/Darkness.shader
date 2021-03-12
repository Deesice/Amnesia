Shader "MyShader/Darkness"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplaceTexBase("Displacement Base Texture", 2D) = "white" {}
        _Magnitude("Magnitude", Range(0,1.0)) = 1
        _Speed("Speed", float) = 1
        _Offset("Offset", float) = 0.5
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
            #include "noiseSimplex.cginc"

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
            sampler2D _DisplaceTexBase;
            float _Magnitude;
            float _Speed;
            float _Offset;

            fixed4 frag(v2f i) : SV_Target
            {
                if (_Magnitude == 0)
                    return tex2D(_MainTex, i.uv);

                float2 disp = tex2D(_DisplaceTexBase, i.uv).xy;
                float amount = tex2D(_DisplaceTexBase, i.uv).z;
                disp.x;
                disp = disp - 0.25f;                
                disp = disp * _Magnitude * amount;

                float2 noisePos = float2(0, 0);
                noisePos.x = _Time.x * _Speed;

                disp = disp * ((snoise(noisePos) / 2) + 1);

                fixed4 col = tex2D(_MainTex, i.uv + disp);
                return col;
            }
            ENDCG
        }
    }
}
