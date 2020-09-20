 Shader "NatureManufacture Shaders/Water/Water River Mobile Flow"
{
	Properties
	{
		[Toggle(_USEGRABSCREENCOLOR_ON)] _UseGrabScreenColor("UseGrabScreenColor", Float) = 1
		_UVVDirection1UDirection0("UV - V Direction (1) U Direction (0)", Int) = 0
		_GlobalTiling("Global Tiling", Range( 0.001 , 100)) = 1
		_BackfaceAlpha("Backface Alpha", Range( 0 , 1)) = 0.85
		_SlowWaterSpeed("Slow Water Speed", Vector) = (0.3,0.3,0,0)
		_SmallCascadeMainSpeed("Small Cascade Main Speed", Vector) = (0.6,0.6,0,0)
		_BigCascadeMainSpeed("Big Cascade Main Speed", Vector) = (1.4,1.4,0,0)
		_CleanFalloffMultiply("Clean Falloff Multiply", Range( 0.1 , 4)) = 0.64
		_CleanFalloffPower("Clean Falloff Power", Range( 0.4 , 10)) = 1.68
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_ShalowFalloffMultiply("Shalow Falloff Multiply", Range( 0.1 , 4)) = 0.47
		_ShalowFalloffPower("Shalow Falloff Power", Range( 0 , 10)) = 3.49
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_WaterSpecularClose("Water Specular Close", Range( 0 , 1)) = 0
		_WaterSpecularFar("Water Specular Far", Range( 0 , 1)) = 0
		_WaterSpecularThreshold("Water Specular Threshold", Range( 0 , 10)) = 1
		_WaterSmoothness("Water Smoothness", Float) = 0
		_Distortion("Distortion", Float) = 0.5
		_SlowWaterTiling("Slow Water Tiling", Vector) = (3,3,0,0)
		[NoScaleOffset]_WaterNormal("Water Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0
		_SmallCascadeAngle("Small Cascade Angle", Range( 0.001 , 90)) = 90
		_SmallCascadeAngleFalloff("Small Cascade Angle Falloff", Range( 0 , 80)) = 5
		_SmallCascadeNormalScale("Small Cascade Normal Scale", Float) = 0
		[NoScaleOffset]_CascadesTexturesRGNoiseA("Cascades Textures (RG) Noise (A)", 2D) = "white" {}
		_SmallCascadeColor("Small Cascade Color", Vector) = (1,1,1,0)
		_SmallCascadeFoamFalloff("Small Cascade Foam Falloff", Range( 0 , 10)) = 0
		_SmallCascadeSmoothness("Small Cascade Smoothness", Float) = 0
		_SmallCascadeSpecular("Small Cascade Specular", Range( 0 , 1)) = 0
		_BigCascadeTiling("Big Cascade Tiling", Vector) = (1,1,0,0)
		_BigCascadeAngle("Big Cascade Angle", Range( 0.001 , 90)) = 90
		_BigCascadeAngleFalloff("Big Cascade Angle Falloff", Range( 0 , 80)) = 15
		_BigCascadeNormalScale("Big Cascade Normal Scale", Float) = 0
		_SmallCascadeTiling("Small Cascade Tiling", Vector) = (1,1,0,0)
		_BigCascadeColor("Big Cascade Color", Vector) = (1,1,1,0)
		_BigCascadeFoamFalloff("Big Cascade Foam Falloff", Range( 0 , 10)) = 0
		_BigCascadeTransparency("Big Cascade Transparency", Range( 0 , 1)) = 0
		_BigCascadeSmoothness("Big Cascade Smoothness", Float) = 0
		_BigCascadeSpecular("Big Cascade Specular", Range( 0 , 1)) = 0
		_NoiseTiling("Noise Tiling", Vector) = (4,4,0,0)
		_NoiseSpeed("Noise Speed", Vector) = (1,1,0,0)
		_SmallCascadeNoisePower("Small Cascade Noise Power", Range( 0 , 10)) = 0
		_SmallCascadeNoiseMultiply("Small Cascade Noise Multiply", Range( 0 , 40)) = 20
		_BigCascadeNoiseMultiply("Big Cascade Noise Multiply", Range( 0 , 40)) = 20
		_BigCascadeNoisePower("Big Cascade Noise Power", Range( 0 , 10)) = 2.71
		_FoamTiling("Foam Tiling", Vector) = (1,1,0,0)
		[NoScaleOffset]_Foam("Foam", 2D) = "white" {}
		_FoamSpeed("Foam Speed", Vector) = (0.3,0.3,0,0)
		_FoamColor("Foam Color", Vector) = (1,1,1,0)
		_FoamDepth("Foam Depth", Range( 0 , 10)) = 1
		_FoamFalloff("Foam Falloff", Range( -100 , 0)) = -10.9
		_FoamSpecular("Foam Specular", Range( 0 , 1)) = 0
		_FoamSmoothness("Foam Smoothness", Float) = 0
		_AOPower("AO Power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+999" }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		BlendOp Add , Add
		GrabPass{ "_WaterGrab" }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature _USEGRABSCREENCOLOR_ON
		#pragma surface surf StandardSpecular keepalpha noshadow 
		struct Input
		{
			float2 uv4_texcoord4;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
			float4 vertexColor : COLOR;
			half ASEVFace : VFACE;
		};

		uniform float _NormalScale;
		uniform sampler2D _WaterNormal;
		uniform int _UVVDirection1UDirection0;
		uniform float2 _SlowWaterSpeed;
		uniform float2 _SlowWaterTiling;
		uniform float _GlobalTiling;
		uniform float _SmallCascadeNormalScale;
		uniform float2 _SmallCascadeMainSpeed;
		uniform float2 _SmallCascadeTiling;
		uniform half _SmallCascadeAngle;
		uniform float _SmallCascadeAngleFalloff;
		uniform half _BigCascadeAngle;
		uniform float _BigCascadeAngleFalloff;
		uniform float _BigCascadeNormalScale;
		uniform float2 _BigCascadeMainSpeed;
		uniform float2 _BigCascadeTiling;
		uniform sampler2D _WaterGrab;
		uniform float _Distortion;
		uniform float3 _FoamColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FoamDepth;
		uniform float _FoamFalloff;
		uniform sampler2D _Foam;
		uniform float2 _FoamSpeed;
		uniform float2 _FoamTiling;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		uniform float _ShalowFalloffMultiply;
		uniform float _ShalowFalloffPower;
		uniform float _BigCascadeTransparency;
		uniform sampler2D _CascadesTexturesRGNoiseA;
		uniform float2 _NoiseSpeed;
		uniform float2 _NoiseTiling;
		uniform float _SmallCascadeNoisePower;
		uniform float _SmallCascadeNoiseMultiply;
		uniform float3 _SmallCascadeColor;
		uniform float _SmallCascadeFoamFalloff;
		uniform float _BigCascadeNoisePower;
		uniform float _BigCascadeNoiseMultiply;
		uniform float3 _BigCascadeColor;
		uniform float _BigCascadeFoamFalloff;
		uniform float _WaterSpecularFar;
		uniform float _WaterSpecularClose;
		uniform float _WaterSpecularThreshold;
		uniform float _FoamSpecular;
		uniform float _SmallCascadeSpecular;
		uniform float _BigCascadeSpecular;
		uniform float _WaterSmoothness;
		uniform float _FoamSmoothness;
		uniform float _SmallCascadeSmoothness;
		uniform float _BigCascadeSmoothness;
		uniform half _AOPower;
		uniform float _CleanFalloffMultiply;
		uniform float _CleanFalloffPower;
		uniform float _BackfaceAlpha;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			int Direction723 = _UVVDirection1UDirection0;
			float U976 = i.uv4_texcoord4.x;
			float2 break1007 = ( _SlowWaterSpeed * _SlowWaterTiling );
			float temp_output_1009_0 = ( U976 * break1007.x );
			float V977 = i.uv4_texcoord4.y;
			float temp_output_1008_0 = ( break1007.y * V977 );
			float2 appendResult1010 = (float2(temp_output_1009_0 , temp_output_1008_0));
			float2 appendResult1011 = (float2(temp_output_1008_0 , temp_output_1009_0));
			float2 temp_output_1017_0 = (( (float)Direction723 == 1.0 ) ? appendResult1010 :  appendResult1011 );
			float temp_output_998_0 = ( _Time.y * 0.15 );
			float Refresh1995 = frac( ( temp_output_998_0 + 1.0 ) );
			float2 uv_TexCoord1015 = i.uv_texcoord * _SlowWaterTiling;
			float Globaltiling920 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1020_0 = ( uv_TexCoord1015 * Globaltiling920 );
			float2 WaterSpeedValueMainFlowUV1777 = ( ( temp_output_1017_0 * Refresh1995 ) + temp_output_1020_0 );
			float Refresh2996 = frac( ( temp_output_998_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2776 = ( ( temp_output_1017_0 * Refresh2996 ) + temp_output_1020_0 );
			float clampResult1032 = clamp( abs( ( ( Refresh1995 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float SlowFlowHeightBase779 = clampResult1032;
			float3 lerpResult820 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV1777 ), _NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV2776 ), _NormalScale ) , SlowFlowHeightBase779);
			float2 temp_output_1035_0 = ( _SmallCascadeMainSpeed * _SmallCascadeTiling );
			float2 break1036 = temp_output_1035_0;
			float temp_output_1040_0 = ( U976 * break1036.x );
			float temp_output_1039_0 = ( break1036.y * V977 );
			float2 appendResult1041 = (float2(temp_output_1040_0 , temp_output_1039_0));
			float2 appendResult1043 = (float2(temp_output_1039_0 , temp_output_1040_0));
			float2 temp_output_1046_0 = (( (float)Direction723 == 1.0 ) ? appendResult1041 :  appendResult1043 );
			float temp_output_999_0 = ( _Time.y * 0.2 );
			float Refresh1v2989 = frac( ( temp_output_999_0 + 1.0 ) );
			float2 uv_TexCoord1045 = i.uv_texcoord * _SmallCascadeTiling;
			float2 temp_output_1049_0 = ( uv_TexCoord1045 * Globaltiling920 );
			float2 SmallCascadeWaterSpeedValueMainFlowUV1782 = ( ( temp_output_1046_0 * Refresh1v2989 ) + temp_output_1049_0 );
			float Refresh2v2991 = frac( ( temp_output_999_0 + 0.5 ) );
			float2 SmallCascadeWaterSpeedValueMainFlowUV2781 = ( ( temp_output_1046_0 * Refresh2v2991 ) + temp_output_1049_0 );
			float SmallCascadeSlowFlowHeightBase780 = abs( ( ( Refresh1v2989 + -0.5 ) * 2.0 ) );
			float3 lerpResult825 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeWaterSpeedValueMainFlowUV1782 ), _SmallCascadeNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeWaterSpeedValueMainFlowUV2781 ), _SmallCascadeNormalScale ) , SmallCascadeSlowFlowHeightBase780);
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult259 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_258_0 = ( _SmallCascadeAngle / 45.0 );
			float clampResult263 = clamp( ( clampResult259 - ( 1.0 - temp_output_258_0 ) ) , 0.0 , 2.0 );
			float clampResult584 = clamp( ( clampResult263 * ( 1.0 / temp_output_258_0 ) ) , 0.0 , 1.0 );
			float clampResult507 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_504_0 = ( _BigCascadeAngle / 45.0 );
			float clampResult509 = clamp( ( clampResult507 - ( 1.0 - temp_output_504_0 ) ) , 0.0 , 2.0 );
			float clampResult583 = clamp( ( clampResult509 * ( 1.0 / temp_output_504_0 ) ) , 0.0 , 1.0 );
			float clampResult514 = clamp( pow( ( 1.0 - clampResult583 ) , _BigCascadeAngleFalloff ) , 0.0 , 1.0 );
			float clampResult285 = clamp( ( pow( ( 1.0 - clampResult584 ) , _SmallCascadeAngleFalloff ) - clampResult514 ) , 0.0 , 1.0 );
			float3 lerpResult330 = lerp( lerpResult820 , lerpResult825 , clampResult285);
			float2 break1067 = ( _BigCascadeMainSpeed * _BigCascadeTiling );
			float temp_output_1070_0 = ( U976 * break1067.x );
			float temp_output_1071_0 = ( break1067.y * V977 );
			float2 appendResult1074 = (float2(temp_output_1070_0 , temp_output_1071_0));
			float2 appendResult1072 = (float2(temp_output_1071_0 , temp_output_1070_0));
			float2 temp_output_1075_0 = (( (float)Direction723 == 1.0 ) ? appendResult1074 :  appendResult1072 );
			float temp_output_981_0 = ( _Time.y * 0.6 );
			float Refresh1v3994 = frac( ( temp_output_981_0 + 1.0 ) );
			float2 uv_TexCoord1079 = i.uv_texcoord * _BigCascadeTiling;
			float2 temp_output_1081_0 = ( uv_TexCoord1079 * Globaltiling920 );
			float2 BigCascadeWaterSpeedValueMainFlowUV1797 = ( ( temp_output_1075_0 * Refresh1v3994 ) + temp_output_1081_0 );
			float Refresh2v31001 = frac( ( temp_output_981_0 + 0.5 ) );
			float2 BigCascadeWaterSpeedValueMainFlowUV2796 = ( ( temp_output_1075_0 * Refresh2v31001 ) + temp_output_1081_0 );
			float BigCascadeSlowFlowHeightBase795 = abs( ( ( Refresh1v3994 + -0.5 ) * 2.0 ) );
			float3 lerpResult830 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, BigCascadeWaterSpeedValueMainFlowUV1797 ), _BigCascadeNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, BigCascadeWaterSpeedValueMainFlowUV2796 ), _BigCascadeNormalScale ) , BigCascadeSlowFlowHeightBase795);
			float3 lerpResult529 = lerp( lerpResult330 , lerpResult830 , clampResult514);
			o.Normal = lerpResult529;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult163 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float4 screenColor65 = tex2D( _WaterGrab, ( float3( ( appendResult163 / ase_grabScreenPosNorm.a ) ,  0.0 ) + ( lerpResult529 * _Distortion ) ).xy );
			float eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float temp_output_89_0 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float temp_output_113_0 = saturate( pow( ( temp_output_89_0 + _FoamDepth ) , _FoamFalloff ) );
			float2 temp_output_1096_0 = ( _FoamSpeed * _FoamTiling );
			float2 break1099 = temp_output_1096_0;
			float temp_output_1100_0 = ( U976 * break1099.x );
			float temp_output_1103_0 = ( break1099.y * V977 );
			float2 appendResult1104 = (float2(temp_output_1100_0 , temp_output_1103_0));
			float2 appendResult1105 = (float2(temp_output_1103_0 , temp_output_1100_0));
			float2 temp_output_1108_0 = (( (float)Direction723 == 1.0 ) ? appendResult1104 :  appendResult1105 );
			float2 uv_TexCoord1111 = i.uv_texcoord * _FoamTiling;
			float2 temp_output_1120_0 = ( uv_TexCoord1111 * Globaltiling920 );
			float2 FoamCascadeWaterSpeedValueMainFlowUV1871 = ( ( temp_output_1108_0 * Refresh1995 ) + temp_output_1120_0 );
			float2 temp_output_958_0 = ( (lerpResult529).xy * float2( 0.03,0.03 ) );
			float2 FoamCascadeWaterSpeedValueMainFlowUV2882 = ( ( temp_output_1108_0 * Refresh2996 ) + temp_output_1120_0 );
			float clampResult1140 = clamp( abs( ( ( Refresh1995 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float FoamCascadeSlowFlowHeightBase883 = clampResult1140;
			float lerpResult850 = lerp( tex2D( _Foam, ( FoamCascadeWaterSpeedValueMainFlowUV1871 + temp_output_958_0 ) ).a , tex2D( _Foam, ( FoamCascadeWaterSpeedValueMainFlowUV2882 + temp_output_958_0 ) ).a , FoamCascadeSlowFlowHeightBase883);
			float temp_output_114_0 = ( temp_output_113_0 * lerpResult850 );
			float lerpResult1150 = lerp( temp_output_114_0 , 0.0 , clampResult514);
			float clampResult1143 = clamp( lerpResult1150 , 0.0 , 1.0 );
			float4 lerpResult117 = lerp( screenColor65 , float4( _FoamColor , 0.0 ) , clampResult1143);
			float4 lerpResult93 = lerp( screenColor65 , lerpResult117 , temp_output_113_0);
			#ifdef _USEGRABSCREENCOLOR_ON
				float4 staticSwitch975 = lerpResult93;
			#else
				float4 staticSwitch975 = lerpResult117;
			#endif
			float lerpResult761 = lerp( pow( ( temp_output_89_0 * _ShalowFalloffMultiply ) , ( _ShalowFalloffPower * -1.0 ) ) , 100.0 , ( _BigCascadeTransparency * clampResult514 ));
			float temp_output_94_0 = saturate( lerpResult761 );
			float4 lerpResult13 = lerp( _DeepColor , _ShalowColor , temp_output_94_0);
			float4 lerpResult390 = lerp( staticSwitch975 , lerpResult13 , ( 1.0 - temp_output_94_0 ));
			float lerpResult835 = lerp( tex2D( _CascadesTexturesRGNoiseA, SmallCascadeWaterSpeedValueMainFlowUV1782 ).r , tex2D( _CascadesTexturesRGNoiseA, SmallCascadeWaterSpeedValueMainFlowUV2781 ).r , SmallCascadeSlowFlowHeightBase780);
			float2 break1110 = ( _NoiseSpeed * _NoiseTiling );
			float temp_output_1116_0 = ( U976 * break1110.x );
			float temp_output_1117_0 = ( break1110.y * V977 );
			float2 appendResult1121 = (float2(temp_output_1116_0 , temp_output_1117_0));
			float2 appendResult1122 = (float2(temp_output_1117_0 , temp_output_1116_0));
			float2 temp_output_1131_0 = (( (float)Direction723 == 1.0 ) ? appendResult1121 :  appendResult1122 );
			float2 uv_TexCoord1128 = i.uv_texcoord * _NoiseTiling;
			float2 temp_output_1135_0 = ( uv_TexCoord1128 * Globaltiling920 );
			float lerpResult1142 = lerp( tex2D( _CascadesTexturesRGNoiseA, ( ( temp_output_1131_0 * Refresh1995 ) + temp_output_1135_0 ) ).a , tex2D( _CascadesTexturesRGNoiseA, ( ( temp_output_1131_0 * Refresh2996 ) + temp_output_1135_0 ) ).a , SlowFlowHeightBase779);
			float clampResult488 = clamp( ( pow( lerpResult1142 , _SmallCascadeNoisePower ) * _SmallCascadeNoiseMultiply ) , 0.0 , 1.0 );
			float lerpResult480 = lerp( 0.0 , lerpResult835 , clampResult488);
			float clampResult322 = clamp( pow( lerpResult835 , _SmallCascadeFoamFalloff ) , 0.0 , 1.0 );
			float lerpResult580 = lerp( 0.0 , clampResult322 , clampResult285);
			float4 lerpResult324 = lerp( lerpResult390 , float4( ( lerpResult480 * _SmallCascadeColor ) , 0.0 ) , lerpResult580);
			float lerpResult840 = lerp( tex2D( _CascadesTexturesRGNoiseA, BigCascadeWaterSpeedValueMainFlowUV1797 ).g , tex2D( _CascadesTexturesRGNoiseA, BigCascadeWaterSpeedValueMainFlowUV2796 ).g , BigCascadeSlowFlowHeightBase795);
			float clampResult758 = clamp( ( pow( lerpResult1142 , _BigCascadeNoisePower ) * _BigCascadeNoiseMultiply ) , 0.0 , 1.0 );
			float lerpResult626 = lerp( ( lerpResult840 * 0.5 ) , lerpResult840 , clampResult758);
			float clampResult299 = clamp( pow( lerpResult840 , _BigCascadeFoamFalloff ) , 0.0 , 1.0 );
			float lerpResult579 = lerp( 0.0 , clampResult299 , clampResult514);
			float4 lerpResult239 = lerp( lerpResult324 , float4( ( lerpResult626 * _BigCascadeColor ) , 0.0 ) , lerpResult579);
			o.Albedo = lerpResult239.rgb;
			float lerpResult910 = lerp( _WaterSpecularFar , _WaterSpecularClose , pow( temp_output_94_0 , _WaterSpecularThreshold ));
			float lerpResult130 = lerp( lerpResult910 , _FoamSpecular , temp_output_114_0);
			float lerpResult585 = lerp( lerpResult130 , _SmallCascadeSpecular , ( lerpResult580 * clampResult285 ));
			float lerpResult587 = lerp( lerpResult585 , _BigCascadeSpecular , ( lerpResult579 * clampResult514 ));
			float3 temp_cast_16 = (lerpResult587).xxx;
			o.Specular = temp_cast_16;
			float lerpResult591 = lerp( _WaterSmoothness , _FoamSmoothness , temp_output_114_0);
			float lerpResult593 = lerp( lerpResult591 , _SmallCascadeSmoothness , ( lerpResult580 * clampResult285 ));
			float lerpResult592 = lerp( lerpResult593 , _BigCascadeSmoothness , ( lerpResult579 * clampResult514 ));
			o.Smoothness = lerpResult592;
			o.Occlusion = _AOPower;
			float clampResult947 = clamp( ( temp_output_89_0 * _CleanFalloffMultiply ) , 0.0 , 1.0 );
			float clampResult951 = clamp( pow( abs( clampResult947 ) , _CleanFalloffPower ) , 0.0 , 1.0 );
			float temp_output_914_0 = ( clampResult951 * i.vertexColor.a );
			float switchResult931 = (((i.ASEVFace>0)?(temp_output_914_0):(( _BackfaceAlpha * temp_output_914_0 ))));
			o.Alpha = switchResult931;
		}

		ENDCG
	}
}