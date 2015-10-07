using UnityEngine;
using System.Collections;

public class RandomIA : MonoBehaviour {

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMesh;

	
	void Start () {
		rb = GetComponent<Rigidbody>();
		navMesh = GetComponent<NavMeshAgent>();
	}

	//TODO moverse aleatoriamente y simular saltar.
	// Update is called once per frame
	void Update () {
		if (!navMesh.hasPath){
			Debug.Log("New Path");
			Vector3 randomDirection = Random.insideUnitSphere * 2;
			randomDirection += transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, 2, 1);
			Vector3 finalPosition = hit.position;
			navMesh.SetDestination(finalPosition);
			
		}

		Vector3 nextCorner = navMesh.steeringTarget; //steeringTarget es un punto entre el punto actual y el destino o el destino.
		rb.MovePosition (Vector3.MoveTowards (transform.position, nextCorner, navMesh.speed));

	}


	
}
