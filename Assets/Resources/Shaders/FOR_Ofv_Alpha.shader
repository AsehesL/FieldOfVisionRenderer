// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FieldOfRange/FOR_Ofv_AlphaBlend"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_DepthTex("DepthTex", 2D) = "white" {}
		_FORParams("FORParams", vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			zwrite off
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "FOR.cginc"
			#pragma vertex vert_fov
			#pragma fragment frag
			
			#pragma multi_compile_fog
			
			fixed4 frag (v2f_fov i) : SV_Target
			{
				fixed2 toUV = i.uv - fixed2(0.5,0.5);
				fixed ag = atan(abs(toUV.y / toUV.x));
				fixed dis = length(toUV);
				fixed4 col = tex2D(_MainTex,i.uv)*i.color;
				col.a *= 1 - saturate((dis - 0.5+ 0.001) / 0.001);
				UNITY_APPLY_FOG(i.fogCoord, col);
				APPLY_FOV(i, col);
				return col;
			}
			ENDCG
		}
	}
}
