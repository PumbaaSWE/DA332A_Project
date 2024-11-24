Shader "Unlit/TestUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Tint color", Color) = (1, 0, 0, 1)
        _Distance ("Distance", float) = 100
        _Scale ("Scale", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Distance;
            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;

                float3 origin = UnityObjectToViewPos(float3(0,0,0));
                float3 p0 = UnityObjectToViewPos(float3(0, 0, _Distance));
                //float3 p0 = UnityObjectToViewPos(float3(0, _Distance, 0));
                float3 n = UnityObjectToViewPos(float3(0, 0, 1)) - origin;
                float3 uDir = UnityObjectToViewPos(float3(1, 0, 0)) - origin;
                float3 vDir = UnityObjectToViewPos(float3(0, 1, 0)) - origin;
                float3 vert = UnityObjectToViewPos(v.vertex);


                float a = dot(p0 , n) / dot(vert, n);

                float3 vert_prime = (-a * vert) - p0;
                

                o.vertex = UnityObjectToClipPos(v.vertex);


                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.uv = float2(dot(vert_prime, uDir), dot(vert_prime, vDir));
                o.uv /= _Scale * _Distance;
                o.uv += .5;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col - float4(.5,.5,.5,.5));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col*_Color;
            }
            ENDCG
        }
    }
}
