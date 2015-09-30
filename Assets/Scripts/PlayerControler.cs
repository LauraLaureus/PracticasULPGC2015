using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float rotationX;
	public float playerSpeed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		this.transform.eulerAngles = this.transform.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal")*rotationX,0f);
		//this.transform.position =  this.transform.position + Input.GetAxis ("Vertical") * transform.forward;
		rb.velocity = Input.GetAxis ("Vertical") * transform.forward * playerSpeed;
	}

}
