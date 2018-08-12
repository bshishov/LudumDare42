Shader "Custom/RedVoxel"
{
	Properties { }
	SubShader
	{
		Tags 
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry+100"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			//ZTest Always
			Stencil
			{
				Ref 64
				Comp always
				Pass replace
			}
			ColorMask 0
		}
	}
}
