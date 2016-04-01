using UnityEngine;
using System.Collections;
using VoxelEngine;
public class addDeletevoxel : MonoBehaviour {
	Ray ray;
	public LayerMask mask;
	public bool islocked;
	GameObject block;
	// Use this for initialization
	void Start () {
		block = GameObject.CreatePrimitive(PrimitiveType.Cube);
		block.SetActive(true);
		Destroy(block.GetComponent<BoxCollider>());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.L)&&islocked==false){
			islocked = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else if(Input.GetKey(KeyCode.L)&&islocked){
			islocked = false;
			Cursor.lockState = CursorLockMode.None;
		}


		RaycastHit hit;
		ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));

		if(VoxelTerrainEngine.RaycastVoxels(ray, out hit, 100 ,mask  )){
			block.transform.position = hit.point;
		if(Input.GetButton("Fire1")){

				}
			if(Input.GetButton("Fire2")){
	
		}
		}
		}
}

	

