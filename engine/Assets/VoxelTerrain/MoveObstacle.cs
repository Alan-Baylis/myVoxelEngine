using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class MoveObstacle : MonoBehaviour {
	public Rigidbody body;
	public float speed;
	public float time;
	public Vector3 pos;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {

		speed = 1;
		Shader.SetGlobalFloat("_speed",speed);
		pos = transform.position;
			Shader.SetGlobalVector("_Obstacle",pos);

	}
}
