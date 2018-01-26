// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Spots"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "noise.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float rand(float2 co) {
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				
				/*
				fixed hLine = rand(round(rand(i.uv.y * _Time.z * 1) * 3 + i.uv.x * 20));
				fixed hLine2 = round(-0.4 + rand(i.uv.x * _Time));
				
				
				
				
				fixed4 whitenoise = min(hLine, round(-0.4 + rand(i.uv.y * _Time)));
				whitenoise.a = 1;*/

				fixed4 whitenoise = saturate(round(- 0.48  + noiseIQ(fixed3(i.uv.x * 10 + _Time.x * 500, i.uv.y * 30 + _Time.y * 800, 1))));
				fixed4 noise2 = saturate(-0.90 + noiseIQ(fixed3(i.uv.x * 200 + _Time.x * 5234, i.uv.y * 100 + _Time.y * 8123, 1)));

				//whitenoise *= lerp(fixed4(1, 1, 0, 1), fixed4(1, 0, 1, 1), rand(i.uv.x));

				return c + whitenoise + noise2;
			}
			ENDCG
		}
	}
}
