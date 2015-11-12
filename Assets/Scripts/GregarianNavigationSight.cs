using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GregarianNavigationSight : MonoBehaviour {

	public float sightDistance = 4f;

	private NavMeshAgent agent;
	private GregarianFSM fsm;
	private GregarianSpacialMemory mem;

	void Start(){
		agent = this.gameObject.GetComponent<NavMeshAgent> ();
		mem = gameObject.GetComponent<GregarianSpacialMemory> ();
		fsm = gameObject.GetComponent<GregarianFSM> ();
	}

	void FixedUpdate () {

		Vector3 destination;

		if (agent.remainingDistance <=0.5f) {
			if (fsm.amIHungry() && mem.doIknowWhereFruitIs ()) {
				destination = mem.getFruit();
			}else{
				destination = takeALook();
			}
			agent.SetDestination (destination);
		}
		Debug.DrawRay(agent.destination,(Vector3.up*100),Color.white);
	}




	Vector3 takeALook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		NavMeshHit hit;
		if(!NavMesh.Raycast(whereIam,whereIam + whereIamLooking * sightDistance,out hit,NavMesh.AllAreas)){
			return whereIam + whereIamLooking * sightDistance;
		} else if (!NavMesh.Raycast (whereIam, whereIam + Vector3.right * sightDistance,out hit,NavMesh.AllAreas)) {
			Debug.Log("Derecha");
			//this.gameObject.transform.LookAt(whereIam + Vector3.right* sightDistance);
			return whereIam + Vector3.right* sightDistance;
		} else if (!NavMesh.Raycast (whereIam, whereIam + Vector3.left* sightDistance,out hit,NavMesh.AllAreas)) {
			Debug.Log("Izquierda");
			//this.gameObject.transform.LookAt(whereIam + Vector3.left* sightDistance);
			return whereIam - Vector3.right * sightDistance;
		} else {
			Debug.Log("Atrás");
			this.gameObject.transform.LookAt(-whereIamLooking+whereIam);
			return whereIam-whereIamLooking;
		}

	}

	public void eatenFruit(){
		mem.eatenFruit ();
	}

}
