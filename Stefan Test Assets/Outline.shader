Shader "Custom/Outline"
{
	Properties
	{
		_OutlineColor("Color", Color) = (0,0,0,1)
		_Thickness("Thickness", Float) = 0.1

		[Toggle] 
		_Enabled("Enabled", Float) = 0
	}

	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent" 
		}

		Pass // Draw a slightly larger version of the object with our outline color.
		{
			Cull Front
			ZWrite On

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float _Thickness;
			uniform float4 _OutlineColor;
			uniform float _Enabled;
			
			struct v2f
			{
				float4 pos : POSITION;
				float4 color : COLOR;
			};

			v2f vert(appdata_base v)
			{
				v2f o;

				if (_Enabled == 1)
				{
					float4 pos = mul(UNITY_MATRIX_MVP, v.vertex);
					float4 normal = mul(UNITY_MATRIX_MVP, v.normal); // The normal, we'll use this to calculate the direction we have to move the vertex in.

					pos += normal * _Thickness; // Extend the position outwards along the normal.

					o.pos = pos; // Calculate vertex position.
					o.color = _OutlineColor; // Set color to outline color.
				}
				else
				{
					o.pos = v.vertex + float4(1000, 1000, 1000, 1000); // Render it somewhere else. (Which in turn will make it not be rendered at all)
					o.color = float4(0, 0, 0, 0); // Just render it transparant.
				}

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				return i.color;
			}

			ENDCG
		}
	}

	Fallback Off
}