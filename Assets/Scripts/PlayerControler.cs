using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float rotationX = 5;
	public float playerSpeed = 5;
    private Quaternion targetRotation;
    private float forwardInput, turnAroundInput;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
        targetRotation = transform.rotation;
        forwardInput = turnAroundInput = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        forwardInput = Input.GetAxis("Vertical");
        turnAroundInput = Input.GetAxis("Mouse X");

        rb.velocity = transform.forward * forwardInput * playerSpeed;
        targetRotation *= Quaternion.AngleAxis(rotationX * turnAroundInput, Vector3.up);
        transform.rotation = targetRotation;
	}

    public Quaternion GetRotation()
    {
        return targetRotation;
    }

}
