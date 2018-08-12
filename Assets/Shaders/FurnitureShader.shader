Shader "Custom/FurnitureShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Stencil
		{
			Ref 64
			Comp NotEqual
			Pass Keep
		}

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
		fixed4 _Color;
		
		UNITY_INSTANCING_BUFFER_START(Props)			
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;			
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		Pass 
		{
			Tags 
			{ 
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
				"IgnoreProjector" = "True" 
			}
			/*
			Stencil
			{
				Ref 64
				Comp Equal
				Pass Keep
			}*/
			//Blend Off
			Blend One One
			//Blend SrcAlpha OneMinusSrcAlpha
			ZTest Less
			ZWrite Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}			

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(1, 0, 0, 1);
			}
			ENDCG
		}
/*
		//Blend One One
		Lighting Off
		Cull Back
		Zwrite On
		//Ztest Always

		CGPROGRAM			
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			float f = (sin(_Time.w * 2) * 0.5 + 0.5);
			f = lerp(0.2, 0.8, f);

			o.Albedo = saturate(c.rgb * fixed3(1, f, f));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
		*/
	}
	FallBack "Diffuse"
}
