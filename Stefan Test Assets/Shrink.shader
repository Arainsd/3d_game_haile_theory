Shader "Custom/Shrink"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,1)
		_Thickness("Thickness", Range(0, 2)) = 0.1
		_Speed("Speed", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass // Draw a slightly larger version of the object with our outline color.
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float _Thickness;
			uniform float4 _Color;
			uniform float _Speed;
			
			struct v2f
			{
				float4 pos : POSITION;
				float4 vertex : POSITION2;
				float4 color : COLOR;
			};

			v2f vert(appdata_base v)
			{
				v2f o;

				o.vertex = v.vertex;
				o.pos = mul(UNITY_MATRIX_MVP, o.vertex); // Calculate vertex position.
				o.color = float4(0, 0, 0, 0);

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float time = 1 - ((_Time.y * _Speed) % 1);
				float4 pos = i.vertex;

				float min = time;
				float max = (time + _Thickness);
				float value = pos.y % 1;

				if (min > 1)
				{
					min -= 1;
					max -= 1;
				}

				if (max > 1)
				{
					max -= 1;

					if (value > min || value < max)
					{
						return _Color;
					}
				}
				else
				{
					if (value > min && value < max)
					{
						return _Color;
					}
				}

				return i.color;
			}

			ENDCG
		}
	}

	Fallback Off
}