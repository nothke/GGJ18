Shader "Hidden/YIQTransposer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 RGB2YIQ(fixed4 c)
			{
				fixed4 yiq;

				yiq.r = 0.299900 * c.r + 0.587000 * c.g + 0.114000 * c.b;
				yiq.g = 0.595716 * c.r - 0.274453 * c.g - 0.321264 * c.b;
				yiq.b = 0.211456 * c.r - 0.522591 * c.g + 0.311350 * c.b;

				return yiq;
			}

			fixed4 YIQ2RGB(fixed4 c)
			{
				fixed4 rgb;

				rgb.r = c.r + 0.9563 * c.g + 0.6210 * c.b;
				rgb.g = c.r - 0.2721 * c.g - 0.6474 * c.b;
				rgb.b = c.r - 1.1070 * c.g + 1.7046 * c.b;

				return rgb;
			}

			float rand(float2 co) {
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

			sampler2D _MainTex;
			float _Distance;

			float _SourceIntensity;
			float _Intensity;


			uniform float _YCurve[10];
			uniform float _IQCurve[10];

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
			
			fixed4 c2 = 0; // added this

			//fixed4 whitenoise = rand(round(rand(i.uv.y * _Time.z * 1)  * 3 + i.uv.x * 20)) * round(frac(_Time.z * 0.1 + rand(i.uv.y * _Time.x) * 3)) ;
			//fixed4 whitenoise = -0.5 + round(frac(_Time + i.uv.x * 20) * rand(i.uv.y));
			
			//fixed hLine = rand(round(rand(i.uv.y * _Time.z * 1) * 3 + i.uv.x * 20));
			//fixed4 whitenoise = min(hLine, round(-0.495 + rand(i.uv.y * _Time)));
			//c += whitenoise * 10;

			//c *= rand(1 + _SinTime * i.uv);

			fixed distance = _Distance / 10;

			for (int p = 0; p < 10; p++)
			{
				half random = rand(_SinTime * i.uv.y * 50) * 0.3;
				//random = 0;
				fixed4 pix = RGB2YIQ(tex2D(_MainTex, float2(i.uv.x - distance * p + distance * 3 * random, i.uv.y)));

				pix.r *= _YCurve[p];
				pix.g *= _IQCurve[p];
				pix.b *= _IQCurve[p];

				c2 += pix * 0.1;
			}

			//fixed4 cOffset = RGB2YIQ(tex2D(_MainTex, float2(i.uv.x + 0.003, i.uv.y)));

			//cOffset.g = 0;

			c = RGB2YIQ(c);

			

			c = YIQ2RGB(c * _SourceIntensity + c2 * _Intensity) * 0.5;
			c.a = 1;

				return c;
			}
			ENDCG
		}
	}
}
