Shader "0_Coral/Art/CustomLighting_SoftFog_Fish"
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
		[Toggle(USE_DIST_FOG)] _UseFogDistance("Distance Fog", Float) = 0
		_FogDistanceColor("Distance Fog Color" , Color) = (1,1,1,1)
		_FogDistanceStart("Distance Start", Float) = 0
		_FogDistanceEnd("Distance End", Float) = 50
		_FogDistanceDensity("Distance Density", Float) = 1
    }
    SubShader
    {
		Tags { "QUEUE" = "Geometry" "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
			Tags { "LIGHTMODE" = "ForwardBase" "QUEUE" = "Geometry" "RenderType" = "Opaque" }

            CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#define USE_MAIN_TEX;
			#pragma shader_feature USE_DIST_FOG
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

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

			uniform half3 _FogDistanceColor;
			uniform half _FogDistanceStart;
			uniform half _FogDistanceEnd;
			uniform half _FogDistanceDensity;

			#include "../ABSW_Marvelous.cginc"

			CL_OUT_WPOS vert(CL_IN v)
            {
#if USE_DIST_FOG
				return customLightingSimpleSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, 
					_FogDistanceStart, _FogDistanceEnd, _FogDistanceDensity,
					_AmbientColor, _AmbientPower);
#else
				return customLightingWPosVertSimple(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
#endif
            }

			fixed4 frag(CL_OUT_WPOS v) : COLOR
            {
#if USE_DIST_FOG
				return customLightingDistFog_Frag(v, 
				_LightTint, 
				_FogDistanceColor,
				_ColorBoost);

#else
				return customLightingFrag(v, _LightTint, _ColorBoost);
#endif
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
