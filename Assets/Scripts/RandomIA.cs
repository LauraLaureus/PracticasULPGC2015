using UnityEngine;
using System.Collections;

public class RandomIA : MonoBehaviour {

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMesh;
	Vector3 finalPosition;
	bool destinationReached = true;

	public float walkRadius = 10;
	int frames_forTimeOut = 1000;
	
	void Start () {
		rb = GetComponent<Rigidbody>();
		navMesh = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

		if (destinationReached) {
			newDestination();
		} else {
			if (frames_forTimeOut == 0){
				destinationReached = true;
				frames_forTimeOut = 1000;
			}else{
				frames_forTimeOut -=1;
			}
		} 

		this.transform.position = newPosition();
		rb.MovePosition(this.transform.position);
		if (this.transform.position == finalPosition)
			destinationReached = true;
	}

	void newDestination(){
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition (randomDirection, out hit, walkRadius, 1);
		finalPosition = hit.position;
		destinationReached = false;
	}

	Vector3 newPosition(){
		Vector3 control = Vector3.MoveTowards (transform.position, finalPosition, navMesh.speed);
		return control;
	}
	
}
