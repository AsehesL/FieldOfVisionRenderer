#ifndef FOR_CG_INC
#define FOR_CG_INC

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

half4 _FORParams;

v2f_fov vert_fov(appdata_fov v)
{
	v2f_fov o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	UNITY_TRANSFER_FOG(o, o.vertex);
	o.color = v.color;

	float4 camPos = v.vertex;
	camPos.z *= -1;

	o.depth = -camPos.z / _FORParams.x;

	camPos = float4(camPos.x*_FORParams.y, -0.5*camPos.z, camPos.z, -camPos.z);
	o.proj = ComputeScreenPos(camPos);

	return o;
}

#define APPLY_FOV(i, c)	half2 pjuv = half2(i.proj.x/i.proj.w, 0.5);  \
					half dep = DecodeFloatRGBA(tex2D(_DepthTex, pjuv)); \
						clip(dep - i.depth); \

#endif