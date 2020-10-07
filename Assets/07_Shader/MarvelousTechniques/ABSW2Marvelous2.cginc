#ifndef MARVELOUS_INCLUDED
#define MARVELOUS_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"

#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE) || !defined (USE_REALTIME_SHADOWS)
#define COPY_SHADOW_COORDS(o2,o)
#else
#define COPY_SHADOW_COORDS(o2,o) o2._ShadowCoord=o._ShadowCoord;
#endif

// Light.
#if USE_DIR_LIGHT
uniform half3 _LightDirF;
uniform half3 _LightDirT;
uniform half3 _LightDirR;
#endif

#ifdef USE_MAIN_TEX
sampler2D _MainTex;
float4 _MainTex_ST;
fixed _Cutoff;
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
	half4 texcoord : TEXCOORD0;
#endif
#ifdef USE_LIGHTMAP
	half4 texcoord1 : TEXCOORD1;
#endif
#ifdef GRADIENT_LOCAL_SPACE
	half4 local_height : TEXCOORD2; // local height info
#endif
#ifdef USE_LAYOUT_TEXTURE
	half4 texcoord3 : TEXCOORD3;
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
#if defined(USE_DIST_FOG) || defined(USE_FOG) || defined(USE_DIST_LIGHT) || defined(USE_GRADIENT)
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

#ifdef USE_REALTIME_SHADOWS
	SHADOW_COORDS(6)
#endif

#if  defined(USE_FOG) || defined(USE_DIST_FOG)
		half4 color : TEXCOORD7;
#else
		half3 color : TEXCOORD7;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID //해당 구조체를 Instance화 하기 위한 코드
		UNITY_VERTEX_OUTPUT_STEREO
};


inline half3 calculateFinalLighting(half ypos, half3 f_color, half3 r_color, half3 t_color, half f_d, half r_d, half t_d, half3 rimColor, half rim, half rimPower) {

//#ifdef USE_GRADIENT
//	half gradient = saturate((ypos - _GradientStartY) / _GradientHeight);
//	f_color = lerp(_FrontColorBottom, f_color, gradient);
//	r_color = lerp(_RightColorBottom, r_color, gradient);
//	t_color = lerp(_TopColorBottom, t_color, gradient);
//	rimColor = lerp(_RimColorBottom, rimColor, gradient);
//#endif
	return (f_color*f_d) + (r_color*r_d) + (t_color*t_d) + (rimColor*rim*rimPower);
}

CL_OUT_WPOS calculateLighting(CL_IN v, half3 rimColor, half rimPower, half3 f_color, half3 r_color, half3 t_color) {

	CL_OUT_WPOS o;
	o.pos = UnityObjectToClipPos(v.vertex);
	half4 wpos1 = mul(unity_ObjectToWorld, half4(v.vertex.xyz, 1));

#if defined(USE_FOG) || defined(USE_DIST_LIGHT) || defined(USE_GRADIENT) || defined(USE_DIST_FOG)// || (defined(USE_SPECULAR)&& defined(USE_SPECULAR_PIXEL_SHADING))
	o.wpos = wpos1;
#endif
//#endif

	half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;
//#if (defined(USE_SPECULAR) && defined(USE_SPECULAR_PIXEL_SHADING))
//	o.normal = normal;
//#endif
	o.color.rgb = v.color.rgb;

#if defined(USE_FOG) || defined(USE_DIST_FOG)
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
	half lpos = wpos1.y;
#ifdef GRADIENT_LOCAL_SPACE
#ifdef GRADIENT_LOCAL_SPACE_INVERSE
	lpos = 1 - v.local_height.z;
#else
	lpos = v.local_height.y;
#endif
#endif

	o.lighting = calculateFinalLighting(lpos, f_color, r_color, t_color, f_d, r_d, t_d, rimColor, rim, rimPower);

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
#ifdef USE_GRADIENT
	half lpos = o.wpos.y;
#ifdef GRADIENT_LOCAL_SPACE
#ifdef GRADIENT_LOCAL_SPACE_INVERSE
	lpos = 1 - v.local_height.z;
#else
	lpos = v.local_height.y;
#endif
#endif
	half gradient = saturate((lpos - _GradientStartY) / _GradientHeight);
	// gradient 색상을 계산된 라이트 색상위에 그리도록 변경.
	//o.lighting = lerp(_MainColorBottom, _MainColor, half3(gradient, gradient, gradient)) * o.lighting + (ambientColor*ambientPower);
	o.lighting *= _MainColor;
	o.lighting = lerp(_MainColorBottom, o.lighting, half3(gradient, gradient, gradient)) + (ambientColor*ambientPower);
#else
	o.lighting = _MainColor * o.lighting + (ambientColor*ambientPower);
#endif
	return o;
}
// fog(height)

CL_OUT_WPOS customLightingSimpleSoftFogVert(CL_IN v, half3 rimColor, half rimPower, half3 rightLight, half3 frontLight, half3 topLight, 
half ambientColor,	half ambientPower, 
half fogStartY, half animationHeight, half fogAnimationFreq) {

	CL_OUT_WPOS o = customLightingWPosVertSimple(v, rimColor, rimPower, rightLight, frontLight, topLight, ambientColor, ambientPower);

#ifdef USE_FOG
	_FogYStartPos += ((sin(_Time * 10 * fogAnimationFreq)) + 1)*0.5* animationHeight;
#endif

//#if USE_DIST_FOG
#ifdef USE_DIST_FOG
	float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos) *_FogDensity;
	o.color.w = saturate((_FogEnd - cameraVertDist) / (_FogEnd - _FogStart));
#endif

	return o;
}










// Frag
fixed4 customLightingFrag(CL_OUT_WPOS v, half3 lightTint, half lightmapPower, half3 lightmapColor, half _ShadowPower, half colorBoost = 1) {
	fixed4 outColor = fixed4(0.0, 0.0, 0.0, 1.0);

	// Texture 색과 Light 색 분리
	// Textyre 색의 a 를 처리하면서 분리함.
	// Light 색 = 레이아웃텍스쳐 * 라이트색(커스텀 라이트)
	half4 lightColor = half4(1,1,1,1);

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
	lightColor.xyz = ((lightTint * v.lighting))*lerp(half3(1, 1, 1), cMap, _LayoutTexturePower);
	//textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz)*lerp(half3(1, 1, 1), cMap, _LayoutTexturePower);
#else
	lightColor.xyz = (v.lighting);
	//lightColor.xyz = (lightTint * v.lighting);
	//textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz);
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

	//realLightmapPower = lerp(half3(1, 1, 1), clamp(DecodeLightmap(bakedColorTex) + (_ShadowPower), 0, 1), lightmapPower);
	/*

	#if defined(UNITY_COLORSPACE_GAMMA)
    # if defined(UNITY_FORCE_LINEAR_READ_FOR_RGBM)
        return (decodeInstructions.x * data.a) * sqrt(data.rgb);
    # else
        return (decodeInstructions.x * data.a) * data.rgb;
    # endif
    #else
        return (decodeInstructions.x * pow(data.a, decodeInstructions.y)) * data.rgb;
    #endif
	
	*/

	//bakedColorTex.rgb = (2.0f * bakedColorTex.a) * sqrt(bakedColorTex.rgb);
	//bakedColorTex.rgb = bakedColorTex.rgb;
	//bakedColorTex.rgb = (2.0f * bakedColorTex.a) * bakedColorTex.rgb;
	//bakedColorTex.rgb = (2.0f * pow(bakedColorTex.a, 2.2f)) * bakedColorTex.rgb;



	// Unity 플랫폼별 라이트맵 계산이 다르게 작동하여 하나로 맞추도록 수작업.
	#if defined(UNITY_LIGHTMAP_DLDR_ENCODING)
    //bakedColorTex.rgb = 2.0f * bakedColorTex.rgb;
		bakedColorTex.rgb = bakedColorTex.rgb;
	#elif defined(UNITY_LIGHTMAP_RGBM_ENCODING)
		#if defined(UNITY_COLORSPACE_GAMMA)
			# if defined(UNITY_FORCE_LINEAR_READ_FOR_RGBM)
				bakedColorTex.rgb = bakedColorTex.rgb;
			# else
				bakedColorTex.rgb = (2.0f * bakedColorTex.a) * sqrt(bakedColorTex.rgb);
			# endif
		#else
			bakedColorTex.rgb = bakedColorTex.rgb;
		#endif
	#else //defined(UNITY_LIGHTMAP_FULL_HDR)
	    bakedColorTex.rgb = (2.0f * bakedColorTex.a) * sqrt(bakedColorTex.rgb);
	#endif

	realLightmapPower = lerp(half3(1, 1, 1), clamp(bakedColorTex + (_ShadowPower), 0, 1), lightmapPower);
	outColor.xyz = lerp(lightmapColor.xyz, half3(1, 1, 1), realLightmapPower) * lightColor.xyz;
	//outColor.xyz = lerp(lightmapColor.xyz, half3(1, 1, 1), realLightmapPower) * textureColor.xyz;
	//outColor.xyz *= lerp(half3(1,1,1), textureColor.xyz, textureColor.a);
	outColor.xyz *= lerp(half3(1,1,1) * lightTint, textureColor.xyz, step(_Cutoff + 0.015, textureColor.a));
	//outColor.xyz *= lerp(half3(1,1,1), textureColor.xyz, step(_Cutoff + 0.015, textureColor.a));
	//outColor.xyz *= lerp(textureColor.xyz, half3(1,1,1), step(textureColor.a, _Cutoff + 0.015));

#else
	outColor.xyz = lightColor.xyz * lerp(half3(1,1,1), textureColor.xyz, step(_Cutoff + 0.015, textureColor.a));
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

fixed4 customLightingSoftFogFrag(CL_OUT_WPOS v, half3 fogColor, half fogHeight, half3 lightTint, half lightmapPower, half3 lightmapColor, half _ShadowPower, half ColorBoost = 1) 
{
	fixed4 outColor = customLightingFrag(v, lightTint, lightmapPower, lightmapColor, _ShadowPower, ColorBoost);

#ifdef USE_FOG
	half fogDensity = clamp((v.wpos.y - _FogYStartPos) / fogHeight, 0, 1);
	outColor.xyz = lerp(fogColor, outColor.xyz, fogDensity);
#endif

#ifdef USE_DIST_FOG
	outColor = lerp(half4(_FogDistanceColor.rgb, 1), outColor, v.color.w);
#endif

	return outColor;
}
#endif