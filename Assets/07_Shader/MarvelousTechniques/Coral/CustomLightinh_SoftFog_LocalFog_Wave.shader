Shader "0_Coral/Art/CustomLightinh_SoftFog_LocalFog_Wave"
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

		[Space(30)]
		[Toggle(USE_TEXCOORD1)] _UseTexcoord1("Use Wave", Float) = 0
		_UV_Speed ("UV_Speed", Float ) = 1
        _UV_SpeedR ("UV_SpeedR", Float ) = -1
        _Main_SpeedR ("Main_SpeedR", Float ) = -5
        _Bright ("Bright", Float ) = 0
        _Density ("Density", Float ) = 0.05
        _Main_Speed ("Main_Speed", Float ) = 5
        _Power ("Power", Float ) = 0
        _Wave_Color ("Wave_Color", Color) = (0.5,0.5,0.5,1)
        _Wave_Tex ("Wave_Tex", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Mask_Int ("Mask_Int", Float ) = 0
        _LOD ("LOD Bias", Range (1, 1000)) = 100
		_LimitY ("LimitY", Float) = 0
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
			#pragma shader_feature USE_LOCAL_HEIGHT_FOG
			#pragma shader_feature USE_HEIGHT_FOG
			#pragma shader_feature USE_DIST_FOG
			#pragma shader_feature USE_TEXCOORD1
			
			
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

			uniform half4 _TimeEditor;
            uniform half _UV_Speed;
            uniform half _Density;
            uniform half _Main_Speed;
            uniform half _Bright;
            uniform half _Power;
            uniform sampler2D _Wave_Tex; uniform half4 _Wave_Tex_ST;
            uniform half _Main_SpeedR;
            uniform half _UV_SpeedR;
            uniform half4 _Wave_Color;
            uniform sampler2D _Mask; uniform half4 _Mask_ST;
            uniform half _Mask_Int;
			float _LOD;
			float dist;


			#include "../ABSW_Marvelous.cginc"

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
				fixed4 customcolor;

#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				customcolor = customLightingSoftFogFrag(v, 
				_FogColor, _FogHeight, _FogYStartPos, _FogAnimationHeight, _FogAnimationFreq,
				_LightTint, 
				_LocalFogColor, _LocalFogStartY, _LocalFogHeight,
				_FogDistanceColor,
				_ColorBoost);
#else
				customcolor = customLightingFrag(v, _LightTint, _ColorBoost);

				//return customLightingFrag(v, _LightTint, _LightmapPower, _LightmapColor, _LightmapShadowPower, _ColorBoost);
#endif

#if USE_TEXCOORD1
                //half4 node_550 = _Time + _TimeEditor;

				half wavetime = sin(_Time.x);

				//half4 node_550 = half4(0,0,0,0);
                half2 node_764 = half2(v.uv1.r,(v.uv1.g+(wavetime*_UV_Speed)));
                half4 _Wave_Tex_copy = tex2D(_Wave_Tex,TRANSFORM_TEX(node_764, _Wave_Tex));

                half2 node_4177 = (half2((v.uv1.r+(wavetime*_Main_Speed)),v.uv1.g)+(_Wave_Tex_copy.b*_Density));
                half4 node_4920 = tex2D(_Wave_Tex,TRANSFORM_TEX(node_4177, _Wave_Tex));

                half2 node_3790 = half2(v.uv1.r,(v.uv1.g+(wavetime*_UV_SpeedR)));
                half4 node_5920 = tex2D(_Wave_Tex,TRANSFORM_TEX(node_3790, _Wave_Tex));

                half2 node_6461 = (half2(v.uv1.g,(v.uv1.r+(wavetime*_Main_SpeedR)))+(node_5920.r*_Density));
                half4 node_6833 = tex2D(_Wave_Tex,TRANSFORM_TEX(node_6461, _Wave_Tex));


                half4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(v.uv1, _Mask));
                half3 emissive = ((saturate(pow(((node_4920.r+node_6833.r)*_Bright),_Power))*_Wave_Color.rgb)*(_Mask_var.r*_Mask_Int));
                half3 finalColor = emissive;

				//customcolor = _Wave_Tex_copy;
				customcolor += fixed4(finalColor, 1);
#endif

				return customcolor;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
