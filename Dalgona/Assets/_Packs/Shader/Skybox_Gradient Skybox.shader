Shader "Skybox/Gradient Skybox" {
	Properties {
		_Color1 ("Color 1", Vector) = (1,1,1,0)
		_Color2 ("Color 2", Vector) = (1,1,1,0)
		_UpVector ("Up Vector", Vector) = (0,1,0,0)
		_Intensity ("Intensity", Float) = 1
		_Exponent ("Exponent", Float) = 1
		_UpVectorPitch ("Up Vector Pitch", Float) = 0
		_UpVectorYaw ("Up Vector Yaw", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	//CustomEditor "GradientSkyboxInspector"
}