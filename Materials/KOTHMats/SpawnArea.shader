Shader "Unlit/SpawnArea"
{
	Properties
	{
		_Scale ("Scale", Range(0, 1)) = 0.5
		_BaseColor ("Base Color", Color) = (0, 0, 0, 0)
		_ScrollColor ("Scroll Color", Color) = (0, 0, 0, 0)
		_ScrollWidth ("Scroll Width", Range(0, 2)) = 0.5
		_ScrollCount ("Scroll Count", Float) = 1
		_TestValue ("Test", Float) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			float4 _BaseColor, _ScrollColor;
			float _Scale, _ScrollWidth, _ScrollCount;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 col = _BaseColor;

				col.a = 1 - i.uv.y;
				col.a *= _Scale;
				col.a *= (1 - i.normal.y);
				col.a = clamp(col.a, 0, 1);
				
				if(sin((i.uv.y - _SinTime.y) * _ScrollCount) < _ScrollWidth - 1) {
					col.rgb = _ScrollColor;
				}

				

				return col;
			}
			ENDCG
		}
	}
}
