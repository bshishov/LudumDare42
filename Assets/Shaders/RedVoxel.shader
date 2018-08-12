Shader "Custom/RedVoxel"
{
	Properties { }
	SubShader
	{
		Tags 
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent+1"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Tags {}
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			
			Stencil
			{
				Ref 64
				Comp Equal
				//Pass keep
				//Pass IncrSat
				Pass DecrSat
			}			

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
				float t = i.vertex.x + i.vertex.y;
				t = sin(t * 0.5 + _Time.w * 5);
				t *= t;				
				return t * fixed4(1, 0, 0, 0.6) + (1 - t) * fixed4(0, 0, 0, 0.6);
			}
			ENDCG
		}	


		Pass
		{
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha

			Stencil
			{
				Ref 64
				Comp Equal
				Pass keep
				//Pass IncrSat
				//Pass DecrSat
			}

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
				float t = i.vertex.x + i.vertex.y;
				t = sin(t * 0.5 + _Time.w * 5);
				t *= t;
				return t * fixed4(1, 0, 0, 0.2) + (1 - t) * fixed4(0, 0, 0, 0.2);
			}
			ENDCG
		}
	}
}
