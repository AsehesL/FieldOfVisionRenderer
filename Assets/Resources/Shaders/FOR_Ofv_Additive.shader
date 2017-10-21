// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FieldOfRange/FOR_Ofv_Additive"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_DepthTex("DepthTex", 2D) = "white" {}
		_Range("Range", float) = 0
		_Fade ("Fade", float) = 0.001
	}
	CGINCLUDE
	
	#include "UnityCG.cginc"

	struct appdata_fov
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float4 color : COLOR;
	};

	struct v2f_fov
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float4 color : COLOR;
		float4 proj : TEXCOORD2;
		float depth : TEXCOORD3;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _DepthTex;
	fixed _Fade;

	float4x4 internalWorldToCamera;
	float4x4 internalCameraToProj;
	half _Range;

	v2f_fov vert_fov(appdata_fov v)
	{
		v2f_fov o;
		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		UNITY_TRANSFER_FOG(o, o.vertex);
		o.color = v.color;

		worldPos = mul(internalWorldToCamera, worldPos);

		o.depth = -worldPos.z / _Range;

		worldPos = mul(internalCameraToProj, worldPos);

		o.proj = ComputeScreenPos(worldPos);

		return o;
	}

	#define APPLY_FOV(i, c)	half2 pjuv = i.proj.xy/i.proj.w;  \
						pjuv.y = 0.5; \
						half dep = DecodeFloatRGBA(tex2D(_DepthTex, pjuv)); \
						if (i.depth > dep) \
							c.a = 0 \

	ENDCG
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			zwrite off
			blend srcalpha one
			CGPROGRAM
			#pragma vertex vert_fov
			#pragma fragment frag
			
			#pragma multi_compile_fog
			
			fixed4 frag (v2f_fov i) : SV_Target
			{
				fixed2 toUV = i.uv - fixed2(0.5,0.5);
				fixed ag = atan(abs(toUV.y / toUV.x));
				fixed dis = length(toUV);
				fixed4 col = tex2D(_MainTex,i.uv)*i.color;
				col.a *= 1 - saturate((dis - 0.5+ _Fade) / _Fade);
				UNITY_APPLY_FOG(i.fogCoord, col);
				APPLY_FOV(i, col);
				return col;
			}
			ENDCG
		}
		/*Pass
		{
			zwrite off
			blend srcalpha one
			CGPROGRAM
			#pragma vertex vert_fov
			#pragma fragment frag

			#pragma multi_compile_fog

			fixed4 frag(v2f_fov i) : SV_Target
			{
				fixed2 toUV = i.uv - fixed2(0.5,0.5);
				fixed ag = atan(abs(toUV.y / toUV.x));
				fixed dis = length(toUV);
				fixed4 col = tex2D(_MainTex,float2(dis*2,0))*i.color;
				col.a *= 1 - saturate((dis - 0.5 + _Fade) / _Fade);
				UNITY_APPLY_FOG(i.fogCoord, col);
				APPLY_FOV(i, col);
				return col;
			}
			ENDCG
		}*/
	}
}
