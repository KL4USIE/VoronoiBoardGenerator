Shader "NatureManufacture Shaders/Water/Water Particles Foam"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,0)
		_ColorPower("Color Power", Vector) = (5,5,5,0)
		_AOPower("AO Power", Range( 0 , 1)) = 1
		_Opacity("Opacity", Range( 0 , 20)) = 0.23
		_ParticleTexture("Particle Texture", 2D) = "white" {}
		_ParticleNormalmap("Particle Normalmap", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( -5 , 5)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.8
		_Metallic("Metallic", Range( 0 , 1)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _NormalScale;
		uniform sampler2D _ParticleNormalmap;
		uniform float4 _ParticleNormalmap_ST;
		uniform float3 _ColorPower;
		uniform float4 _Color;
		uniform sampler2D _ParticleTexture;
		uniform float4 _ParticleTexture_ST;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _AOPower;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ParticleNormalmap = i.uv_texcoord * _ParticleNormalmap_ST.xy + _ParticleNormalmap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _ParticleNormalmap, uv_ParticleNormalmap ), _NormalScale );
			float2 uv_ParticleTexture = i.uv_texcoord * _ParticleTexture_ST.xy + _ParticleTexture_ST.zw;
			float4 tex2DNode34 = tex2D( _ParticleTexture, uv_ParticleTexture );
			o.Albedo = ( ( ( float4( _ColorPower , 0.0 ) * _Color ) * tex2DNode34 ) * i.vertexColor ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Occlusion = _AOPower;
			float clampResult40 = clamp( ( _Opacity * tex2DNode34.a ) , 0.0 , 1.0 );
			o.Alpha = ( i.vertexColor.a * clampResult40 );
		}

		ENDCG
	}
}