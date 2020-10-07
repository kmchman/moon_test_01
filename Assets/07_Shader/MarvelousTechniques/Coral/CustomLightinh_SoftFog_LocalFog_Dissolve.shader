Shader "0_Coral/Art/CustomLightinh_SoftFog_LocalFog_Dissolve"
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
		[Toggle(USE_TEXCOORD1)] _UseDissolve("Dissolve Enabled", Float) = 0
		[HDR]_Line_Color("Line_Color", Color) = (2.462289,1.400125,0.2993371,0)
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_Dissolve("Dissolve", Range( 0 , 1.2)) = 0.5
		_Line_Size("Line_Size", Float) = 2
		_Divide("Divide", Float) = 2
		_Opacity("Opacity", Float) = 0


    }
    SubShader
    {
		Tags { "QUEUE" = "Geometry" "RenderType" = "Opaque" }
        //LOD 200

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

			// dissolve
			sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
			float _Dissolve;
			float _Line_Size;
			float _Divide;
			float _Opacity;
			float4 _Line_Color;

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
#endif

#if USE_TEXCOORD1
				fixed4 col = tex2D(_DissolveTex, v.uv1);
				float temp_output_29_0 = step( col.r , ( _Dissolve / _Line_Size ) );
				float temp_output_31_0 = ( ( step( col.r , _Dissolve ) - temp_output_29_0 ) / _Divide );
				float3 emi = ( temp_output_29_0 + ( _Line_Color * temp_output_31_0 ) ).rgb;
				float output = ( temp_output_29_0 + temp_output_31_0 );

				float3 co = ((temp_output_29_0 + temp_output_31_0) * _Line_Color).rgb;

				customcolor = lerp(customcolor, float4(emi, output * _Opacity), output * _Opacity);

#endif
				return customcolor;
            }
            ENDCG
        }

		//pass
		//{
		//	//Blend SrcAlpha OneMinusSrcAlpha
		//	Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		//	Cull Off
  //      	//Lighting Off
  //      	//ZWrite Off
  //      	//Fog { Mode Off }

  //          CGPROGRAM
  //          #pragma vertex vert
  //          #pragma fragment frag
  //          // make fog work
  //          #pragma multi_compile_fog

  //          #include "UnityCG.cginc"

  //          struct appdata
  //          {
  //              float4 vertex : POSITION;
  //              float2 uv : TEXCOORD1;
  //          };

  //          struct v2f
  //          {
  //              float2 uv : TEXCOORD0;
  //              UNITY_FOG_COORDS(1)
  //              float4 vertex : SV_POSITION;
  //          };

  //          sampler2D _DissolveTex;
  //          float4 _DissolveTex_ST;
		//	float _Dissolve;
		//	float _Line_Size;
		//	float _Divide;
		//	float _Opacity;
		//	float4 _Line_Color;

  //          v2f vert (appdata v)
  //          {
  //              v2f o;
  //              o.vertex = UnityObjectToClipPos(v.vertex);
  //              o.uv = TRANSFORM_TEX(v.uv, _DissolveTex);
  //              UNITY_TRANSFER_FOG(o,o.vertex);
  //              return o;
  //          }

  //          fixed4 frag (v2f i) : SV_Target
  //          {
  //              // sample the texture
  //              fixed4 col = tex2D(_DissolveTex, i.uv);
		//		float temp_output_29_0 = step( col.r , ( _Dissolve / _Line_Size ) );
		//		float temp_output_31_0 = ( ( step( col.r , _Dissolve ) - temp_output_29_0 ) / _Divide );

		//		float3 emi = ( temp_output_29_0 + ( _Line_Color * temp_output_31_0 ) ).rgb;

		//		float output = ( temp_output_29_0 + temp_output_31_0 );

		//		return float4(emi.rgb, output * _Opacity);

  //              // apply fog
  //              //UNITY_APPLY_FOG(i.fogCoord, col);

  //              //return col;
  //          }
  //          ENDCG
		//}
    }
	FallBack "Diffuse"
}
