#ifndef MARVELOUS_INCLUDED
#define MARVELOUS_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"
//#include "UnityPBSLighting.cginc"

#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE) || !defined (USE_REALTIME_SHADOWS)
#define COPY_SHADOW_COORDS(o2,o)
#else
#define COPY_SHADOW_COORDS(o2,o) o2._ShadowCoord=o._ShadowCoord;
#endif



//EDGE
uniform float3 _EdgeColor;
uniform sampler2D _EdgeRampMap;
uniform float4 _EdgeRampMap_ST;
uniform float2 _EdgeRampMap_Scroll;
uniform float _EdgeSize;
uniform float _EdgeStrength;

//EMISSION 2
uniform float3 _EmissionColor2;
uniform float _EmissionSize2;
sampler2D _EmissionMap2;
uniform float4 _EmissionMap2_ST;


// Light.
#if USE_DIR_LIGHT
uniform half3 _LightDirF;
uniform half3 _LightDirT;
uniform half3 _LightDirR;
#endif

#ifdef USE_MAIN_TEX
sampler2D _MainTex;
float4 _MainTex_ST;
#endif

struct CL_IN {
	half4 vertex : POSITION;
	half3 normal : NORMAL;
	half3 color : COLOR;

#ifdef USE_MAIN_TEX
	float4 texcoord : TEXCOORD0;
#endif

#if defined(USE_LOCAL_HEIGHT_FOG) || defined(USE_TEXCOORD1)
	half4 local_height : TEXCOORD1; // local height info
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID	//해당 구조체를 Instance화 하기 위한 코드

};

struct CL_OUT_WPOS {

	half4 pos : SV_POSITION;

#ifdef USE_MAIN_TEX
	half4 uv : TEXCOORD0;
#endif	

#ifdef USE_LOCAL_HEIGHT_FOG
	half4 localpos : TEXCOORD1; // local height info
#endif

	half3 lighting : TEXCOORD2;
#if defined(USE_DIST_FOG) || defined(USE_HEIGHT_FOG) || defined(USE_DIST_LIGHT)
	half4 wpos: TEXCOORD3;
#endif

#if  defined(USE_HEIGHT_FOG) || defined(USE_DIST_FOG)
		half4 color : TEXCOORD4;
#else
		half3 color : TEXCOORD4;
#endif

#ifdef USE_REALTIME_SHADOWS
	SHADOW_COORDS(6)
#endif

#ifdef USE_TEXCOORD1
	half2 uv1 : TEXCOORD5;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID //해당 구조체를 Instance화 하기 위한 코드
	//UNITY_VERTEX_OUTPUT_STEREO
};


inline half3 calculateFinalLighting(half3 f_color, half3 r_color, half3 t_color, half f_d, half r_d, half t_d, half3 rimColor, half rim, half rimPower) {

	return (f_color*f_d) + (r_color*r_d) + (t_color*t_d) + (rimColor*rim*rimPower);
}

CL_OUT_WPOS calculateLighting(CL_IN v, half3 rimColor, half rimPower, half3 f_color, half3 r_color, half3 t_color) {

	CL_OUT_WPOS o;
	o.pos = UnityObjectToClipPos(v.vertex);
	half4 wpos1 = mul(unity_ObjectToWorld, half4(v.vertex.xyz, 1));

#if defined(USE_HEIGHT_FOG) || defined(USE_DIST_LIGHT) || defined(USE_DIST_FOG)// || (defined(USE_SPECULAR)&& defined(USE_SPECULAR_PIXEL_SHADING))
	o.wpos = wpos1;
#endif

#if defined(USE_LOCAL_HEIGHT_FOG)
	o.localpos = v.local_height;
#endif

#if defined(USE_TEXCOORD1)
	o.uv1 = v.local_height;
#endif

	half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;
	o.color.rgb = v.color.rgb;

#if defined(USE_HEIGHT_FOG) || defined(USE_DIST_FOG)
	o.color.a = 1;
#endif

#if USE_DIR_LIGHT
	half f_d = acos((dot(_LightDirF, normal))) / 1.5708;
	half r_d = acos((dot(_LightDirR, normal))) / 1.5708;
	half t_d = acos((dot(_LightDirT, normal))) / 1.5708;

	f_d = lerp(0, f_d - 1, f_d > 1);
	r_d = lerp(0, r_d - 1, r_d > 1);
	t_d = lerp(0, t_d - 1, t_d > 1);

#else
	half f_d = acos(clamp(dot(half3(0, 0, -1), half3(0, 0, normal.z)), -1, 1)) / 1.5708;
	half r_d = acos(clamp(dot(half3(1, 0, 0), half3(normal.x, 0, 0)), -1, 1)) / 1.5708;
	half t_d = acos(clamp(dot(half3(0, 1, 0), half3(0, normal.y, 0)), -1, 1)) / 1.5708;

	f_d = lerp(0, 1 - f_d, half(normal.z < 0));
	r_d = lerp(0, 1 - r_d, half(normal.x > 0));
	t_d = lerp(0, 1 - t_d, half(normal.y > 0));

#endif


#ifdef USE_MAIN_TEX
	o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif

	half rim = 1 - (f_d + r_d + t_d);
	o.lighting = calculateFinalLighting(f_color, r_color, t_color, f_d, r_d, t_d, rimColor, rim, rimPower);

#ifdef USE_LIGHT_PROBES
	o.lighting *= lerp(half3(1, 1, 1), ShadeSH9(half4 (normal, 1.0)), _LightProbePower);
#endif

#ifdef USE_REALTIME_SHADOWS
	TRANSFER_SHADOW(o);
#endif

	return o;
}

// Vert
CL_OUT_WPOS customLightingWPosVertSimple(CL_IN v, half3 rimColor, half rimPower, half3 r_color, half3 f_color, half3 t_color, half3 ambientColor, float ambientPower) {

	CL_OUT_WPOS o = calculateLighting(v, rimColor, rimPower, f_color, r_color, t_color);
	o.lighting += (ambientColor*ambientPower);
	return o;
}

// fog(height)
CL_OUT_WPOS customLightingSimpleSoftFogVert(CL_IN v, half3 rimColor, half rimPower, half3 rightLight, half3 frontLight, half3 topLight,
	half disfotStart, half distfogEnd, half disfogDensity,
	half3 ambientColor, half ambientPower) {

	CL_OUT_WPOS o = customLightingWPosVertSimple(v, rimColor, rimPower, rightLight, frontLight, topLight, ambientColor, ambientPower);

	//o.lighting = half3(((sin(_Time.y * 10 * _FogAnimationFreq)) + 1)*0.5* _FogAnimationHeight, ((sin(_Time.y * 10 * _FogAnimationFreq)) + 1)*0.5* _FogAnimationHeight, ((sin(_Time.y * 10 * _FogAnimationFreq)) + 1)*0.5* _FogAnimationHeight);
//#if USE_DIST_FOG
#ifdef USE_DIST_FOG
	float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos) *disfogDensity;
	o.color.w = saturate((distfogEnd - cameraVertDist) / (distfogEnd - disfotStart));
#endif

	return o;
}

// Frag
fixed4 customLightingFrag(CL_OUT_WPOS v, 
	half3 lightTint, 
	half colorBoost = 1) {

	fixed4 outColor = fixed4(0.0, 0.0, 0.0, 1.0);

#ifdef USE_MAIN_TEX
	half4 textureColor = tex2D(_MainTex, v.uv.xy) * colorBoost;
	outColor.w = textureColor.w;
#else
	half4 textureColor = half4(1, 1, 1, 1) * colorBoost;
#endif
	textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz);

	outColor = textureColor;
	outColor.xyz *= v.color;

	return outColor;
}

fixed4 customLightingDistFog_Frag(CL_OUT_WPOS v, 	
	half3 lightTint,
	half3 disfogColor,
	half colorBoost = 1)
	{
		fixed4 outColor = customLightingFrag(v, lightTint, colorBoost);
#ifdef USE_DIST_FOG
		outColor = lerp(half4(disfogColor.rgb, 1), outColor, v.color.w);
#endif
		return outColor;
	}

fixed4 customLightingSoftFogFrag(CL_OUT_WPOS v, 
	half3 fogColor, half fogHeight, half fogStartY, half animationHeight, half fogAnimationFreq,
	half3 lightTint,	 	
	half3 localFogColor, half localFogStartY, half localFogHeight,
	half3 disfogColor,
	half colorBoost = 1)
{
	fixed4 outColor = customLightingFrag(v, lightTint, colorBoost);

#ifdef USE_LOCAL_HEIGHT_FOG
	half localfog = saturate((v.localpos.y - localFogStartY) / localFogHeight);
	outColor.xyz = lerp(localFogColor, outColor.xyz, localfog);
#endif

#ifdef USE_HEIGHT_FOG
	half randomval = ((sin(_Time * 10 * _FogAnimationFreq)) + 1)*0.5* _FogAnimationHeight;
	half fogstartpos = fogStartY + randomval;
	half fogDensity = saturate((v.wpos.y - fogstartpos) / fogHeight);//, 0, 1);
	outColor.xyz = lerp(fogColor, outColor.xyz, fogDensity);
#endif

#ifdef USE_DIST_FOG
	outColor = lerp(half4(disfogColor.rgb, 1), outColor, v.color.w);
#endif

	return outColor;
}

#endif