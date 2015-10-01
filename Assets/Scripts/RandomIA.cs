using UnityEngine;
using System.Collections;

public class RandomIA : MonoBehaviour {

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMesh;
	Vector3 nextCorner;
	
	void Start () {
		rb = GetComponent<Rigidbody>();
		navMesh = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		navMesh.SetDestination(GameObject.Find("Sphere(Clone)").transform.position);
		nextCorner = navMesh.steeringTarget;
		this.transform.position = Vector3.MoveTowards(transform.position, nextCorner, navMesh.speed);
	}


}
