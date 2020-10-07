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


half        _Cutoff;
//MASK
uniform float _MaskType;
uniform float _CutoffAxis;
uniform float _MaskOffset;
uniform float4 _MaskWorldPosition;

//EDGE
uniform float3 _EdgeColor;
uniform sampler2D _EdgeRampMap;
uniform float4 _EdgeRampMap_ST;
uniform float2 _EdgeRampMap_Scroll;
uniform float _EdgeSize;
uniform float _EdgeStrength;

//DISSOLVE
uniform float3 _DissolveEdgeColor;
uniform float _DissolveEdgeSize;
uniform float _DissolveSize;
sampler2D _DissolveMap;
uniform float4 _DissolveMap_ST;
uniform float2 _DissolveMap_Scroll;

//half4       _EmissionColor;
//sampler2D   _EmissionMap;

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


#ifdef USE_LAYOUT_TEXTURE
sampler2D _LayoutTexture;
float4 _LayoutTexture_ST;
#endif

#ifdef USE_CUSTOM_LIGHTMAP
sampler2D _LightmapTexture;
float4 _LightmapTexture_ST;
#endif

struct CL_IN {
	half4 vertex : POSITION;
	half3 normal : NORMAL;

	half3 color : COLOR;

#ifdef USE_MAIN_TEX
	float4 texcoord : TEXCOORD0;
#endif
#ifdef USE_LIGHTMAP
	half4 texcoord1 : TEXCOORD1;
#endif
#ifdef USE_LOCAL_HEIGHT_FOG
	half4 local_height : TEXCOORD2; // local height info
#endif
#ifdef USE_LAYOUT_TEXTURE
	half4 texcoord3 : TEXCOORD3;
#endif

#ifdef USE_DISSOLVE
	half4 texcoord4 : TEXCOORD4;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID	//해당 구조체를 Instance화 하기 위한 코드

};

struct CL_OUT_WPOS {

	half4 pos : SV_POSITION;

#ifdef USE_MAIN_TEX
#ifdef USE_LIGHTMAP
	half4 uv : TEXCOORD0;
#else
	half2 uv : TEXCOORD0;
#endif
#else
#ifdef USE_LIGHTMAP
	half2 uv : TEXCOORD0;
#endif
#endif	

	half3 lighting : TEXCOORD2;
#if defined(USE_DIST_FOG) || defined(USE_HEIGHT_FOG) || defined(USE_DIST_LIGHT)
	// || (defined(USE_SPECULAR) && defined(USE_SPECULAR_PIXEL_SHADING))
//#ifdef USE_DIST_FOG
	half4 wpos: TEXCOORD3;
#endif

#ifdef USE_LAYOUT_TEXTURE
	half2 layouttexture_uv : TEXCOORD4;
#endif	

//#if (defined(USE_SPECULAR) && defined(USE_SPECULAR_PIXEL_SHADING))
//	half3 normal : TEXCOORD5;
//#endif

#ifdef USE_LOCAL_HEIGHT_FOG
	half4 localpos : TEXCOORD5; // local height info
#endif

#ifdef USE_REALTIME_SHADOWS
	SHADOW_COORDS(6)
#endif

#if  defined(USE_HEIGHT_FOG) || defined(USE_DIST_FOG)
		half4 color : TEXCOORD7;
#else
		half3 color : TEXCOORD7;
#endif

	half4 dissolveUV : TEXCOORD8;

	UNITY_VERTEX_INPUT_INSTANCE_ID //해당 구조체를 Instance화 하기 위한 코드
		UNITY_VERTEX_OUTPUT_STEREO
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
//#endif

	half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;
//#if (defined(USE_SPECULAR) && defined(USE_SPECULAR_PIXEL_SHADING))
//	o.normal = normal;
//#endif
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


#ifdef USE_LAYOUT_TEXTURE	
	o.layouttexture_uv = TRANSFORM_TEX(v.texcoord3, _LayoutTexture);
#endif

#ifdef USE_MAIN_TEX
	o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
#ifdef USE_LIGHTMAP
#ifdef USE_CUSTOM_LIGHTMAP
	o.uv.zw = v.texcoord1.xy * _LightmapTexture_ST.xy + _LightmapTexture_ST.zw;
#else
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#endif
#else
#ifdef USE_LIGHTMAP
#ifdef USE_CUSTOM_LIGHTMAP
	o.uv.xy = v.texcoord1.xy * _LightmapTexture_ST.xy + _LightmapTexture_ST.zw;
#else
	o.uv.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#endif
#endif

	half rim = 1 - (f_d + r_d + t_d);
	o.lighting = calculateFinalLighting(f_color, r_color, t_color, f_d, r_d, t_d, rimColor, rim, rimPower);

#ifdef USE_LIGHT_PROBES
	o.lighting *= lerp(half3(1, 1, 1), ShadeSH9(half4 (normal, 1.0)), _LightProbePower);
#endif

//#if defined(USE_SPECULAR) && !defined(USE_SPECULAR_PIXEL_SHADING)
//	fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//	fixed3 viewDirection = normalize(UnityWorldSpaceViewDir(wpos));
//	o.lighting += specularLight(viewDirection, normal, lightDirection);
//#endif

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


// Vert Dissolve
void customLightingDissolve(float2 mainUv, inout float4 mfxUv)
{
#if USE_LIGHTMAP
	mfxUv.xy = mainUv.xy * _EdgeRampMap_ST.xy + _EdgeRampMap_ST.zw + _EdgeRampMap_Scroll * _Time.y;
	mfxUv.zw = mainUv.xy * _DissolveMap_ST.xy + _DissolveMap_ST.zw + _DissolveMap_Scroll * _Time.y;
#endif
}

// Frag
fixed4 customLightingFrag(CL_OUT_WPOS v, 
	half3 lightTint, 
	half lightmapPower, half3 lightmapColor, half lightmapShadowPower, 
	half colorBoost = 1) {

	fixed4 outColor = fixed4(0.0, 0.0, 0.0, 1.0);

#ifdef USE_MAIN_TEX
	half4 textureColor = tex2D(_MainTex, v.uv.xy) * colorBoost;
	#ifdef MASTER_SHADER
		textureColor.xyz = lerp(half3(1, 1, 1), textureColor.xyz, _MainTexPower);
	#endif
	outColor.w = textureColor.w;
#else
	half4 textureColor = half4(1, 1, 1, 1) * colorBoost;
#endif

#ifdef USE_LAYOUT_TEXTURE
	half4 cMap = tex2D(_LayoutTexture, v.layouttexture_uv);
	textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz)*lerp(half3(1, 1, 1), cMap, _LayoutTexturePower);
#else
	textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz);
#endif

#if USE_LIGHTMAP
	half3 realLightmapPower;

	#ifdef USE_MAIN_TEX
		half2 lm_uv = v.uv.zw;
	#else
		half2 lm_uv = v.uv.xy;
	#endif

	#ifdef USE_CUSTOM_LIGHTMAP
		fixed4 bakedColorTex = tex2D(_LightmapTexture, lm_uv);
	#else
		fixed4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lm_uv);
	#endif

	realLightmapPower = lerp(half3(1, 1, 1), clamp(DecodeLightmap(bakedColorTex) + (lightmapShadowPower), 0, 1), lightmapPower);
	outColor.xyz = lerp(lightmapColor.xyz, half3(1, 1, 1), realLightmapPower) * textureColor.xyz;
#else
	outColor = textureColor * 1;
#endif
	outColor.xyz *= v.color;

#ifdef USE_DIST_LIGHT
	outColor = calculteDistanceLight(outColor, v);
#endif

	//#if defined(USE_SPECULAR) && defined(USE_SPECULAR_PIXEL_SHADING)
	//	fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	//	fixed3 viewDirection = normalize(UnityWorldSpaceViewDir(v.wpos));
	//	outColor.xyz += specularLight(viewDirection, normalize(v.normal), lightDirection);
	//#endif

	return outColor;
}

fixed4 customLightingSoftFogFrag(CL_OUT_WPOS v, 
	half3 fogColor, half fogHeight, half fogStartY, half animationHeight, half fogAnimationFreq,
	half3 lightTint,
	half lightmapPower, half3 lightmapColor, half lightmapShadowPower, 	
	half3 localFogColor, half localFogStartY, half localFogHeight,
	half3 disfogColor,
	half colorBoost = 1)
{
	fixed4 outColor = customLightingFrag(v, lightTint, lightmapPower, lightmapColor, lightmapShadowPower, colorBoost);

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


inline float GetMfxLocalPosition(float3 vertexPos)
{
	float pos = mul(unity_WorldToObject, float4(vertexPos, 1))[(int)_CutoffAxis];
	return pos;
}

inline float GetMfxGlobalPosition(float3 vertexPos)
{
	float pos = (vertexPos)[(int)_CutoffAxis];
	return pos;
}
inline float GetMfxLengthGlobalPosition(float3 vertexPos)
{
	return length(_MaskWorldPosition - vertexPos);
}
inline float GetMfxSinglePosition(float3 vertexPos)
{
#if defined(_MASKTYPE_AXIS_LOCAL)
	#ifdef INVERSPOSTION
	return 1-GetMfxLocalPosition(vertexPos);
	#else
	return GetMfxLocalPosition(vertexPos);
	#endif
#elif defined (_MASKTYPE_AXIS_GLOBAL)
	#ifdef INVERSPOSTION
	return 1 - GetMfxGlobalPosition(vertexPos);
	#else
	return GetMfxGlobalPosition(vertexPos);
	#endif
#elif defined (_MASKTYPE_GLOBAL)
	#ifdef INVERSPOSTION
	return 1-GetMfxLengthGlobalPosition(vertexPos);
	#else
	return GetMfxLengthGlobalPosition(vertexPos);
	#endif
#endif
	return 1;
}
inline float GetAlpha(float2 mfxDissolveUv, float3 vertexPos)
{
	float pos = GetMfxSinglePosition(vertexPos);
	float mask_pos = (pos - _MaskOffset);
	//float alpha = (_DissolveSize + (mask_pos - (_MaskOffset - tex2D(_DissolveMap, mfxDissolveUv).r)));
	float alpha = (_DissolveSize + (mask_pos - (_MaskOffset - 1)));
	return alpha;
}
//--------------
// DISSOLVE
inline float GetDissolveAlpha(float4 mfxUv, float3 vertexPos)
{
#if defined(_MASKTYPE_AXIS_LOCAL) || defined(_MASKTYPE_AXIS_GLOBAL) || defined(_MASKTYPE_GLOBAL)
	float alpha = GetAlpha(mfxUv.zw, vertexPos);
	return alpha;
#elif defined(_MASKTYPE_NONE)
	float alpha = tex2D(_DissolveMap, mfxUv.zw).r;
	return alpha;
#endif
	return 1;
}
inline void DissolveClip(float alpha)
{
	clip(alpha - _Cutoff);
}
fixed4 customLightingDissolve(CL_OUT_WPOS v, fixed4 basecolor)
{
	float3 wpos = float3(0, 0, 0);
#if defined(USE_DIST_FOG) || defined(USE_HEIGHT_FOG) || defined(USE_DIST_LIGHT)
	wpos = v.wpos;
#endif
	float alpha = GetDissolveAlpha(v.dissolveUV, wpos);

	DissolveClip(alpha);

	float pos = GetMfxSinglePosition(wpos);
	float mask_pos = pos - _MaskOffset;
	float edge_pos = (mask_pos - (_MaskOffset - tex2D(_EdgeRampMap, v.dissolveUV.xy).r));
	float scaled_edge = ((50.0 + (_EdgeSize - 0.0) * (0.0 - 50.0) / (1.0 - 0.0)) * edge_pos);
	float clamp_scaled_edge = clamp(scaled_edge, 0.0, 1.0);
	float edge = clamp((1.0 - abs(scaled_edge)), 0.0, 1.0);
	float edge_threshold = ((1.0 - clamp_scaled_edge) - edge);

	float emissionMap = clamp((((1.0 - tex2D(_EmissionMap2, v.dissolveUV.xy).r) - 0.5) * 3.0), 0.0, 1.0);
	float3 emission2 = (_EmissionColor2 * (pow(emissionMap, 3.0) * saturate(((mask_pos - _MaskOffset) + (0.0 + (_EmissionSize2 - 0.0) * (3.0 - 0.0) / (1.0 - 0.0))))));
	//float3 emission2_base = lerp(basecolor, emission2, edge_threshold);
	float3 emission2_base = emission2;
	float alpha_original = alpha + _Cutoff;

	float edge_emission = smoothstep((1.0 - _EdgeSize), 1.0, edge);

	//float3 final_emission = ((alpha <= _DissolveEdgeSize) ? _DissolveEdgeColor : emission2_base + (((1.0 + (_EdgeStrength - 0.0) * (0.1 - 1.0) / (1.0 - 0.0)) <= edge_emission) ? _EdgeColor : (_EdgeColor * edge_emission))));
	float3 final_emission = ((alpha <= _DissolveEdgeSize) ? _DissolveEdgeColor :
		(emission2_base + (((1.0 + (_EdgeStrength - 0.0) * (0.1 - 1.0) / (1.0 - 0.0)) <= edge_emission) ? _EdgeColor :
			(_EdgeColor * edge_emission))));

	return fixed4(final_emission, alpha);
	//return fixed4(1,1,1, alpha);
}
#endif