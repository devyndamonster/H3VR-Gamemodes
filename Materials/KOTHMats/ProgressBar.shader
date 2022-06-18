Shader "Unlit/ProgressBar"
{
	Properties
	{
		_Progress ("Progress", Range(0, 1)) = 0.5
		_BaseColor ("Base Color", Color) = (0, 0, 0, 0)
		_ProgressColor ("Progress Color", Color) = (0, 0, 0, 0)
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

			float4 _BaseColor, _ProgressColor;
			float _Progress;
			
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
				if(i.uv.x < _Progress){
					return _ProgressColor;
				}
				
				return _BaseColor;
			}
			ENDCG
		}
	}
}
