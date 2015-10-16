using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float rotationX = 5;
    private GameObject player;
    private float turnUpInput;
    private Quaternion targetRotation;
    public GameObject arrowPrefab;

    void OnEnable()
    {
        PlayerControler.OnArrowShot += shotArrow;
    }

    void Start()
    {
        turnUpInput = 0;
        targetRotation = transform.rotation;
    }
	// Update is called once per frame
	void FixedUpdate () {
        turnUpInput = Input.GetAxis("Mouse Y");
        targetRotation = transform.rotation;
        targetRotation *= Quaternion.AngleAxis(rotationX * turnUpInput, Vector3.left);

        transform.rotation = targetRotation;

    }

    void shotArrow(float force)
    {
        GameObject arrow = (GameObject) Instantiate(arrowPrefab, transform.position + transform.forward*2, transform.rotation);
        Rigidbody rbArrow = arrow.GetComponent<Rigidbody>();
        rbArrow.velocity = transform.forward * force;
    }

    void OnDisable()
    {
        PlayerControler.OnArrowShot += shotArrow;
    }
}
