Shader "Volorf/GenImage"
{
    Properties
    {
        _ColorA ("First Color", Color) = (1,1,1,0.5)
        _ColorB ("Second Color",   Color) = (0,0,0,0)
        _Scale  ("Scale", Float) = 0.5
        _Speed  ("Speed", Float) = 1
        _PIStrength ("Progress Indicator", Range(0, 1)) = 0.5
        _MainTex ("Texture", 2D) = "white" {}
        _TimeX ("Time", Float) = 0
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
            float _Scale, _Speed, _PIStrength, _TimeX;
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
                i.uv -= float2(0.5, 0.5);
                fixed y = abs(cos(i.uv.y * 3.14159));
                fixed4 col =  lerp(_ColorA, _ColorB, pow(y, 2) / 2);
                float2 distUX = float2(0, cos(i.uv.y * 3.14159 * 2) / 100);
                oldUV += distUX * _PIStrength;
                fixed4 tex = tex2D(_MainTex, oldUV);
                _TimeX = _Time.x;
                fixed4 fCol = tex + fixed4(col.x * col.a, col.y * col.a, col.z * col.a, 0) * _PIStrength;
                return fCol;
            }
            ENDCG
        }
    }
}
