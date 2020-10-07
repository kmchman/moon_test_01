Shader "0_Coral/Art/CustomLighting_SoftFog_LocalFog"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}

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
    }
    SubShader
    {
		Tags { "QUEUE" = "Geometry" "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
			Tags { "LIGHTMODE" = "ForwardBase" "QUEUE" = "Geometry" "RenderType" = "Opaque" }

            CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#define USE_MAIN_TEX;
			#pragma shader_feature USE_LOCAL_HEIGHT_FOG
			#pragma shader_feature USE_HEIGHT_FOG
			#pragma shader_feature USE_DIST_FOG
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			//#include "UnityCG.cginc"

			uniform half3 _TopColor;
			uniform half3 _RightColor;
			uniform half3 _FrontColor;

			uniform half3 _MainColor;

			uniform half3 _RimColor;
			uniform half _RimPower;
			//uniform half _ColorBoost;
			//uniform half3 _LightTint;

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

			#include "../ABSW_Marvelous.cginc"

			//GPU Instancing을 사용하기 위한 기능
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(half3, _LightTint)
			UNITY_DEFINE_INSTANCED_PROP(half, _ColorBoost)
			UNITY_INSTANCING_BUFFER_END(Props)

			CL_OUT_WPOS vert(CL_IN v)
            {
				CL_OUT_WPOS o;
				UNITY_SETUP_INSTANCE_ID(v);

#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				o = customLightingSimpleSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, 
					_FogDistanceStart, _FogDistanceEnd, _FogDistanceDensity,
					_AmbientColor, _AmbientPower);
#else
				o = customLightingWPosVertSimple(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
#endif

				UNITY_TRANSFER_INSTANCE_ID(v, o);
				return o;
            }

			fixed4 frag(CL_OUT_WPOS i) : COLOR
            {

				UNITY_SETUP_INSTANCE_ID(i);
				half3 lighttint = UNITY_ACCESS_INSTANCED_PROP(Props, _LightTint);
				half colorboost = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorBoost);

#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				return customLightingSoftFogFrag(i, 
				_FogColor, _FogHeight, _FogYStartPos, _FogAnimationHeight, _FogAnimationFreq,
				lighttint, 
				_LocalFogColor, _LocalFogStartY, _LocalFogHeight,
				_FogDistanceColor,
				colorboost);
#else
				return customLightingFrag(i, lighttint, colorboost);
#endif
            }
            ENDCG
        }
    }
	//FallBack "Diffuse"
}
