using UnityEngine;
using System.Collections;
namespace VoxelEngine{
public static class MeshFactory  {
	public static VoxelTerrainEngine generator;
	public static Vector3[,,] m_normals;
	public static Vector3[,,] m_normals2;
	public static PerlinNoise SurfacePerlin;
	public static MarchingCubes MarchingCubes;
	public static bool MakeCaves;
	public static Vector3[] Createvertices(byte[,,] m_voxels,VoxelChunk chunk,int lod){

		Vector3[] vertices = MarchingCubes.CreateVertices(m_voxels,chunk,2,2,lod);
		return vertices;

	}



		//calculate normals for the mesh 
		public static Vector3[] CalculateNormals(byte[,,] m_voxels ,int size,Vector3 [] verts,int lod)
	{	

		Vector3[] normals = new Vector3[size];

		int w = m_voxels.GetLength(0);

		int h= m_voxels.GetLength(1);

		int l = m_voxels.GetLength(2);

		//This calculates the normal of each voxel. If you have a 3d array of data
		//the normal is the derivitive of the x, y and z axis.
		//Normally you need to flip the normal (*-1) but it is not needed in this case.
		//If you dont call this function the normals that Unity generates for a mesh are used.
		
		
		
		 m_normals = new Vector3[w,h,l];
		
	for(int x = 2; x < w-2; x++)
		{
	for(int y = 2; y < h-2; y++)
		{
	for(int z = 2; z < l-2; z++)
		{
		float dx = m_voxels[x+1,y,z] - m_voxels[x-1,y,z];
		float dy = m_voxels[x,y+1,z] - m_voxels[x,y-1,z];
		float dz =m_voxels[x,y,z+1] - m_voxels[x,y,z-1];
					
		m_normals[x,y,z] = Vector3.Normalize(new Vector3(dx,dy,dz));
				}
			}
		}
		for(int i = 0;i < size ;i++){
		normals[i] = MeshFactory.TriLinearInterpNormal(verts[i]/VoxelTerrainEngine.TriSize[lod]);
		}
		
		return normals;
		
	}

		//interpolate normals so normals are smoothed

	public static Vector3 TriLinearInterpNormal(Vector3 pos)
	{			

		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;
		
		float fx = pos.x-x;
		float fy = pos.y-y;
		float fz = pos.z-z;
		
		Vector3 x0 = m_normals[x,y,z] * (1.0f-fx) + m_normals[x+1,y,z] * fx;
		Vector3 x1 = m_normals[x,y,z+1] * (1.0f-fx) + m_normals[x+1,y,z+1] * fx;
		
		Vector3 x2 = m_normals[x,y+1,z] * (1.0f-fx) + m_normals[x+1,y+1,z] * fx;
		Vector3 x3 = m_normals[x,y+1,z+1] * (1.0f-fx) + m_normals[x+1,y+1,z+1] * fx;
		
		Vector3 z0 = x0 * (1.0f-fz) + x1 * fz;
		Vector3 z1 = x2 * (1.0f-fz) + x3 * fz;
		
		return z0 * (1.0f-fy) + z1 * fy;
	}

		public static void LinearInterpolate(byte [,,] m_voxels , byte[,,] m_Materials)
		{			

			int w = m_voxels.GetLength(0);
			int h = m_voxels.GetLength(1);
			int l = m_voxels.GetLength(2);
			int sample = 2;
			for(int x = 0; x < w; x+=sample)
			{

				for(int z = 0; z < l; z+=sample)
				{
					for(int y = 0; y < h; y+=sample)
					{
							for(int i = 1;i < sample;i+=2){
								if(x>=sample){
									m_voxels[x-i,y,z] = (byte)Mathf.Lerp((float)m_voxels[x-(i+1),y,z],(float)m_voxels[x,y,z],1f/sample);
								m_Materials[x-i,y,z]= m_Materials[x,y,z];
							}
								if(y>=sample){
									m_voxels[x,y-i,z] = (byte)Mathf.Lerp((float)m_voxels[x,y-(i+1),z],(float)m_voxels[x,y,z],1f/sample);	
								m_Materials[x,y-i,z]= m_Materials[x,y,z];
							}
								if(z>=sample){
									m_voxels[x,y,z-i] = (byte)Mathf.Lerp((float)m_voxels[x,y,z-(i+1)],(float)m_voxels[x,y,z],1f/sample);	
								m_Materials[x,y,z-i]= m_Materials[x,y,z];
							}
								if(x>=sample && z>=sample){
									m_voxels[x-i,y,z-i] = (byte)Mathf.Lerp((float)m_voxels[x-(i+1),y,z-(i+1)],(float)m_voxels[x,y,z],1f/sample);
								m_Materials[x-i,y,z-i]= m_Materials[x,y,z];
							}
								if(z>=sample&&y>=sample){
									m_voxels[x,y-i,z-i] = (byte)Mathf.Lerp((float)m_voxels[x,y-(i+1),z-(i+1)],(float)m_voxels[x,y,z],1f/sample);
								m_Materials[x,y-i,z-i]= m_Materials[x,y,z];
							}
								if(x>=sample&&y>=sample){
									m_voxels[x-i,y-i,z] = (byte)Mathf.Lerp((float)m_voxels[x-(i+1),y-(i+1),z],(float)m_voxels[x,y,z],1f/sample);
								m_Materials[x,y-i,z]= m_Materials[x,y,z];
							}
								if(x>=sample&&y>=sample&&z>=sample){
									m_voxels[x-i,y-i,z-i] = (byte)Mathf.Lerp((float)m_voxels[x-(i+1),y-(i+1),z-(i+1)],(float)m_voxels[x,y,z],1f/sample);
								m_Materials[x-i,y-i,z-i]= m_Materials[x,y,z];}
								if(y>=h-5&&m_voxels[x,y,z]<=127)m_voxels[x,y,z]=255;
								if(y<=5&&m_voxels[x,y,z]>=127)m_voxels[x,y,z]=0;}

					}
				}
			}
		}


	


		//function to create the voxel noises and caves etc.
		public static byte[,,]CreateVoxels(byte[,,] m_voxels,Vector3 m_pos,VoxelChunk chunk , int lod)
	{
		//float startTime = Time.realtimeSinceStartup;
		//Creates the data the mesh is created form. Fills m_voxels with values between -1 and 1 where
		//-1 is a soild voxel and 1 is a empty voxel.

			int w = m_voxels.GetLength(0);
			int h= m_voxels.GetLength(1);
		int l = m_voxels.GetLength(2);
		float worldX;
		float worldZ;
		float worldY;
				for(int x = 0; x < w; x++)
			{
	
					for(int z = 0; z < l; z++)
				{	
					for(int y = h/2; y < h; y++)
					{	m_voxels[x,y,z] = 255;
							
			}
		}
				
	
		
		
				}
			return m_voxels;
		}
		}
}
