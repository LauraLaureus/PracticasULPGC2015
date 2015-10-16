using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float rotationX = 5;
	public float playerSpeed = 5;
    private Quaternion targetRotation;
    private float forwardInput, leftRightInput, turnAroundInput;

    private float loadBow;
    public float maxLoad = 10.0f;
    public float growFactor = 2;

    public delegate void ArrowShot(float force);
    public static event ArrowShot OnArrowShot;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
        targetRotation = transform.rotation;
        forwardInput = leftRightInput = turnAroundInput = 0;
        loadBow = 0.0f;
	}
	
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && loadBow < maxLoad)
            loadBow += 1 / (float) System.Math.Pow(maxLoad,growFactor);

        else if (Input.GetMouseButtonUp(0) && loadBow > 0.0f)
        {
            if (OnArrowShot != null)
                OnArrowShot(maxLoad);
            loadBow = 0.0f;
        }
        
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

}
