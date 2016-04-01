Shader "Custom/Terrain" 
{
	Properties 
	{	_TextureOne("Texture", 2D) = "white" {}
		_TextureOneN("Texture  Normal", 2D) = "white" {}
		_TextureTwo("Texture", 2D) = "white" {}
		_TextureTwoN("Texture Normal", 2D) = "white" {}
		_TextureThree("Texture", 2D) = "white" {}
		_TextureThreeN("Texture  Normal", 2D) = "white" {}
		_TextureFour("Texture", 2D) = "white" {}
		_TextureFourN("Texture  Normal", 2D) = "white" {}
		_Smoothness ("Smoothness" , Range( 0.0 ,1 ))= 0.5
		_Blending ("Blending" , Range( 0.0001 ,0.5 ))= 0.5

	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		//Cull off
				

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vertWorld
		#pragma target 3.0

		sampler2D _TextureOne, _TextureTwo, _TextureThree,_TextureFour , _TextureOneN, _TextureTwoN, _TextureThreeN,_TextureFourN;
		float _Blending,_Smoothness,_highlightSize;
		float3 _highlightArea;
		struct Input 
		{
			float3 thisPos;
			float4 color : COLOR;
			fixed3 thisNormal;
			//INTERNAL_DATA
		};
		 void vertWorld (inout appdata_full v, out Input o)
                {
               
                o.thisNormal = UnityObjectToWorldNormal( v.normal);
                    o.thisPos = mul(_Object2World, v.vertex);
                	o.color = v.color;
                    

                }
              

		fixed4 TriplanarSample(sampler2D tex, fixed3 worldPosition, fixed3 projNormal, float scale)
		{
			fixed4 cZY = tex2D(tex, worldPosition.zy * scale);
			fixed4 cXZ = tex2D(tex, worldPosition.xz * scale);
			fixed4 cXY = tex2D(tex, worldPosition.xy * scale);
			
			cXY = lerp(cXY, cXZ, projNormal.y);
			return lerp(cXY, cZY, projNormal.x);
		}
		
		
		
		float4 blend(float4 texture1, float a1, float4 texture2, float a2)
		{
	
    	float depth = 0.01f;
    	float ma = max(texture1.a + a1, texture2.a + a2) - depth;

    	float b1 = max(texture1.a + a1 - ma, 0);
    	float b2 = max(texture2.a + a2 - ma, 0);

    	return (texture1.rgba * b1 + texture2.rgba * b2) / (b1 + b2);
		}
		
		
		
		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			float3 projNormal = saturate(pow(IN.thisNormal * 1.5, 4));
			
			float4 sand = TriplanarSample(_TextureOne, IN.thisPos, projNormal, 1.0);
			
			float4 gravel = TriplanarSample(_TextureTwo, IN.thisPos, projNormal, 1.0);
			
			float4 rock = TriplanarSample(_TextureThree, IN.thisPos, projNormal, 0.1);
			
			float4 cliff = TriplanarSample(_TextureFour, IN.thisPos, projNormal, 1.0);
			
			float4 sandN = TriplanarSample(_TextureOneN, IN.thisPos, projNormal, 1.0);
			
			float4 gravelN = TriplanarSample(_TextureTwoN, IN.thisPos, projNormal, 1.0);
			
			float4 rockN = TriplanarSample(_TextureThreeN, IN.thisPos, projNormal, 0.1);
			
			float4 cliffN = TriplanarSample(_TextureFourN, IN.thisPos, projNormal, 1.0);
			
			float4 controlMap = IN.color;
			float4 col= 0;
			float4 colN= 0;
			
			
			if(controlMap.r <= 1.0){
			col = blend(col,controlMap.r, rock,controlMap.r);
			colN = blend(colN,controlMap.r, rockN,controlMap.r );}
			
			if(controlMap.g <= 1.0 ){
			col = blend(col,controlMap.g,sand , (controlMap.g));
			colN = blend(colN,controlMap.g,sandN , controlMap.g);}

			if(controlMap.x <= 1.0 ){
			col = blend(col,controlMap.b, gravel, (controlMap.b));
			colN = blend(colN,controlMap.b, gravelN, controlMap.b);}

			if(controlMap.a <= 1.0 ){
			col = blend(col,controlMap.a, cliff, (controlMap.a));
			colN = blend(colN,controlMap.a, cliffN, controlMap.a);}
			_highlightArea.y = IN.thisPos.y;
			if(distance(_highlightArea,IN.thisPos)<=_highlightSize){
			col+=float4(0,0,0.5f,0);
			}

			

			
			
			
			
						

			o.Metallic = 0;
            
			//o.Albedo = controlMap;  
			o.Albedo = col.rgb;
			o.Alpha = col.a;
			o.Normal = UnpackNormal(colN);
			o.Smoothness = _Smoothness;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
