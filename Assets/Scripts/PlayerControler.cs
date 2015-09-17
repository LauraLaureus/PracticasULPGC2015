using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float speed;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 moovement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.AddForce (moovement * speed);
	}

}
