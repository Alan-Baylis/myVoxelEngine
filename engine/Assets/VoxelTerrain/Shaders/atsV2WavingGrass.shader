Shader "Details/WavingDoublePass" {
Properties {
	_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_MainTexNormal ("Normal (RGB) Alpha (A)", 2D) = "white" {}
	//_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
_Cutoff ("Alphacutoff" , Range( 0 ,1 ))= 0.5
_normalInten ("Normal Intensity" , Range( -1 ,1 ))= 0.5

}

SubShader {
	Tags {
	
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="Grass"
	}
	Cull off
	LOD 200
	ColorMask RGB
		
CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
#pragma surface surf Standard vertex:vert addshadow
//#include "TerrainEngine.cginc"
		half _Shininess;

sampler2D _MainTex,_MainTexNormal;
float _Cutoff;
float windSpeed;
float WaveSize;
float WindAmount;
float SqrDistance;
float4 _WavingTint;
float WindDirectionx;
float WindDirectiony;
float WindDirectionz;
half _normalInten;
float3 distort;


float3 pos;
uniform float4 _Obstacle;
uniform float4 _Obstacle2;
uniform float4 _Obstacle3;



struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float4 color : COLOR;
	float3 lightDir ;
};

void FastSinCos (float4 val, out float4 s, out float4 c) {
	val = val * 6.408849 - 3.1415927;
	// powers for taylor series
	float4 r5 = val * val;
	float4 r6 = r5 * r5;
	float4 r7 = r6 * r5;
	float4 r8 = r6 * r5;
	float4 r1 = r5 * val;
	float4 r2 = r1 * r5;
	float4 r3 = r2 * r5;
	//Vectors for taylor's series expansion of sin and cos
	float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
	float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587};
	// sin
	s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
	// cos
	c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}
void vert (inout appdata_full v) {
	
////////// start bending
	
	float4 _waveXSizeMove = float4(0.048, 0.06, 0.24, 0.096);
	float4 _waveYSizeMove = float4 (0.036, 0.2, -0.07, 0.059);
	float4 _waveZSizeMove = float4 (0.024, .08, 0.08, 0.2);
	float4 waveSpeed = float4 (2, 2, 2, 2);
	
	// OBSTACLE AVOIDANCE CALC
	float3 worldPos = mul((float3x4)_Object2World,v.vertex);
float3 bendDir = normalize(float3(worldPos.x,worldPos.y,worldPos.z)- float3(_Obstacle.x,_Obstacle.y,_Obstacle.z));//direction of obstacle bend

float distLimit = 0.35;// how far awaydoes obstacle reach

float distMulti = (distLimit-min(distLimit,distance(float3(worldPos.x,worldPos.y,worldPos.z),float3(_Obstacle.x,_Obstacle.y,_Obstacle.z)))); //distance falloff

	float4 waves;
	waves = worldPos.x * _waveXSizeMove;
	waves += worldPos.z * _waveZSizeMove;
	waves += worldPos.y * _waveYSizeMove;
	_waveXSizeMove = float4(0.024, 0.04, -0.12, 0.096);
	_waveYSizeMove = float4 (0.016, 0.1, -0.05, 0.09);
	_waveZSizeMove = float4 (0.006, .02, -0.02, 0.1);
	
	// Add in time to model them over time
	float origLength = length(v.vertex.xyz);
	waves += _Time.x *(1+ windSpeed * 8 ) *  waveSpeed;
	float4 s, c;
	waves = frac (waves);
	FastSinCos (waves, s,c);
	float waveAmount = v.texcoord.y *(v.color.a) * WindAmount;

	// Faster winds move the grass more than slow winds 
	s *= normalize (waveSpeed);
	s *= s;
	
	float lighting = dot (s, normalize (float4 (1,1,.4,.2))) * .7;
	s *= waveAmount;
	
	fixed3 waveColor = lerp (fixed3(0.5,0.5,0.5), _WavingTint.rgb, lighting);
	
	v.color.rgb = v.color.rgb * waveColor * 2;
	
	float3 waveMove = float3 (0,0,0);
	distort = v.vertex;
	waveMove.x = dot (s, _waveXSizeMove*WindDirectionx);
	waveMove.y = dot (s, _waveYSizeMove*WindDirectiony);
	waveMove.z = dot (s, _waveZSizeMove*WindDirectionz);


	distort.xyz += mul ((float3x3)_World2Object, waveMove).xyz;
	distort.y += float3(0,5,0)* bendDir.y*distMulti*v.normal.y*8*(v.color.a); 
	distort.xz += bendDir.xz*distMulti*v.normal.xz*2*(v.color.a);

		
	if(distance(distort,v.vertex)<1.0f)
	v.vertex.xyz = distort.xyz;
	
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	v.vertex.xyz = normalize(v.vertex.xyz) * origLength;
	
//OBSTACLE AVOIDANCE END

//ADD OBSTACLE BENDING
	
////////// end bending
}




void surf (Input IN, inout SurfaceOutputStandard o) {
	half4 c = tex2D(_MainTex, IN.uv_MainTex);
	// add terrain lighting
	o.Albedo = c.rgb * _WavingTint.rgb;
	o.Alpha = c.a;
	o.Smoothness = _Shininess;

	o.Normal = UnpackNormal(tex2D(_MainTexNormal, IN.uv_MainTex)*_normalInten);
	clip (o.Alpha - _Cutoff);
}
ENDCG
}
	
	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"
		}
		Cull off
		LOD 200
		ColorMask RGB
		
		Pass {
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			Lighting On
			ColorMaterial AmbientAndDiffuse
			AlphaTest Greater [_Cutoff]
			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		}
	}
	
	Fallback Off
}
