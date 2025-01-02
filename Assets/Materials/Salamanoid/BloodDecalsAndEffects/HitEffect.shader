Shader "Custom/HitEffect"
{
   Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ImpactPoint ("Impact Point (Local)", Vector) = (0, 0, 0, 0)
        _ImpactColor ("Impact Color", Color) = (1, 0, 0, 1)
        _ImpactRadius ("Impact Radius", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _ImpactPoint; 
            float4 _ImpactColor;
            float _ImpactRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localPos = v.vertex.xyz; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                float dist = distance(i.localPos, _ImpactPoint.xyz);
                float effect = smoothstep(_ImpactRadius, 0, dist);
                return lerp(_ImpactColor, baseColor, effect);
            }
            ENDCG
        }
    }
}
