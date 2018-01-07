Shader "Sprites/DefaultZ"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Glow ("Intensity", Range(0, 3)) = 1
	}

	CustomEditor "CustomShaderGUI"

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite On
		Fog{ Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#include "UnityCG.cginc"
			
			struct VertexData
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 vertex    : SV_POSITION;
				fixed4 color     : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			Interpolators vert(VertexData v)
			{
				Interpolators i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.texcoord = v.texcoord;
				i.color = v.color * _Color;

				#ifdef PIXELSNAP_ON
				i.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return i;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			half 	  _Glow;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(Interpolators i) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (i.texcoord) * i.color;

				float alpha = c.a;
				clip(alpha - 0.1f); // cutout
				c.rgb *= alpha;
				c *= _Glow;

				return c;
			}
		ENDCG
		}
	}
}
