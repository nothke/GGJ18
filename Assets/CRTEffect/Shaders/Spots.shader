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
			
			sampler2D _MainTex; // Not actually used, just using render textures, for simplicity - not performance
			sampler2D _Tex1;
			sampler2D _Tex2;
			// Add more textures here..

			float _NoiseThreshold;
			float _SpotsSize;
			float _Intensity;
			float _ScrollPosition;
			float _LinesThreshold;
			float _ChannelBlend;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 uv = fixed2(i.uv.x, (i.uv.y + _ScrollPosition) % 1);
				fixed4 c = tex2D(_Tex1, uv);
				fixed4 c2 = tex2D(_Tex2, uv);
				
				/*
				fixed hLine = rand(round(rand(i.uv.y * _Time.z * 1) * 3 + i.uv.x * 20));
				fixed hLine2 = round(-0.4 + rand(i.uv.x * _Time));
				
				
				
				
				fixed4 whitenoise = min(hLine, round(-0.4 + rand(i.uv.y * _Time)));
				whitenoise.a = 1;*/

				float spotsThreshold = -0.5 + _SpotsSize * .5;

				// Spots
				fixed4 bigSpots = saturate(round(spotsThreshold + noiseIQ(fixed3(
					(i.uv.x * 56.553 + _Time.x * 334.34),
					(i.uv.y * 56.551 + _Time.y * 567.23), 1))));
				bigSpots = saturate(bigSpots + round(spotsThreshold + noiseIQ(fixed3(
					i.uv.x * 55.45 + rand(_Time.y) * 45 + _Time.x * 523,
					i.uv.y * 55.45 + _Time.y * 45, 1))));
				fixed4 bigSpots2 = -saturate(round(spotsThreshold + noiseIQ(fixed3(
					i.uv.x * 10.324 + rand(_Time.y + i.uv.y * 0.002) * 10 + _Time.x * 34.3,
					i.uv.y * 10.324 + _Time.y * 34.3, 1))));

				// Lines
				fixed lineWidth = 20.242341;
				
				fixed linesMult = 3;
				fixed linesThres = 0.5 - ((_LinesThreshold) * linesMult);
				fixed lines = saturate(linesThres + ((noiseIQ(fixed3(i.uv.y * lineWidth, _Time.x * 34.2321, 0.234523)))) * linesMult);

				//bigSpots *= lines;

				// Snow
				fixed4 snow = saturate(-0.5 + noiseIQ(fixed3(i.uv.x * 200 + _Time.x * 5234, i.uv.y * 200 + _Time.y * 8123, 1)));
				snow += saturate(-0.5 + noiseIQ(fixed3(i.uv.x * 34.45 + _Time.x * 23.42, i.uv.y * 34.45 + _Time.y * 5654.342, 1)));
				snow += saturate(-0.2 + noiseIQ(fixed3(i.uv.x * 150 + _Time.x * 23.42, i.uv.y * 150 + _Time.y * 5654.342, 1)));
				//noise2 += saturate(rand(_Time.yzw));

				//whitenoise *= lerp(fixed4(1, 1, 0, 1), fixed4(1, 0, 1, 1), rand(i.uv.x));

				c = lerp(c, c2, _ChannelBlend);
				c += (bigSpots + bigSpots2) * 2;
				return lerp(c, snow, _Intensity * lines);
			}
			ENDCG
		}
	}
}
