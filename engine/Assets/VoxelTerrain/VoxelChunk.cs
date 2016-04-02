using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using VoxelEngine;
namespace VoxelEngine{
	public enum VoxelType{
		Dirt = 0,
		SandStone = 1,
		Stone = 2,
		Grass = 3,
		Iron = 4,
		Gold = 5,
		GunPowder = 6,
		Tungsten = 7,
		
	}
public class VoxelChunk {
	
	public List<Vector3>Pos= new List<Vector3>();
	public List<float>Rot= new List<float>();
	public List<Mesh>GrassMesh = new List<Mesh>();
	public List<Material>GrassMat = new List<Material>();
	public byte[,,]Voxels;
		public byte[,,]Materials;
	public int x;
	public int y;
	public int z;
	public Vector3 m_pos;
	public Vector3 RealPos;
	public GameObject m_mesh;
	public Vector3[] verts;
	public int[]tris;
	Vector3[] normals;
	public Mesh mesh;
	public Transform mytransform;
	public bool canCreatemesh;
	public bool canDraw;
	public bool shouldrender;
	public static Transform parent;
	public static VoxelTerrainEngine generator;
	public bool hascollider;
	public MeshCollider col;
	public MeshFilter meshfilter;
	public bool hasproccessed;
	public string FileName;
	public string SaveName;
	public SaverVoxel VoxelSaver;
	public bool HasChanged;
	public bool HasGrass;
	MeshRenderer meshrender;
	public int size;
	public List<Vector3>treelist = new List<Vector3>();
	public List<int>treeindex = new List<int>();
		public int lodLevel;
		public bool hasVoxels;
		public bool hasloaded;
		Color[] uv;

		public VoxelChunk(Vector3 pos, int width, int height,int lod)
	{			

			lodLevel = lod;
			//As we need some extra data to smooth the voxels and create the normals we need a extra 5 voxels
			//+1 one to create a seamless mesh. +2 to create smoothed normals and +2 to smooth the voxels
			//This is a little unoptimsed as it means some data is being generated that has alread been generated in other voxel chunks
			//but it is simpler as we dont need to access the data in the other voxels. You could try and copy the data
			//needed in for the other voxel chunks as a optimisation step

			//set voxel size and data
			Voxels = new byte[width+5, height+5, width+5];
			Materials = new byte[width+5, height+5, width+5];
			x = width+5;
			y = height+5;
			z = width+5;
			//set position so that voxel position matches world position if translated
			m_pos = pos - new Vector3(2f,2f,2f);
			m_pos *=VoxelTerrainEngine.TriSize[lodLevel];
			RealPos = pos;
			//set file name for voxel saver

			FileName = "VoxelChunk "+m_pos;

			SaveName = "Saved Chunks";

			VoxelSaver = new SaverVoxel(FileName+".txt",SaveName);

			VoxelSaver.fileName = SaveName + "/" + FileName+".txt";


	}

///method for rendering grass 
public void RenderGrass()
		{
		//if we have grass, render it , otherwise ignore
			for(int i = 0;i < Pos.Count;i++){
				if((generator.Target.position-Pos[i]).magnitude<=generator.DetailDistance){
					Graphics.DrawMesh(GrassMesh[i],Pos[i],Quaternion.Euler(0,Rot[i],0 ),GrassMat[i],generator.Parent.gameObject.layer);
			}
		}
		}
		/// <summary>
		/// allows you set the value where 0 is full and 255 is empty
		/// </summary>
		/// <param name="voxelpos">Voxelpos.</param>
		/// <param name="value">Value.</param>
		public bool SetVoxels(Vector3 voxelpos ,float value,float dist){

		if(mytransform!=null)
				voxelpos = mytransform.InverseTransformPoint(voxelpos)/VoxelTerrainEngine.TriSize[lodLevel];

			//rounds off the vector3s to nearest integer as voxels are integer values
			float X = voxelpos.x;

			float Y = voxelpos.y;

			float Z = voxelpos.z;


			for(int xx = 0; xx < (int)Mathf.Clamp(X+dist,0,x);xx++){
				for(int yy = 0; yy < (int)Mathf.Clamp(Y+dist,0,y);yy++){
					for(int zz =0; zz < (int)Mathf.Clamp(Z+dist,0,z);zz++){	

				
			//tell the engine to regenerate the terrain based on new voxel data
						if((new Vector3(xx,yy,zz)-new Vector3(X,Y,Z)).magnitude<dist && yy<y-5 && yy>5 && Mathf.Abs(yy-Y)>Mathf.Abs(value)){
							Voxels[xx,yy,zz] = (byte)Mathf.Clamp(Voxels[xx,yy,zz] + value,0,255);
							HasChanged=true;

			
			//send back to tell the engine its got a voxel at this coordinate
					
				}



			}
				}
			}
			if(HasChanged){
				return true;
			}
			//send back to tell the engine it hasnt got voxel at this coordinate
			return false;
		
	}
		/// <summary>
		/// allows you set the value where 0 is full and 255 is empty
		/// </summary>
		/// <param name="voxelpos">Voxelpos.</param>
		/// <param name="value">Value.</param>
		public bool SetMaterial(Vector3 voxelpos ,byte value,float dist){

			if(mytransform!=null)
				voxelpos = mytransform.InverseTransformPoint(voxelpos)/VoxelTerrainEngine.TriSize[lodLevel];

			//rounds off the vector3s to nearest integer as voxels are integer values
			float X = voxelpos.x;

			float Y = voxelpos.y;

			float Z = voxelpos.z;


			for(int xx = (byte)(X-dist); xx < x;xx++){
				for(int yy = (byte)(Y-dist); yy < y;yy++){
					for(int zz = (byte)(Z-dist); zz < z;zz++){	

						float totaldist = (new Vector2(xx,zz)-new Vector2(X,Z)).magnitude;
						//tell the engine to regenerate the terrain based on new voxel data
						if(totaldist<dist && yy<y-5 && yy>5){
							Materials[xx,yy,zz] = value;
							HasChanged=true;


							//send back to tell the engine its got a voxel at this coordinate

						}



					}
				}
			}
			if(HasChanged){
				return true;
			}
			//send back to tell the engine it hasnt got voxel at this coordinate
			return false;

		}

///this is to find the type of voxel thats there eg: iron , oil or gold
///can have up to 8 types which will be in shader
///still needs work id like to have separate array that holds the types in it but it uses to much memory
///still dont quite understand why it uses so much memory
public byte FindVoxelType(Vector3 VoxelPos,byte Type ){
		if(mytransform!=null)
				VoxelPos = mytransform.InverseTransformPoint(VoxelPos)/VoxelTerrainEngine.TriSize[lodLevel];
			
			int voxelpositionX = Mathf.RoundToInt(VoxelPos.x);
			int voxelpositionY = Mathf.RoundToInt(VoxelPos.y);
			int voxelpositionZ = Mathf.RoundToInt(VoxelPos.z);
			
			
		if(voxelpositionX<Voxels.GetLength(0) && voxelpositionY<Voxels.GetLength(1)-5
		   &&voxelpositionZ<Voxels.GetLength(2) && voxelpositionX>=0 && voxelpositionY>=5
		   && voxelpositionZ>=0){

			float t = (float)Voxels[voxelpositionX,voxelpositionY,voxelpositionZ]/255;

			Type = (byte)(t*8);

			return Type ;
				}
			else return 9;
			
		}
/// <summary>
/// Creates the voxels.
/// </summary>
public void CreateVoxels(){
			if(VoxelSaver.GetBool("hasSavedChunk")==false && hasVoxels==false){

				Voxels = MeshFactory.CreateVoxels(Voxels,RealPos-new Vector3(2,2,2),this,lodLevel);
				hasVoxels = true;
			}

			else if(VoxelSaver.GetBool("hasSavedChunk") && hasVoxels==false && hasloaded==false){
				LoadVoxels();
				hasVoxels = true;
				hasloaded = true;
			}
			VoxelTerrainEngine.GenerateVertices.Enqueue(this);
		}

//threaded mesh creation 
// basically just creates the triangles and vertices on a thread then adds them to a mesh
//in the main thread
/// <summary>
/// Creates the vertices.
/// </summary>
public void CreateVertices(){

			canCreatemesh = false;

			//create the verts
			verts = MarchingCubes.CreateVertices(Voxels,this,2,2,lodLevel);

			//store the size so as to avoid garbage creation
			size = verts.Length;

			//create normals
			normals = MeshFactory.CalculateNormals(Voxels,size,verts,lodLevel);

			//create colors

			int V = size;
			uv = new Color[size];
			for(int i = 0; i < size; i++)
			{

			int x = Mathf.RoundToInt((verts[i].x-normals[i].x)/VoxelTerrainEngine.TriSize[lodLevel]);
			int y = Mathf.RoundToInt((verts[i].y-normals[i].y)/VoxelTerrainEngine.TriSize[lodLevel]);
			int z = Mathf.RoundToInt((verts[i].z-normals[i].z)/VoxelTerrainEngine.TriSize[lodLevel]);

			//conversion from voxel value to voxel type
				//seems to work well
				byte vox = Materials[x,y,z];

			//basically each value gets assigned a color of 0.5 to 1.0 
				//in theory each decimal could be a voxel type
				if(vox==(int)VoxelType.Stone){
					
					uv[i] = new Color(1,0,0,0 );}
				
				else if(vox==(int)VoxelType.Grass ){
					
					uv[i] = new Color(0,1,0,0 );}
				
				else if(vox==(int)VoxelType.SandStone){
				
					uv[i] = new Color(0,0,1,0);}
				
				else if(vox==(int)VoxelType.Dirt){

					uv[i] = new Color(0,0,0,1 );}
			}
			canCreatemesh = true;
			VoxelTerrainEngine.MeshChunks.Enqueue(this);

	}

		//save voxels this all works perfectly i dont think it needs much explaining only thing that could 
		//be done is some sort of compression although i couldnt find any compression methods in unity
	public void SaveVoxels(){
			//flatten array of 3d voxels

			byte []voxel = new byte[Voxels.GetLength(0)*Voxels.GetLength(1)*Voxels.GetLength(2)];

		for(int x =0; x < Voxels.GetLength(0);x++){

		for(int y =0; y < Voxels.GetLength(1);y++){

		for(int z =0; z < Voxels.GetLength(2);z++){

			voxel[x + y*Voxels.GetLength(0)+z*Voxels.GetLength(0)*Voxels.GetLength(1)] = Voxels[x,y,z];

					}
				}
			}	
			//if no directory exists create one
		if(!Directory.Exists(SaveName + "/"+FileName))

			Directory.CreateDirectory(SaveName + "/"+FileName);

			File.WriteAllBytes(SaveName + "/"+FileName + "/" +FileName+".dat", voxel );

			VoxelSaver.SetBool("hasSavedChunk",true);

			VoxelSaver.SetFloat("TreeCount",treelist.Count);

		for(int i = 0;i < treelist.Count;i++){

			VoxelSaver.SetFloat(" index "+i,treeindex[i]);

			VoxelSaver.SetFloat("x"+i,treelist[i].x);

			VoxelSaver.SetFloat("y"+i,treelist[i].y);

			VoxelSaver.SetFloat("z"+i,treelist[i].z);

			}

			VoxelSaver.Flush();

			VoxelSaver.DeleteAll();
	}
			//load the voxels if directory exists

	public void LoadVoxels(){
			byte [] values = File.ReadAllBytes(SaveName + "/"+FileName + "/" +FileName+".dat");



		for(int x =0; x < Voxels.GetLength(0);x++){

		for(int y =0; y < Voxels.GetLength(1);y++){

		for(int z =0; z < Voxels.GetLength(2);z++){

			Voxels[x,y,z] = values[x + y*Voxels.GetLength(0)+z*Voxels.GetLength(0)*Voxels.GetLength(1)];

					}
				}
			}

	
		
	}
		//main thread mesh assigning 
		//assigns meshes vertices , triangles and colors to mesh 
		//as well as adds the plant placer script which plants the trees and grass and rocks on the mesh
	public void CreateMesh( )
		{	
			if(canCreatemesh){
		if(mesh==null)
			mesh = new Mesh();
		else if (mesh!=null)
			mesh.Clear();

			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.normals = normals;
			mesh.colors = uv;

			size = 0;
		if(verts!=null)
			size = verts.Length;

		if(size>0)
			shouldrender=true;

		else 
			shouldrender=false;

		if(size>0){
			//store in colors 

			//mark mesh as dynamic not sure how well this works?
			mesh.MarkDynamic();
				mesh.Optimize();
		if(m_mesh==null)
			m_mesh = new GameObject("Voxel Mesh " + m_pos + "Lod = " + lodLevel);
			m_mesh.tag = parent.tag;
			m_mesh.layer = parent.gameObject.layer;
			m_mesh.isStatic = parent.gameObject.isStatic;
			m_mesh.transform.parent = parent;
			m_mesh.transform.localPosition = m_pos;

			m_mesh.transform.localPosition = new Vector3(m_mesh.transform.localPosition.x,0,m_mesh.transform.localPosition.z);
			m_mesh.transform.localScale = Vector3.one;
			mytransform = m_mesh.transform;

		if(col!=null ){
			UnityEngine.Object.DestroyImmediate(col);
				}
		if(size>0){
			col = m_mesh.AddComponent<MeshCollider>();
					col.sharedMesh = mesh;}

		if(HasGrass ==false){
			
			//add plant placer component to mesh object 
			plantPlacer placer = m_mesh.AddComponent<plantPlacer>();
			//assign vertices for plant place to use to know where to place the plants and trees
			placer.Vertices = verts;

			//assign colors to plant placer supposed to be used to only place grass on grass area but
			//i couldnt get it to work
			//placer.control = control;
			meshfilter = m_mesh.AddComponent<MeshFilter>();
			meshrender = m_mesh.AddComponent<MeshRenderer>();
					meshrender.material = generator.m_material;
					meshfilter.sharedMesh = mesh;
					UnityEditor.EditorUtility.SetSelectedWireframeHidden(meshrender,true);
					mytransform.hideFlags = HideFlags.HideAndDontSave;
			placer.chunk = this;
			HasGrass = true;}
			//set flag so engine knows it can draw mesh now
			canDraw=true;
			//nullify all vertices and tris and normals so as to not hold onto unnecassary information
			tris = null;
			verts=null;
			normals=null;


			}
		}
		}
	}
}
