using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float rotationX = 5;
    private GameObject player;
    private float turnUpInput;
    private Quaternion targetRotation;

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
        //transform.rotation.ToAngleAxis(rotationX * turnUpInput, Vector3.left);????

    }

}
