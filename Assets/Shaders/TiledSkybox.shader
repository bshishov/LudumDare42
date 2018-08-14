// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Tiled Skybox"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 0)		
		_Speed("Speed", Vector) = (1, 1, 0, 0)
		// The properties below are used in the custom inspector.
		_UpVectorPitch("Up Vector Pitch", float) = 0
		_UpVectorYaw("Up Vector Yaw", float) = 0
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata
	{
		float4 position : POSITION;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float3 texcoord : TEXCOORD0;
	};

	half4 _Color;
	

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	half2 _Speed;

	v2f vert(appdata v)
	{
		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord = v.texcoord;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex, i.position.xy * _MainTex_TexelSize.xy * 1 + _Time.x * _Speed.xy) * _Color;
		return c;		
	}

	ENDCG

	SubShader
	{
	Tags{ "RenderType" = "Background" "Queue" = "Background" }
		Pass
		{
			ZWrite Off
			Cull Off
			Fog{ Mode Off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	CustomEditor "GradientSkyboxInspector"
}