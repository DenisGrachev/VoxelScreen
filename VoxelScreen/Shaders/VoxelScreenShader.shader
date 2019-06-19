//Tiny vertex color shader.

Shader "VoxelScreen/VoxelScreenShader"
{
   
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        
        struct Input
        {
            float4 vertColor;
        };

        
        void vert(inout appdata_full v,out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.vertColor = v.color;
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = IN.vertColor.rgb;            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
