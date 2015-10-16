using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {
	private Rigidbody rb;
	public float rotationX = 5;
	public float playerSpeed = 5;
    private Quaternion targetRotation;
    private float forwardInput, turnAroundInput;
    private float loadBow;
    public float maxLoad = 10.0f;
    public float growFactor = 2;
    private bool shotArrow;
    public GameObject arrowPrefab;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
        targetRotation = transform.rotation;
        forwardInput = turnAroundInput = 0;
        loadBow = 0.0f;
        shotArrow = false;
	}
	
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            loadBow += 1 / (float) System.Math.Pow(maxLoad,growFactor);

        else if (loadBow > 0.0f)
            shotArrow = true;
        
    }
	// Update is called once per frame
	void FixedUpdate () {

        forwardInput = Input.GetAxis("Vertical");
        turnAroundInput = Input.GetAxis("Mouse X");

        rb.velocity = transform.forward * forwardInput * playerSpeed;
        targetRotation *= Quaternion.AngleAxis(rotationX * turnAroundInput, Vector3.up);
        transform.rotation = targetRotation;

        if (shotArrow)
        {
            loadBow = 0.0f;
            shotArrow = false;
            Instantiate(arrowPrefab, transform.position, transform.rotation);
            Rigidbody rbArrow = arrowPrefab.GetComponent<Rigidbody>();
            rbArrow.AddForce(transform.forward);
        }
	}

    public Quaternion GetRotation()
    {
        return targetRotation;
    }

}
