using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GregarianSight : MonoBehaviour {

	public float sightDistance = 2f;

	private Dictionary<string,Vector3> fruitPlaces;
	private NavMeshAgent agent;

	void Start(){
		agent = this.gameObject.GetComponent<NavMeshAgent> ();
		fruitPlaces = new Dictionary<string, Vector3> ();
	}

	void FixedUpdate () {

		if (doesItSmellFruit (gameObject.transform.position)) {
			Debug.Log("Huele a fruta");
		}
		if (agent.remainingDistance <=1f) {
			agent.SetDestination (takeALook());
		}
		Debug.DrawRay(agent.destination,(Vector3.up*100),Color.white);
	}

	bool doesItSmellFruit(Vector3 currentPosition){
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 2f, Vector3.forward);
		foreach (RaycastHit hit in hits) {
			if(hit.collider.gameObject.tag == "Fruit") return true;
		}
		return false;
	}
	Vector3 takeALook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		NavMeshHit hit;
		if(!NavMesh.Raycast(whereIam,whereIam + whereIamLooking * sightDistance,out hit,NavMesh.AllAreas)){
			//Debug.Log ("Voy para el frente");
			return whereIam + whereIamLooking * sightDistance;
		} else if (!NavMesh.Raycast (whereIam, whereIam + Vector3.right * sightDistance,out hit,NavMesh.AllAreas)) {
			//Debug.Log ("Voy para la derecha");
			return whereIam + Vector3.right* sightDistance;
		} else if (!NavMesh.Raycast (whereIam, whereIam + Vector3.left* sightDistance,out hit,NavMesh.AllAreas)) {
			//Debug.Log ("Voy para la izquierda");
			return whereIam - Vector3.right * sightDistance;
		} else {
			//Debug.Log("Vamos para atrás");
			this.gameObject.transform.LookAt(-whereIamLooking+whereIam);
			return whereIam-whereIamLooking;
		}

	}
}
