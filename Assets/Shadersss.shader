// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ACG/Shadersss"{
    Properties{
        [NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
    }
    SubShader{
        Pass{
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata{
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };
                struct v2f{
                    float4 p_pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };
                //Vertex(appdata) -> fragment(v2f)
                //return v2f -----> fragment
                v2f vert(appdata v){
                    v2f o;
                    //Transform position to clip space
                    //multiply with model*view*projection matrix
                    o.p_pos = UnityObjectToClipPos(v.pos);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;

                float4 frag(v2f i): SV_Target{
                    float4 color = tex2D(_MainTex, i.uv);
                    return color;
                }

            ENDCG
        }
    }
}