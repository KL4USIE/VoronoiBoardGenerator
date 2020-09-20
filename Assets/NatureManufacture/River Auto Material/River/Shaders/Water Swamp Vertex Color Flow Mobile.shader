Shader "NatureManufacture Shaders/Water/Water Swamp Vertex Color Flow Mobile"
{
	Properties
	{
		[Toggle(_USEGRABSCREENCOLOR_ON)] _UseGrabScreenColor("UseGrabScreenColor", Float) = 1
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
		_WaterSpecularClose("Water Specular Close", Range( 0 , 1)) = 0
		_WaterSpecularFar("Water Specular Far", Range( 0 , 1)) = 0
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0
		_Distortion("Distortion", Float) = 0.5
		_WaterTiling("Water Tiling", Vector) = (3,3,0,0)
		_WaterMainSpeed("Water Main Speed", Vector) = (0.2,0.2,0,0)
		_WaterMixSpeed("Water Mix Speed", Vector) = (0.01,0.05,0,0)
		[NoScaleOffset]_WaterNormal("Water Normal", 2D) = "bump" {}
		_WaterNormalScale("Water Normal Scale", Float) = 0.3
		_MacroWaveNormalScale("Macro Wave Normal Scale", Range( 0 , 2)) = 0.33
		_WaterSpecularThreshold("Water Specular Threshold", Range( 0 , 10)) = 1
		_CascadeMainSpeed("Cascade Main Speed", Vector) = (2,2,0,0)
		_CascadeNormalScale("Cascade Normal Scale", Float) = 0.7
		_CascadeAngle("Cascade Angle", Range( 0.001 , 90)) = 90
		_CascadeAngleFalloff("Cascade Angle Falloff", Range( 0 , 80)) = 5
		_CascadeTiling("Cascade Tiling", Vector) = (2,3,0,0)
		_Detail1Tiling("Detail 1 Tiling", Vector) = (3,3,0,0)
		_Detail1MainSpeed("Detail 1 Main Speed", Vector) = (0.2,0.2,0,0)
		_DetailNoisePower("Detail Noise Power", Range( 0 , 10)) = 2.71
		[NoScaleOffset]_DetailAlbedo("Detail Albedo", 2D) = "black" {}
		_DetailAlbedoColor("Detail Albedo Color", Color) = (1,1,1,0)
		[NoScaleOffset]_DetailNormal("Detail Normal", 2D) = "bump" {}
		_DetailNormalScale("Detail Normal Scale", Float) = 0
		_NoiseTiling1("Noise Tiling 1", Vector) = (4,4,0,0)
		[NoScaleOffset]_NoiseRDetail1ASm("Noise (R) Detail 1 (A)Sm", 2D) = "white" {}
		_CascadeTransparency("Cascade Transparency", Range( 0 , 1)) = 0
		_DetailSmoothness("Detail Smoothness", Range( 0 , 1)) = 0
		_DetailSpecular("Detail Specular", Range( 0 , 1)) = 0
		_DetailAOPower("Detail AO Power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
			float2 uv4_texcoord4;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
			float4 screenPos;
			half ASEVFace : VFACE;
		};

		uniform sampler2D _WaterNormal;
		uniform float _MacroWaveNormalScale;
		uniform int _UVVDirection1UDirection0;
		uniform float2 _WaterMixSpeed;
		uniform float2 _WaterTiling;
		uniform float _GlobalTiling;
		uniform float _WaterNormalScale;
		uniform float2 _WaterMainSpeed;
		uniform sampler2D _DetailNormal;
		uniform float _DetailNormalScale;
		uniform float2 _Detail1MainSpeed;
		uniform float2 _CascadeMainSpeed;
		uniform half _CascadeAngle;
		uniform float _CascadeAngleFalloff;
		uniform float2 _Detail1Tiling;
		uniform sampler2D _DetailAlbedo;
		uniform sampler2D _NoiseRDetail1ASm;
		uniform float2 _NoiseTiling1;
		uniform float _DetailNoisePower;
		uniform float _CascadeNormalScale;
		uniform float2 _CascadeTiling;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _ShalowFalloffMultiply;
		uniform float _ShalowFalloffPower;
		uniform float _CascadeTransparency;
		uniform sampler2D _WaterGrab;
		uniform float _Distortion;
		uniform float4 _DetailAlbedoColor;
		uniform float _WaterSpecularFar;
		uniform float _WaterSpecularClose;
		uniform float _WaterSpecularThreshold;
		uniform float _DetailSpecular;
		uniform float _WaterSmoothness;
		uniform float _DetailSmoothness;
		uniform half _WaterAOPower;
		uniform half _DetailAOPower;
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
			float2 appendResult706 = (float2(_WaterMixSpeed.y , _WaterMixSpeed.x));
			float2 uv_TexCoord1302 = i.uv_texcoord * _WaterTiling;
			float Globaltiling1154 = ( 1.0 / _GlobalTiling );
			float2 temp_output_1306_0 = ( uv_TexCoord1302 * Globaltiling1154 );
			float2 panner612 = ( _Time.y * (( (float)Direction723 == 1.0 ) ? _WaterMixSpeed :  appendResult706 ) + temp_output_1306_0);
			float2 WaterSpeedValueMix516 = panner612;
			float U1284 = i.uv4_texcoord4.x;
			float2 break1289 = ( _WaterMainSpeed * _WaterTiling );
			float temp_output_1294_0 = ( U1284 * break1289.x );
			float V1285 = i.uv4_texcoord4.y;
			float temp_output_1292_0 = ( break1289.y * V1285 );
			float2 appendResult1295 = (float2(temp_output_1294_0 , temp_output_1292_0));
			float2 appendResult1296 = (float2(temp_output_1292_0 , temp_output_1294_0));
			float2 temp_output_1304_0 = (( 0 == 1.0 ) ? appendResult1295 :  appendResult1296 );
			float temp_output_816_0 = ( _Time.y * 0.05 );
			float Refresh11189 = frac( ( temp_output_816_0 + 1.0 ) );
			float2 WaterSpeedValueMainFlowUV1830 = ( ( temp_output_1304_0 * Refresh11189 ) + temp_output_1306_0 );
			float Refresh21190 = frac( ( temp_output_816_0 + 0.5 ) );
			float2 WaterSpeedValueMainFlowUV2831 = ( ( temp_output_1304_0 * Refresh21190 ) + temp_output_1306_0 );
			float clampResult1250 = clamp( abs( ( ( Refresh11189 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float WaterSlowHeightBase1252 = clampResult1250;
			float3 lerpResult838 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV1830 ), _WaterNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, WaterSpeedValueMainFlowUV2831 ), _WaterNormalScale ) , WaterSlowHeightBase1252);
			float3 tex2DNode23 = UnpackScaleNormal( tex2D( _WaterNormal, ( WaterSpeedValueMix516 + ( (lerpResult838).xy * float2( 0.05,0.05 ) ) ) ), _MacroWaveNormalScale );
			float3 temp_output_24_0 = BlendNormals( tex2DNode23 , lerpResult838 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult259 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float temp_output_258_0 = ( _CascadeAngle / 45.0 );
			float clampResult263 = clamp( ( clampResult259 - ( 1.0 - temp_output_258_0 ) ) , 0.0 , 2.0 );
			float clampResult584 = clamp( ( clampResult263 * ( 1.0 / temp_output_258_0 ) ) , 0.0 , 1.0 );
			float clampResult285 = clamp( pow( ( 1.0 - clampResult584 ) , _CascadeAngleFalloff ) , 0.0 , 1.0 );
			float WaterfallAng1120 = clampResult285;
			float2 lerpResult1301 = lerp( _Detail1MainSpeed , _CascadeMainSpeed , ( WaterfallAng1120 * 0.2 ));
			float2 break1319 = ( lerpResult1301 * _Detail1Tiling );
			float temp_output_1324_0 = ( U1284 * break1319.x );
			float temp_output_1321_0 = ( break1319.y * V1285 );
			float2 appendResult1331 = (float2(temp_output_1324_0 , temp_output_1321_0));
			float2 appendResult1330 = (float2(temp_output_1321_0 , temp_output_1324_0));
			float2 temp_output_1337_0 = (( (float)Direction723 == 1.0 ) ? appendResult1331 :  appendResult1330 );
			float2 uv_TexCoord1341 = i.uv_texcoord * _Detail1Tiling;
			float2 temp_output_1342_0 = ( uv_TexCoord1341 * Globaltiling1154 );
			float2 Detail1SpeedValueMainFlowUV11018 = ( ( temp_output_1337_0 * Refresh11189 ) + temp_output_1342_0 );
			float2 Detail1SpeedValueMainFlowUV21021 = ( ( temp_output_1337_0 * Refresh21190 ) + temp_output_1342_0 );
			float clampResult1259 = clamp( pow( abs( ( ( Refresh11189 + -0.5 ) * 2.0 ) ) , 7.0 ) , 0.0 , 1.0 );
			float SlowFlowHeightBase1262 = clampResult1259;
			float3 lerpResult864 = lerp( UnpackScaleNormal( tex2D( _DetailNormal, Detail1SpeedValueMainFlowUV11018 ), _DetailNormalScale ) , UnpackScaleNormal( tex2D( _DetailNormal, Detail1SpeedValueMainFlowUV21021 ), _DetailNormalScale ) , SlowFlowHeightBase1262);
			float4 lerpResult935 = lerp( tex2D( _DetailAlbedo, Detail1SpeedValueMainFlowUV11018 ) , tex2D( _DetailAlbedo, Detail1SpeedValueMainFlowUV21021 ) , SlowFlowHeightBase1262);
			float2 break1314 = ( lerpResult1301 * _NoiseTiling1 );
			float temp_output_1318_0 = ( U1284 * break1314.x );
			float temp_output_1317_0 = ( break1314.y * V1285 );
			float2 appendResult1323 = (float2(temp_output_1318_0 , temp_output_1317_0));
			float2 appendResult1322 = (float2(temp_output_1317_0 , temp_output_1318_0));
			float2 temp_output_1332_0 = (( (float)Direction723 == 1.0 ) ? appendResult1323 :  appendResult1322 );
			float2 uv_TexCoord1328 = i.uv_texcoord * _NoiseTiling1;
			float2 temp_output_1339_0 = ( uv_TexCoord1328 * Globaltiling1154 );
			float2 NoiseSpeedValueMainFlowUV11064 = ( ( temp_output_1332_0 * Refresh11189 ) + temp_output_1339_0 );
			float2 NoiseSpeedValueMainFlowUV21063 = ( ( temp_output_1332_0 * Refresh21190 ) + temp_output_1339_0 );
			float lerpResult1014 = lerp( tex2D( _NoiseRDetail1ASm, NoiseSpeedValueMainFlowUV11064 ).r , tex2D( _NoiseRDetail1ASm, NoiseSpeedValueMainFlowUV21063 ).r , SlowFlowHeightBase1262);
			float clampResult488 = clamp( pow( lerpResult1014 , _DetailNoisePower ) , 0.0 , 1.0 );
			float lerpResult1083 = lerp( 0.0 , lerpResult935.a , clampResult488);
			float Detal_1_Alpha_Noi1124 = lerpResult1083;
			float3 lerpResult932 = lerp( temp_output_24_0 , lerpResult864 , Detal_1_Alpha_Noi1124);
			float Detal_1_Alp1122 = lerpResult935.a;
			float3 lerpResult1148 = lerp( lerpResult932 , lerpResult864 , Detal_1_Alp1122);
			float4 VertexColorRB1126 = ( i.vertexColor / float4( 1,1,1,1 ) );
			float4 break1134 = VertexColorRB1126;
			float3 lerpResult748 = lerp( lerpResult932 , lerpResult1148 , break1134.r);
			float3 lerpResult750 = lerp( lerpResult748 , temp_output_24_0 , break1134.b);
			float2 break1350 = ( _CascadeMainSpeed * _CascadeTiling );
			float temp_output_1354_0 = ( U1284 * break1350.x );
			float temp_output_1353_0 = ( break1350.y * V1285 );
			float2 appendResult1356 = (float2(temp_output_1354_0 , temp_output_1353_0));
			float2 appendResult1355 = (float2(temp_output_1353_0 , temp_output_1354_0));
			float2 temp_output_1362_0 = (( (float)Direction723 == 1.0 ) ? appendResult1356 :  appendResult1355 );
			float temp_output_1371_0 = ( _Time.y * 0.03 );
			float Refresh1v21376 = frac( ( temp_output_1371_0 + 1.0 ) );
			float2 uv_TexCoord1359 = i.uv_texcoord * _CascadeTiling;
			float2 temp_output_1363_0 = ( uv_TexCoord1359 * Globaltiling1154 );
			float2 SmallCascadeSpeedValueMainFlowUV11387 = ( ( temp_output_1362_0 * Refresh1v21376 ) + temp_output_1363_0 );
			float Refresh2v21377 = frac( ( temp_output_1371_0 + 0.5 ) );
			float2 SmallCascadeSpeedValueMainFlowUV21386 = ( ( temp_output_1362_0 * Refresh2v21377 ) + temp_output_1363_0 );
			float clampResult1381 = clamp( abs( ( ( Refresh1v21376 + -0.5 ) * 2.0 ) ) , 0.0 , 1.0 );
			float WaterSlowHeightBase21382 = clampResult1381;
			float3 lerpResult1394 = lerp( UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeSpeedValueMainFlowUV11387 ), _CascadeNormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, SmallCascadeSpeedValueMainFlowUV21386 ), _CascadeNormalScale ) , WaterSlowHeightBase21382);
			float3 lerpResult983 = lerp( lerpResult750 , BlendNormals( tex2DNode23 , lerpResult1394 ) , WaterfallAng1120);
			o.Normal = lerpResult983;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float temp_output_89_0 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float lerpResult1419 = lerp( pow( ( temp_output_89_0 * _ShalowFalloffMultiply ) , ( _ShalowFalloffPower * -1.0 ) ) , 100.0 , ( _CascadeTransparency * WaterfallAng1120 ));
			float clampResult1188 = clamp( saturate( lerpResult1419 ) , 0.0 , 1.0 );
			float4 lerpResult13 = lerp( _DeepColor , _ShalowColor , clampResult1188);
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult163 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float4 screenColor65 = tex2D( _WaterGrab, ( float3( ( appendResult163 / ase_grabScreenPosNorm.a ) ,  0.0 ) + ( lerpResult932 * _Distortion ) ).xy );
			float4 lerpResult773 = lerp( screenColor65 , lerpResult13 , ( 1.0 - clampResult1188 ));
			#ifdef _USEGRABSCREENCOLOR_ON
				float4 staticSwitch1417 = lerpResult773;
			#else
				float4 staticSwitch1417 = lerpResult13;
			#endif
			float4 temp_output_1106_0 = ( lerpResult935 * _DetailAlbedoColor );
			float4 lerpResult964 = lerp( staticSwitch1417 , temp_output_1106_0 , Detal_1_Alpha_Noi1124);
			float4 lerpResult1151 = lerp( lerpResult773 , temp_output_1106_0 , Detal_1_Alp1122);
			float4 break1136 = VertexColorRB1126;
			float4 lerpResult986 = lerp( lerpResult964 , lerpResult1151 , break1136.r);
			float4 lerpResult1113 = lerp( lerpResult986 , lerpResult773 , break1136.b);
			float4 lerpResult1058 = lerp( lerpResult1113 , lerpResult773 , WaterfallAng1120);
			o.Albedo = lerpResult1058.rgb;
			float lerpResult1119 = lerp( _WaterSpecularFar , _WaterSpecularClose , pow( clampResult1188 , _WaterSpecularThreshold ));
			float4 temp_cast_10 = (lerpResult1119).xxxx;
			float4 clampResult1050 = clamp( ( temp_output_1106_0 * float4( 0.3,0.3019608,0.3019608,0 ) ) , float4( 0,0,0,0 ) , float4( 0.5,0.5019608,0.5019608,0 ) );
			float4 temp_output_1051_0 = ( _DetailSpecular * clampResult1050 );
			float4 lerpResult969 = lerp( temp_cast_10 , temp_output_1051_0 , Detal_1_Alpha_Noi1124);
			float4 temp_cast_11 = (lerpResult1119).xxxx;
			float4 lerpResult1145 = lerp( temp_cast_11 , temp_output_1051_0 , Detal_1_Alp1122);
			float4 break1132 = VertexColorRB1126;
			float4 lerpResult130 = lerp( lerpResult969 , lerpResult1145 , break1132.r);
			float4 temp_cast_12 = (lerpResult1119).xxxx;
			float4 lerpResult786 = lerp( lerpResult130 , temp_cast_12 , break1132.b);
			float4 temp_cast_13 = (lerpResult1119).xxxx;
			float4 lerpResult982 = lerp( lerpResult786 , temp_cast_13 , WaterfallAng1120);
			o.Specular = lerpResult982.rgb;
			float lerpResult1089 = lerp( tex2D( _NoiseRDetail1ASm, Detail1SpeedValueMainFlowUV11018 ).a , tex2D( _NoiseRDetail1ASm, Detail1SpeedValueMainFlowUV21021 ).a , SlowFlowHeightBase1262);
			float temp_output_1093_0 = ( lerpResult1089 * _DetailSmoothness );
			float lerpResult973 = lerp( _WaterSmoothness , temp_output_1093_0 , Detal_1_Alpha_Noi1124);
			float lerpResult1140 = lerp( _WaterSmoothness , temp_output_1093_0 , Detal_1_Alp1122);
			float4 break1130 = VertexColorRB1126;
			float lerpResult975 = lerp( lerpResult973 , lerpResult1140 , break1130.r);
			float lerpResult971 = lerp( lerpResult975 , _WaterSmoothness , break1130.b);
			float lerpResult981 = lerp( lerpResult971 , _WaterSmoothness , WaterfallAng1120);
			o.Smoothness = lerpResult981;
			float lerpResult1038 = lerp( _WaterAOPower , _DetailAOPower , Detal_1_Alpha_Noi1124);
			float lerpResult1142 = lerp( _WaterAOPower , _DetailAOPower , Detal_1_Alp1122);
			float4 break1129 = VertexColorRB1126;
			float lerpResult1040 = lerp( lerpResult1038 , lerpResult1142 , break1129.r);
			float lerpResult1042 = lerp( lerpResult1040 , _WaterAOPower , break1129.b);
			float lerpResult1057 = lerp( lerpResult1042 , _WaterAOPower , WaterfallAng1120);
			o.Occlusion = lerpResult1057;
			float clampResult1168 = clamp( ( temp_output_89_0 * _CleanFalloffMultiply ) , 0.0 , 1.0 );
			float clampResult1172 = clamp( pow( abs( clampResult1168 ) , _CleanFalloffPower ) , 0.0 , 1.0 );
			float temp_output_779_0 = ( clampResult1172 * i.vertexColor.a );
			float switchResult1164 = (((i.ASEVFace>0)?(temp_output_779_0):(( temp_output_779_0 * _BackfaceAlpha ))));
			o.Alpha = switchResult1164;
		}

		ENDCG
	}
}