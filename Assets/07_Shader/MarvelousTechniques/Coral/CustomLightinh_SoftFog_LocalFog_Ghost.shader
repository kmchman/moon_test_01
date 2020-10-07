Shader "0_Coral/Art/CustomLightinh_SoftFog_LocalFog_Ghost"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0,1)) = 0

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


		_EdgeScale ("Edge Scale", Range (0.5, 4.0)) = 1
		_EdgePower ("Edge Power", Range (0.0, 50.0)) = 1
		_EdgeColor ("Edge Color", Color) = (0,0,0,1)
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
			#pragma shader_feature USE_LOCAL_HEIGHT_FOG
			#pragma shader_feature USE_HEIGHT_FOG
			#pragma shader_feature USE_DIST_FOG
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			uniform half _Alpha;

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

				fixed4 o = fixed4(1,1,1,1);
#if USE_HEIGHT_FOG || USE_DIST_FOG || USE_LOCAL_HEIGHT_FOG
				o = customLightingSoftFogFrag(v, 
				_FogColor, _FogHeight, _FogYStartPos, _FogAnimationHeight, _FogAnimationFreq,
				_LightTint, 
				_LocalFogColor, _LocalFogStartY, _LocalFogHeight,
				_FogDistanceColor,
				_ColorBoost);
#else
				o = customLightingFrag(v, _LightTint, 
				_ColorBoost);
#endif

				o.a *=  _Alpha;
				return o;
            }
            ENDCG
        }

		Pass 
		{
			Cull Back
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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
	//FallBack "Diffuse"
}
