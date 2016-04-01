using UnityEngine;
using System.Collections;
using VoxelEngine;
using System.Collections.Generic;

public class plantPlacer : MonoBehaviour {
	public Vector3[] Vertices;
	public static GameObject[] Trees;
	public static GameObject[] Grass;
	public static float[] TreesWeights;
	public static float[] GrassWeights;
	public int maxTrees;
	public int maxGrass;
	public static VoxelTerrainEngine m_Terrain;
	public static int chance;
	public VoxelChunk chunk;
	public int G = 0;
	static Mesh []mesh;
	static Material []material;
	GameObject gameo;
	public Camera cam;
	public Color[] control;
	float weight;
	float totalweights;
	bool foundgrass;
	// Use this for initialization
	void Start () {
	
		cam =Camera.main;
		if(m_Terrain==null){
			m_Terrain = GameObject.FindObjectOfType<VoxelTerrainEngine>();
		}
			

			//Fill arrays with grass and trees from terrain engine
		if(maxTrees==0){

			maxTrees = m_Terrain.MaxTrees;

			maxGrass = m_Terrain.MaxGrass;

			Grass = m_Terrain.Grass;

			Trees = m_Terrain.Trees;

			GrassWeights = m_Terrain.GrassWeights;

			//TreesWeights= m_Terrain.TreesWeights;
		}
			int g = Grass.Length;

			for(int t=0;t<GrassWeights.Length;t++)
			totalweights+=GrassWeights[t];

		if(mesh==null){

			mesh = new Mesh[g];

			material = new Material[g];

		for(int i =0;i < g;i++){

			mesh[i]=Grass[i].GetComponent<MeshFilter>().mesh;

			material[i]=Grass[i].GetComponent<MeshRenderer>().material;

			}}
			StartCoroutine(spawnstuff());}
IEnumerator spawnstuff(){

			yield return new WaitForSeconds(1);

			chance = UnityEngine.Random.Range(1,10);

			int T = Trees.Length;


			int V = Vertices.Length;


			chance = UnityEngine.Random.Range(1,10);
			G=0;
		for(int i = 0;i < maxGrass;i++){
		   int v = Random.Range(0,V);

			int R =0;

			float TWeights = 0;

			//basic weighting of grass which can be set up on Terrain script
			//for each grass values range is 0.0f to 10.0f

			weight = Random.Range(0,totalweights);

		for(int t=0;t<GrassWeights.Length;t++){
		if(TWeights<=weight){
					
		   TWeights+=GrassWeights[t];
				}
		if(TWeights>weight){
		   R=t;}

					
					}
			G++;	
			//supposed to check if alpha value is greater then x place grass here but couldnt get it to work
			//maybe someone else knows how to do it
		
			

			chunk.GrassMesh.Add(mesh[R]);

			chunk.GrassMat.Add(material[R] );

			chunk.Rot.Add(Random.Range(0.0f,360.0f));

		

			chunk.Pos.Add(Vertices[v]+transform.position);
					

		
				
				
				}

	

	//if the voxels were changed load the changes im going to implement this with the grass as well
		// so that we dont get grass in tunnels 
		if(chunk.VoxelSaver.GetFloat("TreeCount")!=0){

		for(int i = 0;i < chunk.VoxelSaver.GetFloat("TreeCount");i++){

			gameo = Instantiate (Trees[(int)chunk.VoxelSaver.GetFloat

			(" index "+i)],Vector3.zero,Quaternion.identity )as GameObject;

			gameo.transform.position = new Vector3(

			chunk.VoxelSaver.GetFloat("x"+i),

			chunk.VoxelSaver.GetFloat("y"+i),

			chunk.VoxelSaver.GetFloat("z"+i));

			chunk.treelist.Add(gameo.transform.position);

			chunk.treeindex.Add((int)chunk.VoxelSaver.GetFloat(" index "+i));
			}

			chunk.VoxelSaver.DeleteAll();

			chunk.VoxelSaver.Flush();
			}
			else{
			if(Trees.Length>0){
			if(chance==2||chance ==7||chance==9)

			for(int i = 0;i < maxTrees;i++){

				int t =UnityEngine.Random.Range(0,V);

				int num = UnityEngine.Random.Range(0,T);
			
				gameo = Instantiate (Trees[num],Vector3.zero,Quaternion.identity )as GameObject;

				gameo.transform.parent = transform;

				gameo.transform.position = Vertices[t]+transform.position+Vector3.down;

				gameo.isStatic = gameObject.isStatic;

				chunk.treelist.Add(gameo.transform.position);

				chunk.treeindex.Add(num);
			
			}
		}
		}
		

			Vertices=null;

			Destroy(this);
	}
	// Update is called once per frame

}


