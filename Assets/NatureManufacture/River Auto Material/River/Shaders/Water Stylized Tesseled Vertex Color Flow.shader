Shader "NatureManufacture Shaders/Water/Water Stylized Tesseled Vertex Color Flow"
{
	Properties
	{
		_GlobalTiling("Global Tiling", Range( 0.001 , 100)) = 1
		_UVVDirection1UDirection0("UV - V Direction (1) U Direction (0)", Int) = 0
		_BackfaceAlpha("Backface Alpha", Range( 0 , 1)) = 0.85
		_SlowWaterSpeed("Slow Water Speed", Vector) = (0.3,0.3,0,0)
		_SlowWaterMixSpeed("Slow Water Mix Speed", Vector) = (0.002,0.002,0,0)
		_SmallCascadeMainSpeed("Small Cascade Main Speed", Vector) = (0.6,0.6,0,0)
		_SmallCascadeMixSpeed("Small Cascade Mix Speed", Vector) = (0.04,0.08,0,0)
		_BigCascadeMainSpeed("Big Cascade Main Speed", Vector) = (1.4,1.4,0,0)
		_BigCascadeMixSpeed("Big Cascade Mix Speed", Vector) = (0.02,0.28,0,0)
		_CleanFalloffMultiply("Clean Falloff Multiply", Range( 0.1 , 4)) = 0.64
		_CleanFalloffPower("Clean Falloff Power", Range( 0.4 , 10)) = 1.68
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_ShalowFalloffMultiply("Shalow Falloff Multiply", Range( 0.1 , 4)) = 0.47
		_ShalowFalloffPower("Shalow Falloff Power", Range( 0 , 10)) = 3.49
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_WaveTranslucencyPower("Wave Translucency Power", Range( 0 , 10)) = 3.44
		_WaveTranslucencyHardness("Wave Translucency Hardness", Range( 0 , 10)) = 7.78
		_WaveTranslucencyMultiply("Wave Translucency Multiply", Range( 0 , 10)) = 1
		_WaveTranslucencyFallOffDistance("Wave Translucency FallOff Distance", Range( 0 , 100)) = 30
		_WaterSpecularClose("Water Specular Close", Range( 0 , 1)) = 0
		_WaterSpecularFar("Water Specular Far", Range( 0 , 1)) = 0
		_WaterSpecularThreshold("Water Specular Threshold", Range( 0 , 10)) = 1
		_WaterSmoothness("Water Smoothness", Float) = 0
		_Distortion("Distortion", Float) = 0.5
		_FarNormalPower("Far Normal Power", Range( 0 , 1)) = 0.5
		_FarNormalBlendStartDistance("Far Normal Blend Start Distance", Float) = 200
		_FarNormalBlendThreshold("Far Normal Blend Threshold", Range( 0 , 10)) = 10
		[NoScaleOffset]_MicroWaveNormal("Micro Wave Normal", 2D) = "bump" {}
		_MicroWaveTiling("Micro Wave Tiling", Vector) = (20,20,0,0)
		_MicroWaveNormalScale("Micro Wave Normal Scale", Range( 0 , 2)) = 0.25
		_MacroWaveNormalScale("Macro Wave Normal Scale", Range( 0 , 2)) = 0
		_SlowWaterTiling("Slow Water Tiling", Vector) = (3,3,0,0)
		[NoScaleOffset]_SlowWaterNormal("Slow Water Normal", 2D) = "bump" {}
		_SlowNormalScale("Slow Normal Scale", Float) = 0
		[NoScaleOffset]_SlowWaterTesselation("Slow Water Tesselation", 2D) = "black" {}
		_SlowWaterTessScale("Slow Water Tess Scale", Float) = 0
		_SmallCascadeAngle("Small Cascade Angle", Range( 0.001 , 90)) = 90
		_SmallCascadeTiling("Small Cascade Tiling", Vector) = (1,1,0,0)
		_SmallCascadeAngleFalloff("Small Cascade Angle Falloff", Range( 0 , 80)) = 5
		_SmallCascadeNormal("Small Cascade Normal", 2D) = "bump" {}
		_SmallCascadeNormalScale("Small Cascade Normal Scale", Float) = 0
		[NoScaleOffset]_SmallCascadeWaterTess("Small Cascade Water Tess", 2D) = "white" {}
		_SmallCascadeWaterTessScale("Small Cascade Water Tess Scale", Float) = 0
		[NoScaleOffset]_SmallCascade("Small Cascade", 2D) = "white" {}
		_SmallCascadeColor("Small Cascade Color", Vector) = (1,1,1,0)
		_SmallCascadeFoamFalloff("Small Cascade Foam Falloff", Range( 0 , 10)) = 0
		_SmallCascadeSmoothness("Small Cascade Smoothness", Float) = 0
		_SmallCascadeSpecular("Small Cascade Specular", Range( 0 , 1)) = 0
		_BigCascadeAngle("Big Cascade Angle", Range( 0.001 , 90)) = 90
		_BigCascadeTiling("Big Cascade Tiling", Vector) = (1,1,0,0)
		_BigCascadeAngleFalloff("Big Cascade Angle Falloff", Range( 0 , 80)) = 15
		[NoScaleOffset]_BigCascadeNormal("Big Cascade Normal", 2D) = "bump" {}
		_BigCascadeNormalScale("Big Cascade Normal Scale", Float) = 0
		[NoScaleOffset]_BigCascadeWaterTess("Big Cascade Water Tess", 2D) = "black" {}
		_BigCascadeWaterTessScale("Big Cascade Water Tess Scale", Float) = 0
		[NoScaleOffset]_BigCascade("Big Cascade", 2D) = "white" {}
		_BigCascadeColor("Big Cascade Color", Vector) = (1,1,1,0)
		_BigCascadeFoamFalloff("Big Cascade Foam Falloff", Range( 0 , 10)) = 0
		_BigCascadeTransparency("Big Cascade Transparency", Range( 0 , 1)) = 0
		_BigCascadeSmoothness("Big Cascade Smoothness", Float) = 0
		_BigCascadeSpecular("Big Cascade Specular", Range( 0 , 1)) = 0
		[NoScaleOffset]_Noise("Noise", 2D) = "white" {}
		_NoiseSpeed("Noise Speed", Vector) = (1,1,0,0)
		_NoiseTiling("Noise Tiling", Vector) = (4,4,0,0)
		_SmallCascadeNoisePower("Small Cascade Noise Power", Range( 0 , 10)) = 2.71
		_SmallCascadeNoiseMultiply("Small Cascade Noise Multiply", Range( 0 , 20)) = 2
		_BigCascadeNoisePower("Big Cascade Noise Power", Range( 0 , 10)) = 2.71
		_BigCascadeNoiseMultiply("Big Cascade Noise Multiply", Range( 0 , 20)) = 10
		[NoScaleOffset]_Foam("Foam", 2D) = "white" {}
		_FoamSpeed("Foam Speed", Vector) = (0.3,0.3,0,0)
		_FoamTiling("Foam Tiling", Vector) = (1,1,0,0)
		_FoamColor("Foam Color", Vector) = (1,1,1,0)
		_FoamDepth("Foam Depth", Range( 0 , 10)) = 0
		_FoamFalloff("Foam Falloff", Range( -100 , 0)) = -100
		_FoamWaveHardness("Foam Wave Hardness", Range( 0 , 10)) = 0.9
		_FoamWavePower("Foam Wave Power", Range( 0 , 10)) = 2
		_FoamWaveMultiply("Foam Wave Multiply", Range( 0 , 10)) = 7
		_FoamSpecular("Foam Specular", Range( 0 , 1)) = 0
		_FoamSmoothness("Foam Smoothness", Float) = 0
		_OutlinePower("Outline Power", Range( 0 , 1)) = 0
		_OutlineFalloffBorder("Outline Falloff Border", Range( 0 , 200)) = 100
		_AOPower("AO Power", Range( 0 , 1)) = 1
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 25
		_TessMaxDisp( "Max Displacement", Float ) = 11
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+999" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		BlendOp Add , Add
		GrabPass{ "_WaterGrab" }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf StandardSpecular keepalpha noshadow noinstancing vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv4_texcoord4;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
			float3 worldPos;
			float4 screenPos;
			half ASEVFace : VFACE;
		};

		uniform float _SlowWaterTessScale;
		uniform sampler2D _SlowWaterTesselation;
		uniform int _UVVDirection1UDirection0;
		uniform float2 _SlowWaterMixSpeed;
		uniform float2 _SlowWaterTiling;
		uniform float _GlobalTiling;
		uniform sampler2D _SlowWaterNormal;
		uniform float _SlowNormalScale;
		uniform float2 _SlowWaterSpeed;
		uniform sampler2D _SmallCascadeWaterTess;
		uniform float2 _SmallCascadeMixSpeed;
		uniform float2 _SmallCascadeTiling;
		uniform sampler2D _SmallCascadeNormal;
		uniform float _SmallCascadeNormalScale;
		uniform float2 _SmallCascadeMainSpeed;
		uniform float _SmallCascadeWaterTessScale;
		uniform half _SmallCascadeAngle;
		uniform float _SmallCascadeAngleFalloff;
		uniform half _BigCascadeAngle;
		uniform float _BigCascadeAngleFalloff;
		uniform float _BigCascadeWaterTessScale;
		uniform sampler2D _BigCascadeWaterTess;
		uniform float2 _BigCascadeMixSpeed;
		uniform float2 _BigCascadeTiling;
		uniform sampler2D _BigCascadeNormal;
		uniform float _BigCascadeNormalScale;
		uniform float2 _BigCascadeMainSpeed;
		uniform float _MicroWaveNormalScale;
		uniform sampler2D _MicroWaveNormal;
		uniform float2 _MicroWaveTiling;
		uniform float _MacroWaveNormalScale;
		uniform float _FarNormalPower;
		uniform float _FarNormalBlendStartDistance;
		uniform float _FarNormalBlendThreshold;
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
		uniform float _FoamWaveHardness;
		uniform float _FoamWavePower;
		uniform float _FoamWaveMultiply;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		uniform float _ShalowFalloffMultiply;
		uniform float _ShalowFalloffPower;
		uniform float _BigCascadeTransparency;
		uniform float _WaveTranslucencyHardness;
		uniform float _WaveTranslucencyPower;
		uniform float _WaveTranslucencyMultiply;
		uniform float _WaveTranslucencyFallOffDistance;
		uniform sampler2D _SmallCascade;
		uniform sampler2D _Noise;
		uniform float2 _NoiseSpeed;
		uniform float2 _NoiseTiling;
		uniform float _SmallCascadeNoisePower;
		uniform float _SmallCascadeNoiseMultiply;
		uniform float3 _SmallCascadeColor;
		uniform float _SmallCascadeFoamFalloff;
		uniform sampler2D _BigCascade;
		uniform float _BigCascadeNoisePower;
		uniform float _BigCascadeNoiseMultiply;
		uniform float3 _BigCascadeColor;
		uniform float _BigCascadeFoamFalloff;
		uniform float _OutlinePower;
		uniform float _OutlineFalloffBorder;
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
		uniform float _EdgeLength;
		uniform float _TessMaxDisp;
		uniform float _TessPhongStrength;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength , _TessMaxDisp );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			int Direction723 = _UVVDirection1UDirection0;
			float2 appendResult1317 = (float2(_SlowWaterMixSpeed.y , _SlowWaterMixSpeed.x));
			float2 uv_TexCoord1302 = v.texcoord.xy * _SlowWaterTiling;
			float Globaltiling1010 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1310_0 = ( uv_TexCoord1302 * Globaltiling1010 );
			float2 panner1320 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _SlowWaterMixSpeed :  appendResult1317 ) + temp_output_1310_0);
			float2 WaterSpeedValueMix516 = panner1320;
			float U1268 = v.texcoord3.xy.x;
			float2 break1296 = ( _SlowWaterSpeed * _SlowWaterTiling );
			float temp_output_1298_0 = ( U1268 * break1296.x );
			float V1270 = v.texcoord3.xy.y;
			float temp_output_1297_0 = ( break1296.y * V1270 );
			float2 appendResult1299 = (float2(temp_output_1298_0 , temp_output_1297_0));
			float2 appendResult1300 = (float2(temp_output_1297_0 , temp_output_1298_0));
			float2 temp_output_1306_0 = (( (float)Direction723 == 1.0 ) ? appendResult1299 :  appendResult1300 );
			float temp_output_1272_0 = ( _Time.y * 0.15 );
			float Refresh11288 = frac( ( temp_output_1272_0 + 1.0 ) );
			float2 WaterSpeedValueMainFlowUV1830 = ( ( temp_output_1306_0 * Refresh11288 ) + temp_output_1310_0 );
			float Refresh21289 = frac( ( temp_output_1272_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2831 = ( ( temp_output_1306_0 * Refresh21289 ) + temp_output_1310_0 );
			float clampResult1321 = clamp( abs( ( ( Refresh11288 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float SlowFlowHeightBase835 = clampResult1321;
			float3 lerpResult838 = lerp( UnpackScaleNormal( tex2Dlod( _SlowWaterNormal, float4( WaterSpeedValueMainFlowUV1830, 0, 1.0) ), _SlowNormalScale ) , UnpackScaleNormal( tex2Dlod( _SlowWaterNormal, float4( WaterSpeedValueMainFlowUV2831, 0, 1.0) ), _SlowNormalScale ) , SlowFlowHeightBase835);
			float temp_output_398_0 = ( ( _SlowWaterTessScale * 0.3 ) * tex2Dlod( _SlowWaterTesselation, float4( ( WaterSpeedValueMix516 + ( (lerpResult838).xy * float2( 0.05,0.05 ) ) ), 0, 1.0) ).a );
			float lerpResult840 = lerp( tex2Dlod( _SlowWaterTesselation, float4( WaterSpeedValueMainFlowUV1830, 0, 1.0) ).a , tex2Dlod( _SlowWaterTesselation, float4( WaterSpeedValueMainFlowUV2831, 0, 1.0) ).a , SlowFlowHeightBase835);
			float temp_output_415_0 = ( temp_output_398_0 + ( _SlowWaterTessScale * lerpResult840 ) );
			float2 appendResult1348 = (float2(_SmallCascadeMixSpeed.y , _SmallCascadeMixSpeed.x));
			float2 uv_TexCoord1333 = v.texcoord.xy * _SmallCascadeTiling;
			float2 temp_output_1338_0 = ( uv_TexCoord1333 * Globaltiling1010 );
			float2 panner1351 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _SmallCascadeMixSpeed :  appendResult1348 ) + temp_output_1338_0);
			float2 SmallCascadeSpeedValueMix433 = panner1351;
			float2 temp_output_1324_0 = ( _SmallCascadeMainSpeed * _SmallCascadeTiling );
			float2 break1325 = temp_output_1324_0;
			float temp_output_1329_0 = ( U1268 * break1325.x );
			float temp_output_1328_0 = ( break1325.y * V1270 );
			float2 appendResult1330 = (float2(temp_output_1329_0 , temp_output_1328_0));
			float2 appendResult1332 = (float2(temp_output_1328_0 , temp_output_1329_0));
			float2 temp_output_1334_0 = (( (float)Direction723 == 1.0 ) ? appendResult1330 :  appendResult1332 );
			float temp_output_1267_0 = ( _Time.y * 0.2 );
			float Refresh1v21282 = frac( ( temp_output_1267_0 + 1.0 ) );
			float2 SmallCascadeWaterSpeedValueMainFlowUV1860 = ( ( temp_output_1334_0 * Refresh1v21282 ) + temp_output_1338_0 );
			float Refresh2v21285 = frac( ( temp_output_1267_0 + 0.5 ) );
			float2 SmallCascadeWaterSpeedValueMainFlowUV2854 = ( ( temp_output_1334_0 * Refresh2v21285 ) + temp_output_1338_0 );
			float SmallCascadeSlowFlowHeightBase859 = abs( ( ( Refresh1v21282 + -0.5 ) * 2.0 ) );
			float3 lerpResult864 = lerp( UnpackScaleNormal( tex2Dlod( _SmallCascadeNormal, float4( SmallCascadeWaterSpeedValueMainFlowUV1860, 0, 1.0) ), _SmallCascadeNormalScale ) , UnpackScaleNormal( tex2Dlod( _SmallCascadeNormal, float4( SmallCascadeWaterSpeedValueMainFlowUV2854, 0, 1.0) ), _SmallCascadeNormalScale ) , SmallCascadeSlowFlowHeightBase859);
			float temp_output_410_0 = ( tex2Dlod( _SmallCascadeWaterTess, float4( ( SmallCascadeSpeedValueMix433 + ( (lerpResult864).xy * float2( 0.05,0.05 ) ) ), 0, 1.0) ).a * ( _SmallCascadeWaterTessScale * 0.4 ) );
			float lerpResult869 = lerp( tex2Dlod( _SmallCascadeWaterTess, float4( SmallCascadeWaterSpeedValueMainFlowUV1860, 0, 1.0) ).a , tex2Dlod( _SmallCascadeWaterTess, float4( SmallCascadeWaterSpeedValueMainFlowUV2854, 0, 0.0) ).a , SmallCascadeSlowFlowHeightBase859);
			float temp_output_414_0 = ( temp_output_410_0 + ( lerpResult869 * _SmallCascadeWaterTessScale ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float clampResult259 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_258_0 = ( _SmallCascadeAngle / 45.0 );
			float clampResult263 = clamp( ( clampResult259 - ( 1.0 - temp_output_258_0 ) ) , 0.0 , 2.0 );
			float clampResult584 = clamp( ( clampResult263 * ( 1.0 / temp_output_258_0 ) ) , 0.0 , 1.0 );
			float temp_output_267_0 = pow( ( 1.0 - clampResult584 ) , _SmallCascadeAngleFalloff );
			float clampResult507 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_504_0 = ( _BigCascadeAngle / 45.0 );
			float clampResult509 = clamp( ( clampResult507 - ( 1.0 - temp_output_504_0 ) ) , 0.0 , 2.0 );
			float clampResult583 = clamp( ( clampResult509 * ( 1.0 / temp_output_504_0 ) ) , 0.0 , 1.0 );
			float clampResult514 = clamp( pow( ( 1.0 - clampResult583 ) , _BigCascadeAngleFalloff ) , 0.0 , 1.0 );
			float clampResult285 = clamp( ( temp_output_267_0 - clampResult514 ) , 0.0 , 1.0 );
			float lerpResult407 = lerp( temp_output_415_0 , ( temp_output_414_0 * clampResult285 ) , clampResult285);
			float2 appendResult1366 = (float2(_BigCascadeMixSpeed.y , _BigCascadeMixSpeed.x));
			float2 uv_TexCoord1364 = v.texcoord.xy * _BigCascadeTiling;
			float2 temp_output_1375_0 = ( uv_TexCoord1364 * Globaltiling1010 );
			float2 panner1369 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _BigCascadeMixSpeed :  appendResult1366 ) + temp_output_1375_0);
			float2 BigCascadeSpeedValueMix608 = panner1369;
			float2 break1356 = ( _BigCascadeMainSpeed * _BigCascadeTiling );
			float temp_output_1359_0 = ( U1268 * break1356.x );
			float temp_output_1360_0 = ( break1356.y * V1270 );
			float2 appendResult1363 = (float2(temp_output_1359_0 , temp_output_1360_0));
			float2 appendResult1361 = (float2(temp_output_1360_0 , temp_output_1359_0));
			float2 temp_output_1373_0 = (( (float)Direction723 == 1.0 ) ? appendResult1363 :  appendResult1361 );
			float temp_output_1274_0 = ( _Time.y * 0.6 );
			float Refresh1v31287 = frac( ( temp_output_1274_0 + 1.0 ) );
			float2 BigCascadeWaterSpeedValueMainFlowUV1893 = ( ( temp_output_1373_0 * Refresh1v31287 ) + temp_output_1375_0 );
			float Refresh2v31290 = frac( ( temp_output_1274_0 + 0.5 ) );
			float2 BigCascadeWaterSpeedValueMainFlowUV2894 = ( ( temp_output_1373_0 * Refresh2v31290 ) + temp_output_1375_0 );
			float BigCascadeSlowFlowHeightBase895 = abs( ( ( Refresh1v31287 + -0.5 ) * 2.0 ) );
			float3 lerpResult899 = lerp( UnpackScaleNormal( tex2Dlod( _BigCascadeNormal, float4( BigCascadeWaterSpeedValueMainFlowUV1893, 0, 1.0) ), _BigCascadeNormalScale ) , UnpackScaleNormal( tex2Dlod( _BigCascadeNormal, float4( BigCascadeWaterSpeedValueMainFlowUV2894, 0, 1.0) ), _BigCascadeNormalScale ) , BigCascadeSlowFlowHeightBase895);
			float temp_output_564_0 = ( ( _BigCascadeWaterTessScale * 0.5 ) * tex2Dlod( _BigCascadeWaterTess, float4( ( BigCascadeSpeedValueMix608 + ( (lerpResult899).xy * float2( 0.05,0.05 ) ) ), 0, 1.0) ).a );
			float lerpResult874 = lerp( tex2Dlod( _BigCascadeWaterTess, float4( BigCascadeWaterSpeedValueMainFlowUV1893, 0, 1.0) ).a , tex2Dlod( _BigCascadeWaterTess, float4( BigCascadeWaterSpeedValueMainFlowUV2894, 0, 1.0) ).a , BigCascadeSlowFlowHeightBase895);
			float temp_output_565_0 = ( temp_output_564_0 + ( _BigCascadeWaterTessScale * lerpResult874 ) );
			float4 break770 = ( v.color / float4( 1,1,1,1 ) );
			float lerpResult754 = lerp( max( lerpResult407 , ( temp_output_565_0 * clampResult514 ) ) , temp_output_415_0 , break770.r);
			float lerpResult755 = lerp( lerpResult754 , temp_output_414_0 , break770.g);
			float lerpResult752 = lerp( lerpResult755 , temp_output_565_0 , break770.b);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( (lerpResult752*1.0 + ( lerpResult752 * -0.5 )) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			int Direction723 = _UVVDirection1UDirection0;
			float2 appendResult1317 = (float2(_SlowWaterMixSpeed.y , _SlowWaterMixSpeed.x));
			float2 uv_TexCoord1302 = i.uv_texcoord * _SlowWaterTiling;
			float Globaltiling1010 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1310_0 = ( uv_TexCoord1302 * Globaltiling1010 );
			float2 panner1320 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _SlowWaterMixSpeed :  appendResult1317 ) + temp_output_1310_0);
			float2 WaterSpeedValueMix516 = panner1320;
			float U1268 = i.uv4_texcoord4.x;
			float2 break1296 = ( _SlowWaterSpeed * _SlowWaterTiling );
			float temp_output_1298_0 = ( U1268 * break1296.x );
			float V1270 = i.uv4_texcoord4.y;
			float temp_output_1297_0 = ( break1296.y * V1270 );
			float2 appendResult1299 = (float2(temp_output_1298_0 , temp_output_1297_0));
			float2 appendResult1300 = (float2(temp_output_1297_0 , temp_output_1298_0));
			float2 temp_output_1306_0 = (( (float)Direction723 == 1.0 ) ? appendResult1299 :  appendResult1300 );
			float temp_output_1272_0 = ( _Time.y * 0.15 );
			float Refresh11288 = frac( ( temp_output_1272_0 + 1.0 ) );
			float2 WaterSpeedValueMainFlowUV1830 = ( ( temp_output_1306_0 * Refresh11288 ) + temp_output_1310_0 );
			float Refresh21289 = frac( ( temp_output_1272_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2831 = ( ( temp_output_1306_0 * Refresh21289 ) + temp_output_1310_0 );
			float clampResult1321 = clamp( abs( ( ( Refresh11288 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float SlowFlowHeightBase835 = clampResult1321;
			float3 lerpResult838 = lerp( UnpackScaleNormal( tex2D( _SlowWaterNormal, WaterSpeedValueMainFlowUV1830 ), _SlowNormalScale ) , UnpackScaleNormal( tex2D( _SlowWaterNormal, WaterSpeedValueMainFlowUV2831 ), _SlowNormalScale ) , SlowFlowHeightBase835);
			float2 temp_output_1086_0 = ( (lerpResult838).xy * float2( 0.05,0.05 ) );
			float3 temp_output_24_0 = BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _MicroWaveNormal, ( ( _MicroWaveTiling * WaterSpeedValueMix516 ) + temp_output_1086_0 ) ), _MicroWaveNormalScale ) , UnpackScaleNormal( tex2D( _SlowWaterNormal, ( WaterSpeedValueMix516 + temp_output_1086_0 ) ), _MacroWaveNormalScale ) ) , lerpResult838 );
			float2 appendResult1348 = (float2(_SmallCascadeMixSpeed.y , _SmallCascadeMixSpeed.x));
			float2 uv_TexCoord1333 = i.uv_texcoord * _SmallCascadeTiling;
			float2 temp_output_1338_0 = ( uv_TexCoord1333 * Globaltiling1010 );
			float2 panner1351 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _SmallCascadeMixSpeed :  appendResult1348 ) + temp_output_1338_0);
			float2 SmallCascadeSpeedValueMix433 = panner1351;
			float2 temp_output_1324_0 = ( _SmallCascadeMainSpeed * _SmallCascadeTiling );
			float2 break1325 = temp_output_1324_0;
			float temp_output_1329_0 = ( U1268 * break1325.x );
			float temp_output_1328_0 = ( break1325.y * V1270 );
			float2 appendResult1330 = (float2(temp_output_1329_0 , temp_output_1328_0));
			float2 appendResult1332 = (float2(temp_output_1328_0 , temp_output_1329_0));
			float2 temp_output_1334_0 = (( (float)Direction723 == 1.0 ) ? appendResult1330 :  appendResult1332 );
			float temp_output_1267_0 = ( _Time.y * 0.2 );
			float Refresh1v21282 = frac( ( temp_output_1267_0 + 1.0 ) );
			float2 SmallCascadeWaterSpeedValueMainFlowUV1860 = ( ( temp_output_1334_0 * Refresh1v21282 ) + temp_output_1338_0 );
			float Refresh2v21285 = frac( ( temp_output_1267_0 + 0.5 ) );
			float2 SmallCascadeWaterSpeedValueMainFlowUV2854 = ( ( temp_output_1334_0 * Refresh2v21285 ) + temp_output_1338_0 );
			float SmallCascadeSlowFlowHeightBase859 = abs( ( ( Refresh1v21282 + -0.5 ) * 2.0 ) );
			float3 lerpResult864 = lerp( UnpackScaleNormal( tex2D( _SmallCascadeNormal, SmallCascadeWaterSpeedValueMainFlowUV1860 ), _SmallCascadeNormalScale ) , UnpackScaleNormal( tex2D( _SmallCascadeNormal, SmallCascadeWaterSpeedValueMainFlowUV2854 ), _SmallCascadeNormalScale ) , SmallCascadeSlowFlowHeightBase859);
			float2 temp_output_1096_0 = ( (lerpResult864).xy * float2( 0.1,0.1 ) );
			float3 temp_output_526_0 = BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _MicroWaveNormal, ( ( _MicroWaveTiling * SmallCascadeSpeedValueMix433 ) + temp_output_1096_0 ) ), _MicroWaveNormalScale ) , UnpackScaleNormal( tex2D( _SmallCascadeNormal, ( SmallCascadeSpeedValueMix433 + temp_output_1096_0 ) ), _MacroWaveNormalScale ) ) , lerpResult864 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult259 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_258_0 = ( _SmallCascadeAngle / 45.0 );
			float clampResult263 = clamp( ( clampResult259 - ( 1.0 - temp_output_258_0 ) ) , 0.0 , 2.0 );
			float clampResult584 = clamp( ( clampResult263 * ( 1.0 / temp_output_258_0 ) ) , 0.0 , 1.0 );
			float temp_output_267_0 = pow( ( 1.0 - clampResult584 ) , _SmallCascadeAngleFalloff );
			float clampResult507 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_504_0 = ( _BigCascadeAngle / 45.0 );
			float clampResult509 = clamp( ( clampResult507 - ( 1.0 - temp_output_504_0 ) ) , 0.0 , 2.0 );
			float clampResult583 = clamp( ( clampResult509 * ( 1.0 / temp_output_504_0 ) ) , 0.0 , 1.0 );
			float clampResult514 = clamp( pow( ( 1.0 - clampResult583 ) , _BigCascadeAngleFalloff ) , 0.0 , 1.0 );
			float clampResult285 = clamp( ( temp_output_267_0 - clampResult514 ) , 0.0 , 1.0 );
			float3 lerpResult330 = lerp( temp_output_24_0 , temp_output_526_0 , clampResult285);
			float2 appendResult1366 = (float2(_BigCascadeMixSpeed.y , _BigCascadeMixSpeed.x));
			float2 uv_TexCoord1364 = i.uv_texcoord * _BigCascadeTiling;
			float2 temp_output_1375_0 = ( uv_TexCoord1364 * Globaltiling1010 );
			float2 panner1369 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _BigCascadeMixSpeed :  appendResult1366 ) + temp_output_1375_0);
			float2 BigCascadeSpeedValueMix608 = panner1369;
			float2 break1356 = ( _BigCascadeMainSpeed * _BigCascadeTiling );
			float temp_output_1359_0 = ( U1268 * break1356.x );
			float temp_output_1360_0 = ( break1356.y * V1270 );
			float2 appendResult1363 = (float2(temp_output_1359_0 , temp_output_1360_0));
			float2 appendResult1361 = (float2(temp_output_1360_0 , temp_output_1359_0));
			float2 temp_output_1373_0 = (( (float)Direction723 == 1.0 ) ? appendResult1363 :  appendResult1361 );
			float temp_output_1274_0 = ( _Time.y * 0.6 );
			float Refresh1v31287 = frac( ( temp_output_1274_0 + 1.0 ) );
			float2 BigCascadeWaterSpeedValueMainFlowUV1893 = ( ( temp_output_1373_0 * Refresh1v31287 ) + temp_output_1375_0 );
			float Refresh2v31290 = frac( ( temp_output_1274_0 + 0.5 ) );
			float2 BigCascadeWaterSpeedValueMainFlowUV2894 = ( ( temp_output_1373_0 * Refresh2v31290 ) + temp_output_1375_0 );
			float BigCascadeSlowFlowHeightBase895 = abs( ( ( Refresh1v31287 + -0.5 ) * 2.0 ) );
			float3 lerpResult899 = lerp( UnpackScaleNormal( tex2D( _BigCascadeNormal, BigCascadeWaterSpeedValueMainFlowUV1893 ), _BigCascadeNormalScale ) , UnpackScaleNormal( tex2D( _BigCascadeNormal, BigCascadeWaterSpeedValueMainFlowUV2894 ), _BigCascadeNormalScale ) , BigCascadeSlowFlowHeightBase895);
			float2 temp_output_1102_0 = ( (lerpResult899).xy * float2( 0.15,0.15 ) );
			float3 temp_output_333_0 = BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _MicroWaveNormal, ( ( BigCascadeSpeedValueMix608 * _MicroWaveTiling ) + temp_output_1102_0 ) ), _MicroWaveNormalScale ) , UnpackScaleNormal( tex2D( _BigCascadeNormal, ( BigCascadeSpeedValueMix608 + temp_output_1102_0 ) ), _MacroWaveNormalScale ) ) , lerpResult899 );
			float3 lerpResult529 = lerp( lerpResult330 , temp_output_333_0 , clampResult514);
			float4 break770 = ( i.vertexColor / float4( 1,1,1,1 ) );
			float3 lerpResult748 = lerp( lerpResult529 , temp_output_24_0 , break770.r);
			float3 lerpResult749 = lerp( lerpResult748 , temp_output_526_0 , break770.g);
			float3 lerpResult750 = lerp( lerpResult749 , temp_output_333_0 , break770.b);
			float3 appendResult1447 = (float3(_FarNormalPower , _FarNormalPower , 1.0));
			float3 ase_worldPos = i.worldPos;
			float Distance1174 = distance( ase_worldPos , _WorldSpaceCameraPos );
			float clampResult1449 = clamp( pow( ( Distance1174 / _FarNormalBlendStartDistance ) , _FarNormalBlendThreshold ) , 0.0 , 1.0 );
			float3 lerpResult1451 = lerp( lerpResult750 , ( lerpResult750 * appendResult1447 ) , clampResult1449);
			o.Normal = lerpResult1451;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult163 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float4 screenColor65 = tex2D( _WaterGrab, ( float3( ( appendResult163 / ase_grabScreenPosNorm.a ) ,  0.0 ) + ( lerpResult529 * _Distortion ) ).xy );
			float eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float temp_output_3_0 = ( eyeDepth1 - ase_screenPos.w );
			float temp_output_89_0 = abs( temp_output_3_0 );
			float temp_output_113_0 = saturate( pow( ( temp_output_89_0 + _FoamDepth ) , _FoamFalloff ) );
			float2 temp_output_1385_0 = ( _FoamSpeed * _FoamTiling );
			float2 break1388 = temp_output_1385_0;
			float temp_output_1390_0 = ( U1268 * break1388.x );
			float temp_output_1389_0 = ( break1388.y * V1270 );
			float2 appendResult1393 = (float2(temp_output_1390_0 , temp_output_1389_0));
			float2 appendResult1391 = (float2(temp_output_1389_0 , temp_output_1390_0));
			float2 temp_output_1396_0 = (( (float)Direction723 == 1.0 ) ? appendResult1393 :  appendResult1391 );
			float2 uv_TexCoord1395 = i.uv_texcoord * _FoamTiling;
			float2 temp_output_1400_0 = ( uv_TexCoord1395 * Globaltiling1010 );
			float2 FoamCascadeWaterSpeedValueMainFlowUV1932 = ( ( temp_output_1396_0 * Refresh11288 ) + temp_output_1400_0 );
			float2 temp_output_1123_0 = ( (lerpResult529).xy * float2( 0.03,0.03 ) );
			float2 FoamCascadeWaterSpeedValueMainFlowUV2933 = ( ( temp_output_1396_0 * Refresh21289 ) + temp_output_1400_0 );
			float clampResult1408 = clamp( abs( ( ( Refresh11288 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float FoamCascadeSlowFlowHeightBase935 = clampResult1408;
			float lerpResult937 = lerp( tex2D( _Foam, ( FoamCascadeWaterSpeedValueMainFlowUV1932 + temp_output_1123_0 ) ).a , tex2D( _Foam, ( FoamCascadeWaterSpeedValueMainFlowUV2933 + temp_output_1123_0 ) ).a , FoamCascadeSlowFlowHeightBase935);
			float temp_output_114_0 = ( temp_output_113_0 * lerpResult937 );
			float lerpResult1462 = lerp( temp_output_114_0 , 0.0 , clampResult514);
			float clampResult1454 = clamp( lerpResult1462 , 0.0 , 1.0 );
			float temp_output_398_0 = ( ( _SlowWaterTessScale * 0.3 ) * tex2D( _SlowWaterTesselation, ( WaterSpeedValueMix516 + ( (lerpResult838).xy * float2( 0.05,0.05 ) ) ) ).a );
			float lerpResult840 = lerp( tex2D( _SlowWaterTesselation, WaterSpeedValueMainFlowUV1830 ).a , tex2D( _SlowWaterTesselation, WaterSpeedValueMainFlowUV2831 ).a , SlowFlowHeightBase835);
			float temp_output_410_0 = ( tex2D( _SmallCascadeWaterTess, ( SmallCascadeSpeedValueMix433 + ( (lerpResult864).xy * float2( 0.05,0.05 ) ) ) ).a * ( _SmallCascadeWaterTessScale * 0.4 ) );
			float lerpResult869 = lerp( tex2D( _SmallCascadeWaterTess, SmallCascadeWaterSpeedValueMainFlowUV1860 ).a , tex2D( _SmallCascadeWaterTess, SmallCascadeWaterSpeedValueMainFlowUV2854 ).a , SmallCascadeSlowFlowHeightBase859);
			float lerpResult1163 = lerp( ( temp_output_398_0 + lerpResult840 ) , ( ( temp_output_410_0 + lerpResult869 ) * clampResult285 ) , clampResult285);
			float temp_output_564_0 = ( ( _BigCascadeWaterTessScale * 0.5 ) * tex2D( _BigCascadeWaterTess, ( BigCascadeSpeedValueMix608 + ( (lerpResult899).xy * float2( 0.05,0.05 ) ) ) ).a );
			float lerpResult874 = lerp( tex2D( _BigCascadeWaterTess, BigCascadeWaterSpeedValueMainFlowUV1893 ).a , tex2D( _BigCascadeWaterTess, BigCascadeWaterSpeedValueMainFlowUV2894 ).a , BigCascadeSlowFlowHeightBase895);
			float lerpResult1166 = lerp( lerpResult1163 , ( ( temp_output_564_0 + lerpResult874 ) * clampResult514 ) , clampResult514);
			float HeightMapMix1263 = lerpResult1166;
			float clampResult1440 = clamp( ( pow( abs( ( HeightMapMix1263 * _FoamWaveHardness ) ) , _FoamWavePower ) * _FoamWaveMultiply ) , 0.0 , 1.0 );
			float4 lerpResult117 = lerp( screenColor65 , float4( _FoamColor , 0.0 ) , ( clampResult1454 * clampResult1440 ));
			float clampResult1455 = clamp( temp_output_113_0 , 0.0 , 1.0 );
			float4 lerpResult390 = lerp( screenColor65 , lerpResult117 , clampResult1455);
			float lerpResult810 = lerp( pow( ( temp_output_89_0 * _ShalowFalloffMultiply ) , ( _ShalowFalloffPower * -1.0 ) ) , 100.0 , ( _BigCascadeTransparency * clampResult514 ));
			float clampResult1453 = clamp( saturate( lerpResult810 ) , 0.0 , 1.0 );
			float4 lerpResult13 = lerp( _DeepColor , _ShalowColor , clampResult1453);
			float clampResult1158 = clamp( ( Distance1174 / _WaveTranslucencyFallOffDistance ) , 0.0 , 1.0 );
			float lerpResult1159 = lerp( ( pow( abs( ( HeightMapMix1263 * _WaveTranslucencyHardness ) ) , _WaveTranslucencyPower ) * _WaveTranslucencyMultiply ) , 0.0 , clampResult1158);
			float clampResult1160 = clamp( lerpResult1159 , 0.0 , 1.0 );
			float Microwaves1161 = clampResult1160;
			float4 lerpResult1169 = lerp( lerpResult13 , _ShalowColor , Microwaves1161);
			float temp_output_458_0 = ( 1.0 - clampResult1453 );
			float4 lerpResult1007 = lerp( lerpResult390 , lerpResult1169 , temp_output_458_0);
			float lerpResult879 = lerp( tex2D( _SmallCascade, SmallCascadeWaterSpeedValueMainFlowUV1860 ).a , tex2D( _SmallCascade, SmallCascadeWaterSpeedValueMainFlowUV2854 ).a , SmallCascadeSlowFlowHeightBase859);
			float2 break1413 = ( _NoiseSpeed * _NoiseTiling );
			float temp_output_1416_0 = ( U1268 * break1413.x );
			float temp_output_1415_0 = ( break1413.y * V1270 );
			float2 appendResult1419 = (float2(temp_output_1416_0 , temp_output_1415_0));
			float2 appendResult1418 = (float2(temp_output_1415_0 , temp_output_1416_0));
			float2 temp_output_1422_0 = (( (float)Direction723 == 1.0 ) ? appendResult1419 :  appendResult1418 );
			float2 uv_TexCoord1423 = i.uv_texcoord * _NoiseTiling;
			float2 temp_output_1426_0 = ( uv_TexCoord1423 * Globaltiling1010 );
			float lerpResult1431 = lerp( tex2D( _Noise, ( ( temp_output_1422_0 * Refresh11288 ) + temp_output_1426_0 ) ).a , tex2D( _Noise, ( ( temp_output_1422_0 * Refresh21289 ) + temp_output_1426_0 ) ).a , SlowFlowHeightBase835);
			float clampResult488 = clamp( ( pow( lerpResult1431 , _SmallCascadeNoisePower ) * _SmallCascadeNoiseMultiply ) , 0.0 , 1.0 );
			float lerpResult480 = lerp( 0.0 , lerpResult879 , clampResult488);
			float3 temp_output_320_0 = ( lerpResult480 * _SmallCascadeColor );
			float clampResult322 = clamp( pow( lerpResult480 , _SmallCascadeFoamFalloff ) , 0.0 , 1.0 );
			float lerpResult580 = lerp( 0.0 , clampResult322 , clampResult285);
			float4 lerpResult324 = lerp( lerpResult1007 , float4( temp_output_320_0 , 0.0 ) , lerpResult580);
			float lerpResult902 = lerp( tex2D( _BigCascade, BigCascadeWaterSpeedValueMainFlowUV1893 ).a , tex2D( _BigCascade, BigCascadeWaterSpeedValueMainFlowUV2894 ).a , BigCascadeSlowFlowHeightBase895);
			float clampResult807 = clamp( ( pow( lerpResult1431 , _BigCascadeNoisePower ) * _BigCascadeNoiseMultiply ) , 0.0 , 1.0 );
			float lerpResult626 = lerp( ( lerpResult902 * 0.5 ) , lerpResult902 , clampResult807);
			float3 temp_output_241_0 = ( lerpResult626 * _BigCascadeColor );
			float clampResult299 = clamp( pow( lerpResult902 , _BigCascadeFoamFalloff ) , 0.0 , 1.0 );
			float lerpResult579 = lerp( 0.0 , clampResult299 , clampResult514);
			float4 lerpResult239 = lerp( lerpResult324 , float4( temp_output_241_0 , 0.0 ) , lerpResult579);
			float4 lerpResult773 = lerp( screenColor65 , lerpResult1169 , temp_output_458_0);
			float4 lerpResult757 = lerp( lerpResult239 , lerpResult773 , break770.r);
			float4 lerpResult762 = lerp( lerpResult773 , float4( temp_output_320_0 , 0.0 ) , clampResult322);
			float4 lerpResult758 = lerp( lerpResult757 , lerpResult762 , break770.g);
			float4 lerpResult763 = lerp( lerpResult773 , float4( temp_output_241_0 , 0.0 ) , clampResult299);
			float4 lerpResult756 = lerp( lerpResult758 , lerpResult763 , break770.b);
			o.Albedo = lerpResult756.rgb;
			float clampResult1058 = clamp( pow( ( temp_output_3_0 + ( 1.0 - _OutlinePower ) ) , ( 1.0 - _OutlineFalloffBorder ) ) , 0.0 , 1.0 );
			o.Emission = ( ( clampResult1058 * float4(1,1,1,0) ) * i.vertexColor.a ).rgb;
			float lerpResult994 = lerp( _WaterSpecularFar , _WaterSpecularClose , pow( clampResult1453 , _WaterSpecularThreshold ));
			float lerpResult130 = lerp( lerpResult994 , _FoamSpecular , temp_output_114_0);
			float lerpResult585 = lerp( lerpResult130 , _SmallCascadeSpecular , ( lerpResult580 * clampResult285 ));
			float lerpResult587 = lerp( lerpResult585 , _BigCascadeSpecular , ( lerpResult579 * clampResult514 ));
			float lerpResult785 = lerp( lerpResult587 , lerpResult130 , break770.r);
			float lerpResult796 = lerp( lerpResult130 , _SmallCascadeSpecular , lerpResult580);
			float lerpResult786 = lerp( lerpResult785 , lerpResult796 , break770.g);
			float lerpResult797 = lerp( lerpResult130 , _BigCascadeSpecular , lerpResult579);
			float lerpResult787 = lerp( lerpResult786 , lerpResult797 , break770.b);
			float3 temp_cast_22 = (lerpResult787).xxx;
			o.Specular = temp_cast_22;
			float lerpResult591 = lerp( _WaterSmoothness , _FoamSmoothness , temp_output_114_0);
			float lerpResult593 = lerp( lerpResult591 , _SmallCascadeSmoothness , ( lerpResult580 * clampResult285 ));
			float lerpResult592 = lerp( lerpResult593 , _BigCascadeSmoothness , ( lerpResult579 * clampResult514 ));
			float lerpResult788 = lerp( lerpResult592 , lerpResult591 , break770.r);
			float lerpResult798 = lerp( lerpResult591 , _SmallCascadeSmoothness , lerpResult580);
			float lerpResult789 = lerp( lerpResult788 , lerpResult798 , break770.g);
			float lerpResult799 = lerp( lerpResult591 , _BigCascadeSmoothness , lerpResult579);
			float lerpResult790 = lerp( lerpResult789 , lerpResult799 , break770.b);
			o.Smoothness = lerpResult790;
			o.Occlusion = _AOPower;
			float clampResult1065 = clamp( i.vertexColor.a , 0.0 , 1.0 );
			float clampResult1142 = clamp( ( temp_output_89_0 * _CleanFalloffMultiply ) , 0.0 , 1.0 );
			float clampResult1146 = clamp( pow( abs( clampResult1142 ) , _CleanFalloffPower ) , 0.0 , 1.0 );
			float temp_output_779_0 = ( clampResult1065 * clampResult1146 );
			float switchResult1070 = (((i.ASEVFace>0)?(temp_output_779_0):(( temp_output_779_0 * _BackfaceAlpha ))));
			o.Alpha = switchResult1070;
		}

		ENDCG
	}
}