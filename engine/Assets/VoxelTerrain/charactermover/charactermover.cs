using UnityEngine;
using System.Collections;

public class charactermover : MonoBehaviour {
	public CharacterController controller;
	public float speed;
	public float Mass;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		controller.SimpleMove(transform.forward*speed*Input.GetAxis("Vertical"));

		controller.SimpleMove(transform.right*speed*Input.GetAxis("Horizontal"));



		if(Input.GetKey(KeyCode.Space)){
			controller.Move(Vector3.up*5);
		}
	}
}
