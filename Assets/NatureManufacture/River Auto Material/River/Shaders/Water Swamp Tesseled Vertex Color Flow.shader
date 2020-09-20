Shader "NatureManufacture Shaders/Water/Water Swamp Tesseled Vertex Color Flow"
{
	Properties
	{
		_GlobalTiling("Global Tiling", Range( 0.001 , 100)) = 1
		_UVVDirection1UDirection0("UV - V Direction (1) U Direction (0)", Int) = 0
		_BackfaceAlpha("Backface Alpha", Range( 0 , 1)) = 0.85
		_WaterAOPower("Water AO Power", Range( 0 , 1)) = 1
		_CleanFalloffMultiply("Clean Falloff Multiply", Range( 0.1 , 4)) = 0.64
		_CleanFalloffPower("Clean Falloff Power", Range( 0.4 , 10)) = 1.68
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_ShalowFalloffMultiply("Shalow Falloff Multiply", Range( 0.1 , 4)) = 0.47
		_ShalowFalloffPower("Shalow Falloff Power", Range( 0 , 10)) = 3.49
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_WaveTranslucencyPower("Wave Translucency Power", Range( 0 , 10)) = 0.97
		_WaveTranslucencyHardness("Wave Translucency Hardness", Range( 0 , 10)) = 2.5
		_WaveTranslucencyMultiply("Wave Translucency Multiply", Range( 0 , 10)) = 0.34
		_WaveTranslucencyFallOffDistance("Wave Translucency FallOff Distance", Range( 0 , 100)) = 30
		_WaterSpecularClose("Water Specular Close", Range( 0 , 1)) = 0
		_WaterSpecularFar("Water Specular Far", Range( 0 , 1)) = 0
		_WaterSpecularThreshold("Water Specular Threshold", Range( 0 , 10)) = 1
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0
		_Distortion("Distortion", Float) = 0.5
		[NoScaleOffset]_MicroWaveNormal("Micro Wave Normal", 2D) = "bump" {}
		_MicroWaveTiling("Micro Wave Tiling", Vector) = (20,20,0,0)
		_MicroWaveNormalScale("Micro Wave Normal Scale", Range( 0 , 2)) = 0.25
		_MacroWaveNormalScale("Macro Wave Normal Scale", Range( 0 , 2)) = 0.33
		_WaterTiling("Water Tiling", Vector) = (3,3,0,0)
		_WaterMainSpeed("Water Main Speed", Vector) = (0.2,0.2,0,0)
		_CascadeTiling("Cascade Tiling", Vector) = (2,3,0,0)
		_CascadeMainSpeed("Cascade Main Speed", Vector) = (2,2,0,0)
		_WaterMixSpeed("Water Mix Speed", Vector) = (0.01,0.05,0,0)
		[NoScaleOffset]_WaterNormal("Water Normal", 2D) = "bump" {}
		_WaterNormalScale("Water Normal Scale", Float) = 0.3
		_CascadeNormalScale("Cascade Normal Scale", Float) = 0.7
		_FarNormalPower("Far Normal Power", Range( 0 , 1)) = 0.5
		_FarNormalBlendStartDistance("Far Normal Blend Start Distance", Float) = 200
		_FarNormalBlendThreshold("Far Normal Blend Threshold", Range( 0 , 10)) = 10
		[NoScaleOffset]_WaterTesselation("Water Tesselation", 2D) = "black" {}
		_WaterTessScale("Water Tess Scale", Float) = 0
		_CascadeAngle("Cascade Angle", Range( 0.001 , 90)) = 90
		_CascadeAngleFalloff("Cascade Angle Falloff", Range( 0 , 80)) = 5
		_CascadeTransparency("Cascade Transparency", Range( 0 , 1)) = 0
		[NoScaleOffset]_Noise("Noise", 2D) = "white" {}
		_NoiseTiling1("Noise Tiling 1", Vector) = (4,4,0,0)
		_NoiseTiling2("Noise Tiling 2", Vector) = (4,4,0,0)
		_DetailNoisePower("Detail Noise Power", Range( 0 , 10)) = 2.71
		[NoScaleOffset]_DetailAlbedo("Detail Albedo", 2D) = "black" {}
		_DetailAlbedoColor("Detail Albedo Color", Color) = (1,1,1,0)
		_Detail1Tiling("Detail 1 Tiling", Vector) = (3,3,0,0)
		_Detail1MainSpeed("Detail 1 Main Speed", Vector) = (0.2,0.2,0,0)
		_DetailSpecular("Detail Specular", Range( 0 , 1)) = 0
		[NoScaleOffset]_DetailNormal("Detail Normal", 2D) = "bump" {}
		_DetailNormalScale("Detail Normal Scale", Float) = 0
		_DetailSmoothness("Detail Smoothness", Range( 0 , 1)) = 0
		_DetailAOPower("Detail AO Power", Range( 0 , 1)) = 1
		_Detail2NoisePower("Detail 2 Noise Power", Range( 0 , 10)) = 10
		[NoScaleOffset]_Detail2Albedo("Detail 2 Albedo", 2D) = "black" {}
		_Detail2AlbedoColor("Detail 2 Albedo Color", Color) = (1,1,1,0)
		_Detail2Tiling("Detail 2 Tiling", Vector) = (3,3,0,0)
		_Detail2MainSpeed("Detail 2 Main Speed", Vector) = (0.2,0.2,0,0)
		_Detail2Specular("Detail 2 Specular", Range( 0 , 1)) = 0
		[NoScaleOffset]_Detail2Normal("Detail 2 Normal", 2D) = "bump" {}
		_Detail2NormalScale("Detail 2 Normal Scale", Float) = 0
		_Detail2Smoothness("Detail 2 Smoothness", Range( 0 , 1)) = 0
		_Detail2AOPower("Detail 2 AO Power", Range( 0 , 1)) = 1
		[NoScaleOffset]_Detail1GSmDetail2ASm("Detail 1 (G)Sm Detail 2 (A)Sm", 2D) = "white" {}
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 25
		_TessMaxDisp( "Max Displacement", Float ) = 11
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
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

		uniform float _WaterTessScale;
		uniform sampler2D _WaterTesselation;
		uniform int _UVVDirection1UDirection0;
		uniform float2 _WaterMixSpeed;
		uniform float2 _WaterTiling;
		uniform float _GlobalTiling;
		uniform sampler2D _WaterNormal;
		uniform float _WaterNormalScale;
		uniform float2 _WaterMainSpeed;
		uniform float _MicroWaveNormalScale;
		uniform sampler2D _MicroWaveNormal;
		uniform float2 _MicroWaveTiling;
		uniform float _MacroWaveNormalScale;
		uniform sampler2D _DetailNormal;
		uniform float _DetailNormalScale;
		uniform float2 _Detail1MainSpeed;
		uniform float2 _CascadeMainSpeed;
		uniform half _CascadeAngle;
		uniform float _CascadeAngleFalloff;
		uniform float2 _Detail1Tiling;
		uniform sampler2D _DetailAlbedo;
		uniform sampler2D _Noise;
		uniform float2 _NoiseTiling1;
		uniform float _DetailNoisePower;
		uniform sampler2D _Detail2Normal;
		uniform float _Detail2NormalScale;
		uniform float2 _Detail2MainSpeed;
		uniform float2 _Detail2Tiling;
		uniform sampler2D _Detail2Albedo;
		uniform float2 _NoiseTiling2;
		uniform float _Detail2NoisePower;
		uniform float _CascadeNormalScale;
		uniform float2 _CascadeTiling;
		uniform float _FarNormalPower;
		uniform float _FarNormalBlendStartDistance;
		uniform float _FarNormalBlendThreshold;
		uniform sampler2D _WaterGrab;
		uniform float _Distortion;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _ShalowFalloffMultiply;
		uniform float _ShalowFalloffPower;
		uniform float _CascadeTransparency;
		uniform float _WaveTranslucencyHardness;
		uniform float _WaveTranslucencyPower;
		uniform float _WaveTranslucencyMultiply;
		uniform float _WaveTranslucencyFallOffDistance;
		uniform float4 _DetailAlbedoColor;
		uniform float4 _Detail2AlbedoColor;
		uniform float _WaterSpecularFar;
		uniform float _WaterSpecularClose;
		uniform float _WaterSpecularThreshold;
		uniform float _DetailSpecular;
		uniform float _Detail2Specular;
		uniform float _WaterSmoothness;
		uniform sampler2D _Detail1GSmDetail2ASm;
		uniform float _DetailSmoothness;
		uniform float _Detail2Smoothness;
		uniform half _WaterAOPower;
		uniform half _DetailAOPower;
		uniform half _Detail2AOPower;
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
			float2 appendResult706 = (float2(_WaterMixSpeed.y , _WaterMixSpeed.x));
			float2 uv_TexCoord1484 = v.texcoord.xy * _WaterTiling;
			float Globaltiling1185 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1487_0 = ( uv_TexCoord1484 * Globaltiling1185 );
			float2 panner612 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _WaterMixSpeed :  appendResult706 ) + temp_output_1487_0);
			float2 WaterSpeedValueMix516 = panner612;
			float U1472 = v.texcoord3.xy.x;
			float2 break1475 = ( _WaterMainSpeed * _WaterTiling );
			float temp_output_1477_0 = ( U1472 * break1475.x );
			float V1471 = v.texcoord3.xy.y;
			float temp_output_1478_0 = ( break1475.y * V1471 );
			float2 appendResult1480 = (float2(temp_output_1477_0 , temp_output_1478_0));
			float2 appendResult1481 = (float2(temp_output_1478_0 , temp_output_1477_0));
			float2 temp_output_1482_0 = (( (float)Direction723 == 1.0 ) ? appendResult1480 :  appendResult1481 );
			float temp_output_816_0 = ( _Time.y * 0.05 );
			float Refresh11404 = frac( ( temp_output_816_0 + 1.0 ) );
			float2 WaterSpeedValueMainFlowUV1830 = ( ( temp_output_1482_0 * Refresh11404 ) + temp_output_1487_0 );
			float Refresh21406 = frac( ( temp_output_816_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2831 = ( ( temp_output_1482_0 * Refresh21406 ) + temp_output_1487_0 );
			float clampResult845 = clamp( abs( ( ( Refresh11404 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float WaterSlowHeightBase1314 = clampResult845;
			float3 lerpResult838 = lerp( UnpackScaleNormal( tex2Dlod( _WaterNormal, float4( WaterSpeedValueMainFlowUV1830, 0, 1.0) ), _WaterNormalScale ) , UnpackScaleNormal( tex2Dlod( _WaterNormal, float4( WaterSpeedValueMainFlowUV2831, 0, 1.0) ), _WaterNormalScale ) , WaterSlowHeightBase1314);
			float temp_output_398_0 = ( ( _WaterTessScale * 0.3 ) * tex2Dlod( _WaterTesselation, float4( ( WaterSpeedValueMix516 + ( (lerpResult838).xy * float2( 0.05,0.05 ) ) ), 0, 1.0) ).a );
			float lerpResult840 = lerp( tex2Dlod( _WaterTesselation, float4( WaterSpeedValueMainFlowUV1830, 0, 1.0) ).a , tex2Dlod( _WaterTesselation, float4( WaterSpeedValueMainFlowUV2831, 0, 1.0) ).a , WaterSlowHeightBase1314);
			float temp_output_415_0 = ( temp_output_398_0 + ( _WaterTessScale * lerpResult840 ) );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( (temp_output_415_0*1.0 + ( temp_output_415_0 * -0.5 )) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			int Direction723 = _UVVDirection1UDirection0;
			float2 appendResult706 = (float2(_WaterMixSpeed.y , _WaterMixSpeed.x));
			float2 uv_TexCoord1484 = i.uv_texcoord * _WaterTiling;
			float Globaltiling1185 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1487_0 = ( uv_TexCoord1484 * Globaltiling1185 );
			float2 panner612 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _WaterMixSpeed :  appendResult706 ) + temp_output_1487_0);
			float2 WaterSpeedValueMix516 = panner612;
			float U1472 = i.uv4_texcoord4.x;
			float2 break1475 = ( _WaterMainSpeed * _WaterTiling );
			float temp_output_1477_0 = ( U1472 * break1475.x );
			float V1471 = i.uv4_texcoord4.y;
			float temp_output_1478_0 = ( break1475.y * V1471 );
			float2 appendResult1480 = (float2(temp_output_1477_0 , temp_output_1478_0));
			float2 appendResult1481 = (float2(temp_output_1478_0 , temp_output_1477_0));
			float2 temp_output_1482_0 = (( (float)Direction723 == 1.0 ) ? appendResult1480 :  appendResult1481 );
			float temp_output_816_0 = ( _Time.y * 0.05 );
			float Refresh11404 = frac( ( temp_output_816_0 + 1.0 ) );
			float2 WaterSpeedValueMainFlowUV1830 = ( ( temp_output_1482_0 * Refresh11404 ) + temp_output_1487_0 );
			float Refresh21406 = frac( ( temp_output_816_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2831 = ( ( temp_output_1482_0 * Refresh21406 ) + temp_output_1487_0 );
			float clampResult845 = clamp( abs( ( ( Refresh11404 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float WaterSlowHeightBase1314 = clampResult845;
			float3 lerpResult838 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV1830 ), _WaterNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV2831 ), _WaterNormalScale ) , WaterSlowHeightBase1314);
			float2 temp_output_1246_0 = ( (lerpResult838).xy * float2( 0.05,0.05 ) );
			float3 temp_output_1250_0 = BlendNormals( UnpackScaleNormal( tex2D( _MicroWaveNormal, ( ( _MicroWaveTiling * WaterSpeedValueMix516 ) + temp_output_1246_0 ) ), _MicroWaveNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, ( WaterSpeedValueMix516 + temp_output_1246_0 ) ), _MacroWaveNormalScale ) );
			float3 temp_output_24_0 = BlendNormals( temp_output_1250_0 , lerpResult838 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult259 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_258_0 = ( _CascadeAngle / 45.0 );
			float clampResult263 = clamp( ( clampResult259 - ( 1.0 - temp_output_258_0 ) ) , 0.0 , 2.0 );
			float clampResult584 = clamp( ( clampResult263 * ( 1.0 / temp_output_258_0 ) ) , 0.0 , 1.0 );
			float clampResult285 = clamp( pow( ( 1.0 - clampResult584 ) , _CascadeAngleFalloff ) , 0.0 , 1.0 );
			float WaterfallAngle1144 = clampResult285;
			float2 lerpResult1654 = lerp( _Detail1MainSpeed , _CascadeMainSpeed , ( WaterfallAngle1144 * 0.2 ));
			float2 break1494 = ( lerpResult1654 * _Detail1Tiling );
			float temp_output_1496_0 = ( U1472 * break1494.x );
			float temp_output_1497_0 = ( break1494.y * V1471 );
			float2 appendResult1499 = (float2(temp_output_1496_0 , temp_output_1497_0));
			float2 appendResult1498 = (float2(temp_output_1497_0 , temp_output_1496_0));
			float2 temp_output_1505_0 = (( (float)Direction723 == 1.0 ) ? appendResult1499 :  appendResult1498 );
			float2 uv_TexCoord1504 = i.uv_texcoord * _Detail1Tiling;
			float2 temp_output_1508_0 = ( uv_TexCoord1504 * Globaltiling1185 );
			float2 Detail1SpeedValueMainFlowUV11018 = ( ( temp_output_1505_0 * Refresh11404 ) + temp_output_1508_0 );
			float2 Detail1SpeedValueMainFlowUV21021 = ( ( temp_output_1505_0 * Refresh21406 ) + temp_output_1508_0 );
			float clampResult1291 = clamp( pow( abs( ( ( Refresh11404 + -0.5 ) * 2.0 ) ) , 7.0 ) , 0.0 , 1.0 );
			float SlowFlowHeightBase835 = clampResult1291;
			float3 lerpResult864 = lerp( UnpackScaleNormal( tex2D( _DetailNormal, Detail1SpeedValueMainFlowUV11018 ), _DetailNormalScale ) , UnpackScaleNormal( tex2D( _DetailNormal, Detail1SpeedValueMainFlowUV21021 ), _DetailNormalScale ) , SlowFlowHeightBase835);
			float4 lerpResult935 = lerp( tex2D( _DetailAlbedo, Detail1SpeedValueMainFlowUV11018 ) , tex2D( _DetailAlbedo, Detail1SpeedValueMainFlowUV21021 ) , SlowFlowHeightBase835);
			float2 break1588 = ( lerpResult1654 * _NoiseTiling1 );
			float temp_output_1591_0 = ( U1472 * break1588.x );
			float temp_output_1592_0 = ( break1588.y * V1471 );
			float2 appendResult1594 = (float2(temp_output_1591_0 , temp_output_1592_0));
			float2 appendResult1593 = (float2(temp_output_1592_0 , temp_output_1591_0));
			float2 temp_output_1598_0 = (( (float)Direction723 == 1.0 ) ? appendResult1594 :  appendResult1593 );
			float2 uv_TexCoord1599 = i.uv_texcoord * _NoiseTiling1;
			float2 temp_output_1600_0 = ( uv_TexCoord1599 * Globaltiling1185 );
			float2 NoiseSpeedValueMainFlowUV11064 = ( ( temp_output_1598_0 * Refresh11404 ) + temp_output_1600_0 );
			float2 NoiseSpeedValueMainFlowUV21063 = ( ( temp_output_1598_0 * Refresh21406 ) + temp_output_1600_0 );
			float clampResult1386 = clamp( SlowFlowHeightBase835 , 0.0 , 1.0 );
			float lerpResult1014 = lerp( tex2D( _Noise, NoiseSpeedValueMainFlowUV11064 ).r , tex2D( _Noise, NoiseSpeedValueMainFlowUV21063 ).r , clampResult1386);
			float temp_output_484_0 = pow( lerpResult1014 , _DetailNoisePower );
			float clampResult488 = clamp( temp_output_484_0 , 0.0 , 1.0 );
			float lerpResult1083 = lerp( 0.0 , lerpResult935.a , clampResult488);
			float Detal_1_Alpha_Noise1158 = lerpResult1083;
			float3 lerpResult932 = lerp( temp_output_24_0 , lerpResult864 , Detal_1_Alpha_Noise1158);
			float2 lerpResult1659 = lerp( _Detail2MainSpeed , _CascadeMainSpeed , ( WaterfallAngle1144 * 0.2 ));
			float2 break1516 = ( lerpResult1659 * _Detail2Tiling );
			float temp_output_1518_0 = ( U1472 * break1516.x );
			float temp_output_1519_0 = ( break1516.y * V1471 );
			float2 appendResult1521 = (float2(temp_output_1518_0 , temp_output_1519_0));
			float2 appendResult1520 = (float2(temp_output_1519_0 , temp_output_1518_0));
			float2 temp_output_1524_0 = (( (float)Direction723 == 1.0 ) ? appendResult1521 :  appendResult1520 );
			float2 uv_TexCoord1534 = i.uv_texcoord * _Detail2Tiling;
			float2 temp_output_1527_0 = ( uv_TexCoord1534 * Globaltiling1185 );
			float2 Detail2SpeedValueMainFlowUV11022 = ( ( temp_output_1524_0 * Refresh11404 ) + temp_output_1527_0 );
			float2 Detail2SpeedValueMainFlowUV21025 = ( ( temp_output_1524_0 * Refresh21406 ) + temp_output_1527_0 );
			float clampResult1301 = clamp( pow( abs( ( ( Refresh11404 + -0.5 ) * 2.0 ) ) , 7.0 ) , 0.0 , 1.0 );
			float SlowFlowHeightBase21302 = clampResult1301;
			float3 lerpResult922 = lerp( UnpackScaleNormal( tex2D( _Detail2Normal, Detail2SpeedValueMainFlowUV11022 ), _Detail2NormalScale ) , UnpackScaleNormal( tex2D( _Detail2Normal, Detail2SpeedValueMainFlowUV21025 ), _Detail2NormalScale ) , SlowFlowHeightBase21302);
			float4 lerpResult947 = lerp( tex2D( _Detail2Albedo, Detail2SpeedValueMainFlowUV11022 ) , tex2D( _Detail2Albedo, Detail2SpeedValueMainFlowUV21025 ) , SlowFlowHeightBase21302);
			float2 break1566 = ( lerpResult1659 * _NoiseTiling2 );
			float temp_output_1569_0 = ( U1472 * break1566.x );
			float temp_output_1568_0 = ( break1566.y * V1471 );
			float2 appendResult1570 = (float2(temp_output_1569_0 , temp_output_1568_0));
			float2 appendResult1572 = (float2(temp_output_1568_0 , temp_output_1569_0));
			float2 temp_output_1573_0 = (( (float)Direction723 == 1.0 ) ? appendResult1570 :  appendResult1572 );
			float2 uv_TexCoord1576 = i.uv_texcoord * _NoiseTiling2;
			float2 temp_output_1579_0 = ( uv_TexCoord1576 * Globaltiling1185 );
			float2 NoiseSpeedValueMainFlowUV1v21583 = ( ( temp_output_1573_0 * Refresh11404 ) + temp_output_1579_0 );
			float2 NoiseSpeedValueMainFlowUV2v21584 = ( ( temp_output_1573_0 * Refresh21406 ) + temp_output_1579_0 );
			float clampResult1387 = clamp( SlowFlowHeightBase21302 , 0.0 , 1.0 );
			float lerpResult1016 = lerp( tex2D( _Noise, NoiseSpeedValueMainFlowUV1v21583 ).r , tex2D( _Noise, NoiseSpeedValueMainFlowUV2v21584 ).r , clampResult1387);
			float temp_output_960_0 = pow( max( lerpResult1016 , lerpResult1014 ) , _Detail2NoisePower );
			float clampResult962 = clamp( temp_output_960_0 , 0.0 , 1.0 );
			float lerpResult1125 = lerp( 0.0 , lerpResult947.a , clampResult962);
			float Detal_2_Alpha_Noise1159 = lerpResult1125;
			float3 lerpResult933 = lerp( lerpResult932 , lerpResult922 , Detal_2_Alpha_Noise1159);
			float Detal_1_Alpha1146 = lerpResult935.a;
			float3 lerpResult1134 = lerp( temp_output_24_0 , lerpResult864 , Detal_1_Alpha1146);
			float4 VertexColorRGB1172 = ( i.vertexColor / float4( 1,1,1,1 ) );
			float4 break1177 = VertexColorRGB1172;
			float3 lerpResult748 = lerp( lerpResult933 , lerpResult1134 , break1177.r);
			float Detal_2_Alpha1152 = lerpResult947.a;
			float3 lerpResult1135 = lerp( temp_output_24_0 , lerpResult922 , Detal_2_Alpha1152);
			float3 lerpResult749 = lerp( lerpResult748 , lerpResult1135 , break1177.g);
			float3 lerpResult750 = lerp( lerpResult749 , temp_output_24_0 , break1177.b);
			float2 break1609 = ( _CascadeMainSpeed * _CascadeTiling );
			float temp_output_1611_0 = ( U1472 * break1609.x );
			float temp_output_1610_0 = ( break1609.y * V1471 );
			float2 appendResult1613 = (float2(temp_output_1611_0 , temp_output_1610_0));
			float2 appendResult1614 = (float2(temp_output_1610_0 , temp_output_1611_0));
			float2 temp_output_1616_0 = (( (float)Direction723 == 1.0 ) ? appendResult1613 :  appendResult1614 );
			float temp_output_1639_0 = ( _Time.y * 0.1 );
			float Refresh1v21643 = frac( ( temp_output_1639_0 + 1.0 ) );
			float2 uv_TexCoord1617 = i.uv_texcoord * _CascadeTiling;
			float2 temp_output_1622_0 = ( uv_TexCoord1617 * Globaltiling1185 );
			float2 SmallCascadeSpeedValueMainFlowUV11625 = ( ( temp_output_1616_0 * Refresh1v21643 ) + temp_output_1622_0 );
			float Refresh2v21645 = frac( ( temp_output_1639_0 + 0.5 ) );
			float2 SmallCascadeSpeedValueMainFlowUV21626 = ( ( temp_output_1616_0 * Refresh2v21645 ) + temp_output_1622_0 );
			float clampResult1650 = clamp( abs( ( ( Refresh1v21643 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float WaterSlowHeightBase21651 = clampResult1650;
			float3 lerpResult1634 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeSpeedValueMainFlowUV11625 ), _CascadeNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeSpeedValueMainFlowUV21626 ), _CascadeNormalScale ) , WaterSlowHeightBase21651);
			float3 lerpResult983 = lerp( lerpResult750 , BlendNormals( temp_output_1250_0 , lerpResult1634 ) , WaterfallAngle1144);
			float3 appendResult1422 = (float3(_FarNormalPower , _FarNormalPower , 1.0));
			float3 ase_worldPos = i.worldPos;
			float Distance1207 = distance( ase_worldPos , _WorldSpaceCameraPos );
			float clampResult1423 = clamp( pow( ( Distance1207 / _FarNormalBlendStartDistance ) , _FarNormalBlendThreshold ) , 0.0 , 1.0 );
			float3 lerpResult1425 = lerp( lerpResult983 , ( lerpResult983 * appendResult1422 ) , clampResult1423);
			o.Normal = lerpResult1425;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult163 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float4 screenColor65 = tex2D( _WaterGrab, ( float3( ( appendResult163 / ase_grabScreenPosNorm.a ) ,  0.0 ) + ( lerpResult933 * _Distortion ) ).xy );
			float eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float temp_output_89_0 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float lerpResult1661 = lerp( pow( ( temp_output_89_0 * _ShalowFalloffMultiply ) , ( _ShalowFalloffPower * -1.0 ) ) , 100.0 , ( _CascadeTransparency * WaterfallAngle1144 ));
			float clampResult1259 = clamp( saturate( lerpResult1661 ) , 0.0 , 1.0 );
			float4 lerpResult13 = lerp( _DeepColor , _ShalowColor , clampResult1259);
			float temp_output_398_0 = ( ( _WaterTessScale * 0.3 ) * tex2D( _WaterTesselation, ( WaterSpeedValueMix516 + ( (lerpResult838).xy * float2( 0.05,0.05 ) ) ) ).a );
			float lerpResult840 = lerp( tex2D( _WaterTesselation, WaterSpeedValueMainFlowUV1830 ).a , tex2D( _WaterTesselation, WaterSpeedValueMainFlowUV2831 ).a , WaterSlowHeightBase1314);
			float clampResult1216 = clamp( ( Distance1207 / _WaveTranslucencyFallOffDistance ) , 0.0 , 1.0 );
			float lerpResult1217 = lerp( ( pow( abs( ( ( temp_output_398_0 + lerpResult840 ) * _WaveTranslucencyHardness ) ) , _WaveTranslucencyPower ) * _WaveTranslucencyMultiply ) , 0.0 , clampResult1216);
			float clampResult1218 = clamp( lerpResult1217 , 0.0 , 1.0 );
			float Microwaves1219 = clampResult1218;
			float4 lerpResult1258 = lerp( lerpResult13 , _ShalowColor , Microwaves1219);
			float4 lerpResult773 = lerp( screenColor65 , lerpResult1258 , ( 1.0 - clampResult1259 ));
			float4 temp_output_1106_0 = ( lerpResult935 * _DetailAlbedoColor );
			float4 lerpResult964 = lerp( lerpResult773 , temp_output_1106_0 , Detal_1_Alpha_Noise1158);
			float4 temp_output_1108_0 = ( lerpResult947 * _Detail2AlbedoColor );
			float4 lerpResult987 = lerp( lerpResult964 , temp_output_1108_0 , Detal_2_Alpha_Noise1159);
			float4 lerpResult1132 = lerp( lerpResult773 , temp_output_1106_0 , Detal_1_Alpha1146);
			float4 break1181 = VertexColorRGB1172;
			float4 lerpResult986 = lerp( lerpResult987 , lerpResult1132 , break1181.r);
			float4 lerpResult1133 = lerp( lerpResult773 , temp_output_1108_0 , Detal_2_Alpha1152);
			float4 lerpResult984 = lerp( lerpResult986 , lerpResult1133 , break1181.g);
			float4 lerpResult1113 = lerp( lerpResult984 , lerpResult773 , break1181.b);
			float4 lerpResult1058 = lerp( lerpResult1113 , lerpResult773 , WaterfallAngle1144);
			o.Albedo = lerpResult1058.rgb;
			float lerpResult1124 = lerp( _WaterSpecularFar , _WaterSpecularClose , pow( clampResult1259 , _WaterSpecularThreshold ));
			float4 temp_cast_16 = (lerpResult1124).xxxx;
			float4 clampResult1050 = clamp( ( temp_output_1106_0 * float4( 0.3,0.3019608,0.3019608,0 ) ) , float4( 0,0,0,0 ) , float4( 0.5,0.5019608,0.5019608,0 ) );
			float4 temp_output_1051_0 = ( _DetailSpecular * clampResult1050 );
			float4 lerpResult969 = lerp( temp_cast_16 , temp_output_1051_0 , Detal_1_Alpha_Noise1158);
			float4 clampResult1053 = clamp( ( lerpResult947 * float4( 0.3,0.3019608,0.3019608,0 ) ) , float4( 0,0,0,0 ) , float4( 0.5,0.5019608,0.5019608,0 ) );
			float4 temp_output_1052_0 = ( _Detail2Specular * clampResult1053 );
			float4 lerpResult970 = lerp( lerpResult969 , temp_output_1052_0 , Detal_2_Alpha_Noise1159);
			float4 temp_cast_17 = (lerpResult1124).xxxx;
			float4 lerpResult1136 = lerp( temp_cast_17 , temp_output_1051_0 , Detal_1_Alpha1146);
			float4 break1179 = VertexColorRGB1172;
			float4 lerpResult130 = lerp( lerpResult970 , lerpResult1136 , break1179.r);
			float4 temp_cast_18 = (lerpResult1124).xxxx;
			float4 lerpResult1137 = lerp( temp_cast_18 , temp_output_1052_0 , Detal_2_Alpha1152);
			float4 lerpResult785 = lerp( lerpResult130 , lerpResult1137 , break1179.g);
			float4 temp_cast_19 = (lerpResult1124).xxxx;
			float4 lerpResult786 = lerp( lerpResult785 , temp_cast_19 , break1179.b);
			float4 temp_cast_20 = (lerpResult1124).xxxx;
			float4 lerpResult982 = lerp( lerpResult786 , temp_cast_20 , WaterfallAngle1144);
			o.Specular = lerpResult982.rgb;
			float4 lerpResult1089 = lerp( tex2D( _Detail1GSmDetail2ASm, Detail1SpeedValueMainFlowUV11018 ) , tex2D( _Detail1GSmDetail2ASm, Detail1SpeedValueMainFlowUV21021 ) , SlowFlowHeightBase835);
			float temp_output_1093_0 = ( lerpResult1089.g * _DetailSmoothness );
			float lerpResult973 = lerp( _WaterSmoothness , temp_output_1093_0 , Detal_1_Alpha_Noise1158);
			float4 lerpResult1102 = lerp( tex2D( _Detail1GSmDetail2ASm, Detail2SpeedValueMainFlowUV11022 ) , tex2D( _Detail1GSmDetail2ASm, Detail2SpeedValueMainFlowUV21025 ) , SlowFlowHeightBase21302);
			float temp_output_1094_0 = ( lerpResult1102.a * _Detail2Smoothness );
			float lerpResult974 = lerp( lerpResult973 , temp_output_1094_0 , Detal_2_Alpha_Noise1159);
			float lerpResult1129 = lerp( _WaterSmoothness , temp_output_1093_0 , Detal_1_Alpha1146);
			float4 break1174 = VertexColorRGB1172;
			float lerpResult975 = lerp( lerpResult974 , lerpResult1129 , break1174.r);
			float lerpResult1127 = lerp( _WaterSmoothness , temp_output_1094_0 , Detal_2_Alpha1152);
			float lerpResult972 = lerp( lerpResult975 , lerpResult1127 , break1174.g);
			float lerpResult971 = lerp( lerpResult972 , _WaterSmoothness , break1174.b);
			float lerpResult981 = lerp( lerpResult971 , _WaterSmoothness , WaterfallAngle1144);
			o.Smoothness = lerpResult981;
			float lerpResult1038 = lerp( _WaterAOPower , _DetailAOPower , Detal_1_Alpha_Noise1158);
			float lerpResult1039 = lerp( lerpResult1038 , _Detail2AOPower , Detal_2_Alpha_Noise1159);
			float lerpResult1130 = lerp( _WaterAOPower , _DetailAOPower , Detal_1_Alpha1146);
			float4 break1175 = VertexColorRGB1172;
			float lerpResult1040 = lerp( lerpResult1039 , lerpResult1130 , break1175.r);
			float lerpResult1128 = lerp( _WaterAOPower , _Detail2AOPower , Detal_2_Alpha1152);
			float lerpResult1041 = lerp( lerpResult1040 , lerpResult1128 , break1175.g);
			float lerpResult1042 = lerp( lerpResult1041 , _WaterAOPower , break1175.b);
			float lerpResult1057 = lerp( lerpResult1042 , _WaterAOPower , WaterfallAngle1144);
			o.Occlusion = lerpResult1057;
			float clampResult1234 = clamp( ( temp_output_89_0 * _CleanFalloffMultiply ) , 0.0 , 1.0 );
			float clampResult1238 = clamp( pow( abs( clampResult1234 ) , _CleanFalloffPower ) , 0.0 , 1.0 );
			float temp_output_779_0 = ( clampResult1238 * i.vertexColor.a );
			float switchResult1230 = (((i.ASEVFace>0)?(temp_output_779_0):(( temp_output_779_0 * _BackfaceAlpha ))));
			o.Alpha = switchResult1230;
		}

		ENDCG
	}
}