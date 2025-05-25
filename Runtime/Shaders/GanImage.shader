Shader "Volorf/GenImage"
{
    Properties
    {
        _ColorA ("First Color", Color) = (1,1,1,0.5)
        _ColorB ("Second Color",   Color) = (0,0,0,0)
        _Scale  ("Scale", Float) = 0.5
        _Speed  ("Speed", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct app { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            fixed4 _ColorA, _ColorB;
            float _Scale, _Speed;
            sampler2D _MainTex;
            
            v2f vert(app v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 oldUV = i.uv;
                i.uv.x = i.uv.x - _Time.x * _Speed;
                i.uv = float2( i.uv.x + i.uv.y, i.uv.x - i.uv.y );
                i.uv *= _Scale;
                fixed y = abs(cos(i.uv.y * 3.14159));
                fixed4 col =  lerp(_ColorA, _ColorB, pow(y, 2) / 2);
                fixed4 tex = tex2D(_MainTex, float2(oldUV.x + cos(i.uv.x * 2) * 0, oldUV.y + cos(i.uv.y * 3.14159 * 2) / 100));
                fixed4 fCol = tex + fixed4(col.x * col.a, col.y * col.a, col.z * col.a, 0);
                return fCol;
            }
            ENDCG
        }
    }
}
