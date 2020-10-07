Shader "0_Coral/Art/CustomLightinh_SoftFog_LocalFog_Alpha"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0,1)) = 0

		[Space(30)]
		[Toggle(USE_LAYOUT_TEXTURE)] _UseLayoutTexture("Layout Texture", Float) = 0
		_LayoutTexture("Layout Texture", 2D) = "white" {}
		_LayoutTexturePower("Layout Texture Power", Range(0,1)) = 0.5

		[Space(30)]
		_TopColor("Top Color", Color) = (1,1,1,0)
		_RightColor("Right Color", Color) = (0.9,0.9,0.9,0)
		_FrontColor("Front Color", Color) = (0.7,0.7,0.7,0)
		_MainColor("Main Color", Color) = (0.7,0.7,0.7,0)
		_RimColor("Rim Color", Color) = (0,0,0,0)
		_RimPower("Rim Power", Float) = 0.0
		_ColorBoost("Color Boost" , Range(1,5)) = 1

		[Space(30)]
		_LightTint("Light Tint", Color) = (1,1,1,0)
		_AmbientColor("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
		_AmbientPower("Ambient Power", Float) = 0.0

		[Space(30)]
		[Toggle(USE_LOCAL_HEIGHT_FOG)] _UseLocalSpace("Local Height Fog", Float) = 0
		_LocalFogColor("Local Fog Color", Color) = (1,0.73,0.117,0)
		_LocalFogStartY("Local Fog Y-start pos", Float) = 0
		_LocalFogHeight("Local Fog Height", Float) = 0.1

		[Space(30)]
		[Toggle(USE_HEIGHT_FOG)] _UseFog("Height Fog", Float) = 0
		_FogColor("Fog color", Color) = (1,1,1,1)
		_FogYStartPos("Fog Y-start pos", Float) = 0
		_FogHeight("Fog Height", Float) = 0.1
		_FogAnimationHeight("Fog Animation Height", Float) = 0.1
		_FogAnimationFreq("Fog Animation Frequency", Float) = 0.1

		[Space(30)]
		[Toggle(USE_DIST_FOG)] _UseFogDistance("Distance Fog", Float) = 0
		_FogDistanceColor("Distance Fog Color" , Color) = (1,1,1,1)
		_FogDistanceStart("Distance Start", Float) = 0
		_FogDistanceEnd("Distance End", Float) = 50
		_FogDistanceDensity("Distance Density", Float) = 1

		[Space(30)]
		[Toggle(USE_LIGHTMAP)] _UseLightMap("Lightmap Enabled", Float) = 0
		_LightmapColor("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower("Lightmap Power", Float) = 1
		_LightmapShadowPower("Lightmap Light", Float) = 1
		[Space(30)]
		[Toggle(USE_CUSTOM_LIGHTMAP)] _UseCustomLightmap("Custom Lightmap", Float) = 0
		_LightmapTexture("Lightmap Texture", 2D) = "white" {}

		[Space(30)]
		[HideInInspector]_LightDirF("Front Light Direction", Vector) = (0,0,-1)
		[HideInInspector]_LightDirT("Top Light Direction", Vector) = (0,1,0)
		[HideInInspector]_LightDirR("Right Light Direction", Vector) = (1,0,0)
		[Toggle(USE_DIR_LIGHT)] _UseDirLight("Directional Light", Float) = 0
    }
    SubShader
    {
		//Tags { "QUEUE" = "Geometry" "RenderType" = "Opaque" }
        //LOD 200
		Tags { "QUEUE" = "Transparent" "RenderType" = "Opaque" }
        Pass
        {
			Cull Back
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			

            CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#define USE_MAIN_TEX;
			#pragma shader_feature USE_LIGHTMAP
			#pragma shader_feature USE_DIR_LIGHT
			#pragma shader_feature USE_LOCAL_HEIGHT_FOG
			#pragma shader_feature USE_HEIGHT_FOG
			#pragma shader_feature USE_DIST_FOG
			#pragma shader_feature USE_CUSTOM_LIGHTMAP
			#pragma shader_feature USE_LAYOUT_TEXTURE
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			uniform half _Alpha;

			uniform half _LayoutTexturePower;
			uniform half3 _TopColor;
			uniform half3 _RightColor;
			uniform half3 _FrontColor;

			uniform half3 _MainColor;

			uniform half3 _RimColor;
			uniform half _RimPower;
			uniform half _ColorBoost;

			uniform half3 _LightTint;
			uniform half3 _AmbientColor;
			uniform half _AmbientPower;

			uniform half3 _LocalFogColor;
			uniform half _LocalFogStartY;
			uniform half _LocalFogHeight;

			uniform half3 _FogColor;
			uniform half _FogYStartPos;
			uniform half _FogHeight;
			uniform half _FogAnimationHeight;
			uniform half _FogAnimationFreq;

			uniform half3 _FogDistanceColor;
			uniform half _FogDistanceStart;
			uniform half _FogDistanceEnd;
			uniform half _FogDistanceDensity;

			uniform half _LightmapPower;
			uniform half3 _LightmapColor;
			uniform half _LightmapShadowPower;

			#include "../ABSW2Marvelous.cginc"

			CL_OUT_WPOS vert(CL_IN v)
            {
#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				return customLightingSimpleSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, 
					_FogDistanceStart, _FogDistanceEnd, _FogDistanceDensity,
					_AmbientColor, _AmbientPower);
#else
				return customLightingWPosVertSimple(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
#endif
            }

			fixed4 frag(CL_OUT_WPOS v) : COLOR
            {

				fixed4 o = fixed4(1,1,1,1);
#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				o = customLightingSoftFogFrag(v, 
				_FogColor, _FogHeight, _FogYStartPos, _FogAnimationHeight, _FogAnimationFreq,
				_LightTint, 
				_LightmapPower, _LightmapColor, _LightmapShadowPower, 
				_LocalFogColor, _LocalFogStartY, _LocalFogHeight,
				_FogDistanceColor,
				_ColorBoost);
#else
				o = customLightingFrag(v, _LightTint, _LightmapPower, _LightmapColor, _LightmapShadowPower, _ColorBoost);
#endif

				o.a *=  _Alpha;
				return o;
            }
            ENDCG
        }
    }
	//FallBack "Diffuse"
}
