// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FurnitureShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Active("Active", Float) = 0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{		
			/*
		Pass 
		{
			Tags{ "RenderType" = "Transparent" "Queue"="Transparent" }
			Stencil
			{
				Ref 64
				Comp Always
				Pass Replace
			}

			ZTest Always
			ZWrite On

			CGPROGRAM			
			#pragma vertex vert						
			#pragma fragment frag

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
			
			float _Active;
			sampler2D _MainTex;			

			fixed4 frag(v2f i) : SV_Target
			{
				clip(_Active - 0.1);
				
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * fixed4(_Active, 0, 0, 1);
			}
			ENDCG 
		}	
			*/

		Tags{ "RenderType" = "Opaque" }
		LOD 200
					
		Stencil
		{
			Ref 64
			Comp Always
			Pass Replace
		}

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

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;			
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG		
	}
	FallBack "Diffuse"
}
