// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/CustomLightingSoftFog_Rim_with_highLight" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
 		_FrontColor ("Front Color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim Color", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
 		[Toggle(USE_LIGHTMAP)]_UseLightMap ("Lightmap Enabled", Float) = 0
		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		_ShadowPower ("Lightmap Light", Float) = 1
		
		_FogColor ("Fog color", Color) = (1,1,1,1)
		_FogYStartPos ("Fog Y-start pos", Float) = 0.1
		_FogHeight ("Fog Height", Float) = 0.1
		_FogAnimationHeight ("Fog Animation Height", Float) = 0.1
		_FogAnimationFreq ("Fog Animation Frequency", Float) = 0.1
		
		[Toggle(USE_DIST_FOG)]_UseFogDistance ("Distance Fog", Float) = 0
		_FogStart ("Distance Start", Float) = 0
 		_FogEnd ("Distance End", Float) = 50
 		_FogDensity ("Distance Density", Float) = 1

 		[HideInInspector]_LightDirF ("Front Light Direction", Vector) = (0,0,-1)
		[HideInInspector]_LightDirT ("Top Light Direction", Vector) = (0,1,0)
		[HideInInspector]_LightDirR ("Right Light Direction", Vector) = (1,0,0)
		[Toggle(USE_DIR_LIGHT)] _UseDirLight ("Directional Light", Float) = 0

	_EdgeScale ("Edge Scale", Range (0.5, 4.0)) = 1
    _EdgePower ("Edge Power", Range (0.0, 50.0)) = 1
    _EdgeColor ("Edge Color", Color) = (0,0,0,1)
	}
	SubShader {
		Tags { "QUEUE"="Geometry -2" "RenderType"="Opaque" }

		LOD 200

		Pass {
			Stencil {
				Ref 5
				Comp always
				Pass replace
				}
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
	
			CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma shader_feature USE_LIGHTMAP
				#pragma shader_feature USE_DIST_FOG
				#pragma shader_feature USE_DIR_LIGHT

				#define USE_MAIN_TEX;
				#define USE_FOG

				#pragma vertex vert
				#pragma fragment frag
				

				uniform half3 _RimColor;
				uniform half _RimPower;
				uniform half3 _RightColor;
				uniform half3 _TopColor;
				uniform half3 _FrontColor;
				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				uniform half _UseLightMap;
				uniform half _FogAnimationHeight;
				uniform half _FogAnimationFreq;

				uniform half _LightmapPower;
				uniform half3 _LightTint;
				uniform half3 _LightmapColor;
				uniform half _ShadowPower;
				
				uniform half3 _FogColor;
				uniform half _FogYStartPos;
				uniform half _FogHeight;
				
				uniform half _FogStart;
				uniform half _FogEnd;
				uniform half _FogDensity;
				
				#include "Marvelous.cginc"
								
				CL_OUT_WPOS vert(CL_IN v) {
				#ifndef USE_DIST_FOG
					return customLightingSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower,_FogYStartPos, _FogAnimationHeight, _FogAnimationFreq);
				#else
					CL_OUT_WPOS o=customLightingSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower,_FogYStartPos, _FogAnimationHeight, _FogAnimationFreq);
					float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos)*_FogDensity; 
					o.color.w = saturate((_FogEnd - cameraVertDist) / (_FogEnd - _FogStart));	
					return o;		
				#endif
				}
				
				fixed4 frag(CL_OUT_WPOS v) : COLOR {
				#ifndef USE_DIST_FOG
					return customLightingSoftFogFrag(v, _FogColor, _FogHeight, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower, 1);
				#else
					fixed4 c = customLightingSoftFogFrag(v, _FogColor, _FogHeight, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower);
					return lerp(half4(_FogColor,1),c,v.color.w);
				#endif
				}
				
			ENDCG
		}
		    Pass {
      Cull Back
      ZTest LEqual
      ZWrite On
      Blend SrcAlpha OneMinusSrcAlpha
  //    BlendOp Add

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
      
      uniform fixed _EdgeScale;
      uniform fixed _EdgePower;
      uniform fixed3 _EdgeColor;
      
      // Vertex
      struct VS {
        half4 pos: POSITION;
        half3 normal: TEXCOORD1;
        half3 eye: TEXCOORD2;
      };

      // Vertex shader
      VS vert (appdata_base i) {

        half3 world_pos = mul (unity_ObjectToWorld, i.vertex).xyz;
        half3 world_normal = mul (unity_ObjectToWorld, half4(i.normal, 0)).xyz;
        half3 camera_dir = normalize (WorldSpaceViewDir (i.vertex));

        VS o;
        o.pos = UnityObjectToClipPos (i.vertex);
        o.normal = normalize (world_normal);
        o.eye = camera_dir;
        return o;
      }
      // Fragment shader
      fixed4 frag (VS i): COLOR {

        fixed s = _EdgeScale;
        fixed n =  dot (i.normal, i.eye); // 0 -> 1
        fixed r = n + ((s - 1) / s);
        r = clamp (r, 0, 1);
        r = pow (r, _EdgePower);

        fixed a = 1 - r;

        return fixed4 (_EdgeColor.rgb, a);
      }

ENDCG
    }
	}
	FallBack "Diffuse"
}
