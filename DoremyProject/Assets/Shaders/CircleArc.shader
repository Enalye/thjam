Shader "Custom/CircleArc" {
Properties {
		[PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
		[MaterialToggle]PixelSnap ("Pixel snap", Float) = 0
		_GradientTex("Gradient Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 1
	}

	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend One OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vertInput {
				float4 vertex : POSITION0;
				float4 color  : COLOR;
				float2 uv 	  : TEXCOORD0;
			};

			struct vertOutput {
				float4 vertex : POSITION;
				float4 color  : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _GradientTex;

			float _Cutoff;

			vertOutput vert (vertInput v) {
				vertOutput o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;

				return o;
			}

			fixed4 SampleSpriteTexture (float2 uv) {
				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag (vertOutput i) : COLOR {
				float2 textureCoord = UNITY_PROJ_COORD(i.uv);
				fixed4 col = SampleSpriteTexture (i.uv) * i.color;
				fixed4 gradientCol = tex2D(_GradientTex, i.uv);

        		float remove = (gradientCol.r < _Cutoff) || (_Cutoff == 1);
        		float keep = 1 - keep;

        		float alpha = col.a;
				clip(alpha - 0.1f); // cutout
				col.rgb *= alpha;

				return col * remove;
			}
			ENDCG
		}
	}
}
