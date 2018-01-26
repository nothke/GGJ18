Shader "Custom/NoiseTV" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Emission", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", float) = 1 
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _Emission;
		fixed4 _Color;

		float rand(float3 co)
		{
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
/*
			IN.uv_MainTex.x += _Time.y * _TilingSpeed.x;
			IN.uv_MainTex.y += _Time.y * _TilingSpeed.y;
*/
			IN.uv_MainTex.x = rand(_Time.yzw * IN.uv_MainTex.x);
			IN.uv_MainTex.y = rand(_Time.yzw * IN.uv_MainTex.y);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			//o.Albedo = c.rgb;
			o.Emission = c.rgb * _Emission;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
