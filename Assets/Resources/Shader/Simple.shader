Shader "Unlit/Simple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 col : COLOR0;
                float4 vertex : SV_POSITION;
            };

            struct VertexData
            {
                float3 pos;
                float4 color;
            };

            StructuredBuffer<VertexData> vertexDataBuffer;

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(vertexDataBuffer[id].pos, 1));
                o.col = vertexDataBuffer[id].color;

                //o.col = float4(normalize((mul(unity_ObjectToWorld, float4(0,0,1, 1.0))).xyz), 1);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.col;
            }
            ENDCG
        }
    }
}