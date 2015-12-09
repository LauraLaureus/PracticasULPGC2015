using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float rotationX = 5;
	public float playerSpeed = 5;
    private Quaternion targetRotation;
    private float forwardInput, leftRightInput, turnAroundInput;

    private bool haveKey = true;

    public float maxLoad = 10.0f;
    public float growFactor = 2;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
        targetRotation = transform.rotation;
        forwardInput = leftRightInput = turnAroundInput = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        forwardInput = Input.GetAxis("Vertical");
        leftRightInput = Input.GetAxis("Horizontal");
        turnAroundInput = Input.GetAxis("Mouse X");

        rb.velocity = transform.forward * forwardInput * playerSpeed +  transform.right*leftRightInput*playerSpeed;
        targetRotation *= Quaternion.AngleAxis(rotationX * turnAroundInput, Vector3.up);
        transform.rotation = targetRotation;

	}

    public Quaternion GetRotation()
    {
        return targetRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            Destroy(other.gameObject);
            haveKey = true;
        }
        else if (other.gameObject.CompareTag("Door") && haveKey)
        {
            Destroy(other.gameObject);
            //haveKey = false;
        }
    }

}
