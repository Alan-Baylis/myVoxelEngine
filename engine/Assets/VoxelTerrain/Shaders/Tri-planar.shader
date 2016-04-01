 
// ----------------------------------------------------------------------------------------------------------------
// Shader:    TriPlanarTan_BumpedSpecular - Surface Shader
// Author:    NoiseCrime
// Date:    11.07.11
// Unity:    3.3
// ----------------------------------------------------------------------------------------------------------------
 
 
// SUMMARY
// ----------------------------------------------------------------------------------------------------------------
// A TriPlanar blinn-Phong, based surface shader that exposes only basic material properties.
// Support for 3 textures (one for each major axis) and counterpart normalmaps.
// ----------------------------------------------------------------------------------------------------------------
 
 
// REQUIREMENTS
// ----------------------------------------------------------------------------------------------------------------
// ShaderModel 3.0 - only by a few instructions!!
// Packed normal maps - i.e texture is set to 'normalmap' in inspector. (Unity 3)
// Model with tangents.
// ----------------------------------------------------------------------------------------------------------------
 
 
// NOTES
// ----------------------------------------------------------------------------------------------------------------
// Add 'nolightmap' to remove uv2 requirement.
// Can be set up for local or world space mapping - See Vertex Definition.
// ----------------------------------------------------------------------------------------------------------------
 
 
Shader "ncp/procedural/TriPlanarTan_BumpedSpecular"
{
    Properties
    {
        _Color                 ("Main Color",         Color)                 = (1,1,1,0.5)
        _SpecColor             ("Specular Color",     Color)                 = (1.0, 1.0, 1.0, 1.0)
        _Shininess             ("Shininess",         Range (0.00, 1.0))     = 0.078125
        _NormalScale             ("Normal Intensitiy",         Range (0.0, 8.0))     = 0.078125
         _Metalic            ("Metalness",         Range (0.03, 1.0))     = 0.078125

         _blendscale           ("blendscale",         Range (0.00, 4.0))     = 0.078125
        _BlendPlateau1         ("BlendPlateau",     Range (0.0, 1.0))     = 0.2
        _BlendPlateau2         ("BlendPlateau1",     Range (0.0, 1.0))     = 0.2
         _BlendPlateau3         ("BlendPlateau2",     Range (0.0, 1.0))     = 0.2
         
        _MainTex             ("Base 1 (RGB) Gloss(A)", 2D)             = "white" {}
        _BumpMap1             ("NormalMap 1 (_Y_X)", 2D)                 = "bump" {}
         _TexScale             ("Tex Scale",         Range (0.1, 10.0))     = 1.0
        _MainTex2             ("Base 2 (RGB) Gloss(A)", 2D)             = "white" {}
         _BumpMap2             ("NormalMap 2 (_Y_X)", 2D)                 = "bump" {}
         _TexScale2             ("Tex Scale",         Range (0.1, 10.0))     = 1.0
        _MainTex3             ("Base 3 (RGB) Gloss(A)", 2D)             = "white" {}
         _BumpMap3             ("NormalMap 3 (_Y_X)", 2D)                 = "bump" {}   
         _TexScale3             ("Tex Scale",         Range (0.1, 10.0))     = 1.0
       
        
       
        
    }
   
    Category
    {
        SubShader
        {
            ZWrite On            
            Tags { "RenderType"="Opaque" }
            LOD 400
 
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile_builtin        
                               
                #pragma surface surf Standard fullforwardshadows vertex:vertWorld
                           
                sampler2D     _MainTex;
                sampler2D     _MainTex2;
                sampler2D     _MainTex3;
                sampler2D     _BumpMap1;
                sampler2D     _BumpMap2;
                sampler2D     _BumpMap3;            
                fixed4         _Color;
                half 		 _blendscale;
                half         _Shininess;
                half         _Metalic;
                half         _NormalScale ;
                half         _TexScale;
                half         _TexScale2;
                half         _TexScale3;
                half         _BlendPlateau;
               half         _BlendPlateau1;
               half         _BlendPlateau2;
               half         _BlendPlateau3;
                struct Input
                {	float4 color : COLOR;
                    float3 thisPos;        
                    float3 thisNormal;    
                };                    
               
                // Vertex program is determined in pragma above
                void vertWorld (inout appdata_full v, out Input o)
                {
                    o.thisNormal     = mul(_Object2World, float4(v.normal, 0.0f)).xyz;
                    o.thisPos         = mul(_Object2World, v.vertex);
                    o.color           = v.color;
                }
 
                void vertLocal (inout appdata_full v, out Input o)
                {
                    o.thisNormal     = v.normal;
                    o.thisPos         = v.vertex;// * _TexScale;
                }        
 
 
                void surf (Input IN, inout SurfaceOutputStandard o)
                {                    
                    // Determine the blend weights for the 3 planar projections.    
                    half3 blend_weights = abs( IN.thisNormal.xyz );           // Tighten up the blending zone:
                   
                    blend_weights = (blend_weights +_blendscale);           // (blend_weights - 0.2) * 7; * 7 has no effect.
                    blend_weights = max(blend_weights, 1);                  // Force weights to sum to 1.0 (very important!)  
                    blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z ).xxx;  
                           
                    // Now determine a color value and bump vector for each of the 3  
                    // projections, blend them, and store blended results in these two vectors:  
                    half4 blended_color;     // .w hold spec value  Not true in this shader
                    half3 blended_bumpvec;
                             
                    // Compute the UV coords for each of the 3 planar projections.
                    // tex_scale (default ~ 1.0) determines how big the textures appear.  
                    half3 coord1 = IN.thisPos.yzx * _TexScale;  
                    half3 coord2 = IN.thisPos.zxy * _TexScale2;  
                    half3 coord3 = IN.thisPos.xyz * _TexScale3;  
 
                    // Sample color maps for each projection, at those UV coords.  
                    half4 col1         = tex2D(_MainTex, coord1);
                    half4 col2         = tex2D(_MainTex2, coord2);
                    half4 col3         = tex2D(_MainTex3, coord3);
 
                    // Sample bump maps too, and generate bump vectors. (Note: this uses an oversimplified tangent basis.)  
                    // Using Unity packed normals (_Y_X), but we don't unpack since we don't need z.                    
                    half3 bumpVec1    = tex2D(_BumpMap1, coord1).zxy * 2-1 ;      // To use standard normal maps change wy to xy
                    half3 bumpVec2    = tex2D(_BumpMap2, coord2).zxy * 2-1 ;      // To use standard normal maps change wy to xy
                    half3 bumpVec3    = tex2D(_BumpMap3, coord3).xyz * 2-1 ;        // To use standard normal maps change wy to xy
 
                     half3 bump1     = half3( bumpVec1.z,bumpVec1.x,bumpVec1.y );  
                    half3 bump2     =  half3( bumpVec2.z,bumpVec2.x,bumpVec2.y);   
                    half3 bump3     = half3(bumpVec3.x, bumpVec3.y, bumpVec2.z);
 
                    // Finally, blend the results of the 3 planar projections. 
                    float4 controlMap = IN.color; 
                    blended_color     =     col1.yzxw +  
                                        col2.zxyw  * controlMap.r +  
                                        col3.xyzw  * controlMap.b ; 
                     
                    blended_bumpvec =     bump1.xyz  +  
                                        bump2.xyz  * controlMap.r+  
                                        bump3.xyz * controlMap.b;  
                  	
                  	
                    half4 c         = blended_color.rgba * _Color.rgba;
                    o.Albedo         = c.rgb*4;
                    o.Metallic        = _Metalic;
                    o.Smoothness          = _Shininess;                    
                    o.Normal         = normalize( half3(0,0,2) + blended_bumpvec.xyz *- _NormalScale);        
                }
            ENDCG
        }
    }
    FallBack "Diffuse"
}