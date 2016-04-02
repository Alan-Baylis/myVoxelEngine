using UnityEngine;
using System.Collections;
using VoxelEngine;
public class TerrainUpdate : MonoBehaviour {
	public VoxelTerrainEngine engine;
	// Update is called once per frame
	void Start(){
		if(engine==null)
			engine = GetComponent<VoxelTerrainEngine>();
		
		Transform[] gamos = GetComponentsInChildren<Transform>();
		for(int i = 0;i < gamos.Length;i++){
			if(gamos[i]!=transform){
				DestroyImmediate(gamos[i].gameObject);

			}
		}
		engine.Initialize();

		Debug.Log("Entered Playmode");
	}
	void Update () {
	engine.UpdateTerrain();
	}
}
