using UnityEngine;
using System.Collections;

public class KeyController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
}
