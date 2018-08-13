Shader "Custom/Blur"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_Radius("Radius", Float) = 1
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		LOD 200

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		// Blend SrcAlpha One // Additive
		Blend One OneMinusSrcAlpha // Alpha blend
		//AlphaTest Greater .01
			
		GrabPass { }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 grabPos  : TEXCOORD1;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Radius;

			fixed4 frag(v2f IN) : COLOR
			{
				float mask = tex2D(_MainTex, IN.texcoord).a * IN.color.a;

				float dx = _Radius * _GrabTexture_TexelSize.x;
				float dy = _Radius * _GrabTexture_TexelSize.y;
				const int lod = 0;

				half4 pixelCol = half4(0, 0, 0, 0);

				#define ADDPIXEL(weight,kernelX) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.grabPos.x + dx * kernelX, IN.grabPos.y, IN.grabPos.z, IN.grabPos.w))) * weight

				pixelCol += ADDPIXEL(0.05, 4.0);
				pixelCol += ADDPIXEL(0.09, 3.0);
				pixelCol += ADDPIXEL(0.12, 2.0);
				pixelCol += ADDPIXEL(0.15, 1.0);
				pixelCol += ADDPIXEL(0.18, 0.0);
				pixelCol += ADDPIXEL(0.15, -1.0);
				pixelCol += ADDPIXEL(0.12, -2.0);
				pixelCol += ADDPIXEL(0.09, -3.0);
				pixelCol += ADDPIXEL(0.05, -4.0);

				pixelCol.rgb *= IN.color.rgb;
				return saturate(pixelCol * mask);
			}
			ENDCG
		}

		GrabPass{}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 grabPos  : TEXCOORD1;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Radius;

			fixed4 frag(v2f IN) : COLOR
			{
				float mask = tex2D(_MainTex, IN.texcoord).a * IN.color.a;

				float dx = _Radius * _GrabTexture_TexelSize.x;
				float dy = _Radius * _GrabTexture_TexelSize.y;
				const int lod = 0;

				half4 pixelCol = half4(0, 0, 0, 0);

				#define ADDPIXEL(weight,kernelX) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.grabPos.x, IN.grabPos.y + dy * kernelX, IN.grabPos.z, IN.grabPos.w))) * weight

				pixelCol += ADDPIXEL(0.05, 4.0);
				pixelCol += ADDPIXEL(0.09, 3.0);
				pixelCol += ADDPIXEL(0.12, 2.0);
				pixelCol += ADDPIXEL(0.15, 1.0);
				pixelCol += ADDPIXEL(0.18, 0.0);
				pixelCol += ADDPIXEL(0.15, -1.0);
				pixelCol += ADDPIXEL(0.12, -2.0);
				pixelCol += ADDPIXEL(0.09, -3.0);
				pixelCol += ADDPIXEL(0.05, -4.0);

				pixelCol.rgb *= IN.color.rgb;
				return saturate(pixelCol * mask);
			}
			ENDCG
		}

		GrabPass { }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 grabPos  : TEXCOORD1;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;				
				OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);					
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Radius;

			fixed4 frag(v2f IN) : COLOR
			{
				float mask = tex2D(_MainTex, IN.texcoord).a * IN.color.a;

				float dx = _Radius * _GrabTexture_TexelSize.x;
				float dy = _Radius * _GrabTexture_TexelSize.y;
				const int lod = 0;
				
				half4 pixelCol = half4(0, 0, 0, 0);

				#define ADDPIXEL(weight,kernelX) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.grabPos.x + dx * kernelX, IN.grabPos.y, IN.grabPos.z, IN.grabPos.w))) * weight

				pixelCol += ADDPIXEL(0.05, 4.0);
				pixelCol += ADDPIXEL(0.09, 3.0);
				pixelCol += ADDPIXEL(0.12, 2.0);
				pixelCol += ADDPIXEL(0.15, 1.0);
				pixelCol += ADDPIXEL(0.18, 0.0);
				pixelCol += ADDPIXEL(0.15, -1.0);
				pixelCol += ADDPIXEL(0.12, -2.0);
				pixelCol += ADDPIXEL(0.09, -3.0);
				pixelCol += ADDPIXEL(0.05, -4.0);

				pixelCol.rgb *= IN.color.rgb;
				return saturate(pixelCol * mask);
			}
			ENDCG
		}

		GrabPass{}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 grabPos  : TEXCOORD1;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Radius;

			fixed4 frag(v2f IN) : COLOR
			{				
				float mask = tex2D(_MainTex, IN.texcoord).a * IN.color.a;

				float dx = _Radius * _GrabTexture_TexelSize.x;
				float dy = _Radius * _GrabTexture_TexelSize.y;
				const int lod = 0;

				half4 pixelCol = half4(0, 0, 0, 0);

				#define ADDPIXEL(weight,kernelX) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.grabPos.x, IN.grabPos.y + dy * kernelX, IN.grabPos.z, IN.grabPos.w))) * weight

				pixelCol += ADDPIXEL(0.05, 4.0);
				pixelCol += ADDPIXEL(0.09, 3.0);
				pixelCol += ADDPIXEL(0.12, 2.0);
				pixelCol += ADDPIXEL(0.15, 1.0);
				pixelCol += ADDPIXEL(0.18, 0.0);
				pixelCol += ADDPIXEL(0.15, -1.0);
				pixelCol += ADDPIXEL(0.12, -2.0);
				pixelCol += ADDPIXEL(0.09, -3.0);
				pixelCol += ADDPIXEL(0.05, -4.0);

				pixelCol.rgb *= IN.color.rgb;
				return saturate(pixelCol * mask);
			}
			ENDCG
		}


	}
}
