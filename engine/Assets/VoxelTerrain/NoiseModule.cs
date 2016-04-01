using UnityEngine;
using System.Collections;
using VoxelEngine;
[SerializeField]
public class NoiseModule:MonoBehaviour {
	// Use this for initialization
	public float Frequency;
	public float CaveFrequency;
	public float GroundFrequency;
	public int oct;
	public int caveOct;
	public int GroundOct;
	public float amplitude;
	public float caveAmplitude;
	public float GroundAmplitude;
	public bool check;

	public float SampleMountains(float x,float z, PerlinNoise perlin)
	{
		float w = perlin.FractalNoise2D(x , z ,oct,Frequency,amplitude);
		//This creates the noise used for the mountains. It used something called 
		//domain warping. Domain warping is basically offseting the position used for the noise by
		//another noise value. It tends to create a warped effect that looks nice.
		//Clamp noise to 0 so mountains only occur where there is a positive value
		//The last value (32.0f) is the amp that defines (roughly) the maximum mountaion height
		//Change this to create high/lower mountains
		
		return Mathf.Min(0.0f, perlin.FractalNoise2D(x +w, z +w,oct,Frequency,amplitude) );
	}
	
	public float SampleGround(float x,float z, PerlinNoise perlin)
	{
		//This creates the noise used for the ground.
		//The last value (8.0f) is the amp that defines (roughly) the maximum 
		float w = perlin.FractalNoise2D(x , z ,1,GroundFrequency,GroundAmplitude);
		return perlin.FractalNoise2D(x-w, z-w,GroundOct,GroundFrequency,GroundAmplitude);
	}
	//not cave noise just normal noise now as it needed a noise with another seed
	public float SampleCaves(float x, float z, PerlinNoise perlin)
	{
		float w = perlin.FractalNoise2D(x , z ,1,CaveFrequency,caveAmplitude);
		//larger caves (A higher frequency will also create larger caves). It is unitless, 1 != 1m
		return Mathf.Abs(perlin.FractalNoise2D(x+w, z+w,caveOct,CaveFrequency,caveAmplitude));
		
	}
	//sample caves using simplex noise
	public float SampleCavesreal(float x,float y, float z)
	{
		//The creates the noise used for the caves. It uses domain warping like the moiuntains
		
		//to creat long twisting caves.
		
		//The last vaule is the cave amp and defines the maximum cave diameter. A larger value will create
		
		float w = SimplexNoise.Noise.Generate(x /180,y/180, z/180);
		
		//larger caves (A higher frequency will also create larger caves). It is unitless, 1 != 1m
		
		return SimplexNoise.Noise.Generate(x/850+w,y/150+w, z/750+w)*32;
		
	}

	public float FillVoxel2d(float x,float z,PerlinNoise noise)
	{
		//float startTime = Time.realtimeSinceStartup;
		
		//Creates the data the mesh is created form. Fills m_voxels with values between -1 and 1 where
		//-1 is a soild voxel and 1 is a empty voxel.

		float HT = 0;

		HT  += SampleGround(x,z,noise);
		HT  += SampleMountains(x,z,noise);
		if(check)
			Debug.Log(HT);

					


		
		return HT;
		
		
	}

	public float FillVoxel3d(float x,float y,float z)
	{
		//float startTime = Time.realtimeSinceStartup;
		
		//Creates the data the mesh is created form. Fills m_voxels with values between -1 and 1 where
		//-1 is a soild voxel and 1 is a empty voxel.
		
		float HT = 0;


			HT = SampleCavesreal(x,y,z);

		
		
		return HT;
		
		
	}
}
