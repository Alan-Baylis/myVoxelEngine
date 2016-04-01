using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class addTree : MonoBehaviour {
	public bendPlant plant;
	public bool add;
	public int terrain;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(add){
			int T =plant.terrains[terrain].terrainData.treeInstanceCount;
			add = false;
			Vector3 size = plant.terrains[terrain].terrainData.size;
			Vector3 TerPos = plant.terrains[terrain].transform.position;
			for(int t = 0;t < T;t++){

				Vector3 TPos = plant.terrains[terrain].terrainData.treeInstances[t].position;
				plant.pos.Add(new Vector3(TPos.x*size.x,TPos.y*size.y,TPos.z*size.z)+TerPos);
			}
			Debug.Log("finished");
	}



	}
}
