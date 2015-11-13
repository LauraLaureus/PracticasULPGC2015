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

		if (fsm.amIHungry () && mem.doIknowWhereFruitIs ()) {
			destination = mem.getFruit ();
		} else {
			if(!agent.hasPath || agent.remainingDistance < 1f)
				destination = takeAPhysicalNavigableLook();
			else
				destination = agent.destination;
		}
		mem.setNewPointInPath (destination);
		agent.SetDestination (destination);
		Debug.DrawRay(agent.destination,(Vector3.up*100),Color.white);

	}




	Vector3 takeALook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		if (isNavigable (whereIam, whereIam + whereIamLooking * sightDistance) && mem.canIgoHere (whereIam, whereIam + whereIamLooking * sightDistance)) {
			return whereIam + whereIamLooking * sightDistance;
		} else if (isNavigable (whereIam, whereIam + Vector3.right * sightDistance) && mem.canIgoHere (whereIam, whereIam + Vector3.right * sightDistance)) {
			Debug.Log ("Derecha");
			return whereIam + Vector3.right * sightDistance;
		} else if (isNavigable (whereIam, whereIam + Vector3.left * sightDistance) && mem.canIgoHere (whereIam, whereIam + Vector3.left * sightDistance)) {
			Debug.Log ("Izquierda");
			return whereIam - Vector3.right * sightDistance;
		} else if (isNavigable (whereIam, whereIam - whereIamLooking * sightDistance) && mem.canIgoHere (whereIam, whereIam - whereIamLooking * sightDistance)) {
			Debug.Log ("Atrás");
			return whereIam - whereIamLooking * sightDistance;
		} else {
			NavMeshHit hit;
			if(NavMesh.SamplePosition(whereIam,out hit,sightDistance,NavMesh.AllAreas)){
				Debug.Log("Hit position");
				return hit.position;
			}
			return whereIam;
		}

	}
	Vector3 takeAPhysicalNavigableLook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		RaycastHit hit;
		Vector3 result = Vector3.zero;

		Physics.Raycast (whereIam, whereIamLooking, out hit);
		result += whereIam-hit.point;
		Debug.Log ("Raycast1" + result.ToString ());

		Physics.Raycast (whereIam, whereIamLooking+Vector3.right, out hit);
		result += whereIam-hit.point;
		Debug.Log ("Raycast2" + result.ToString ());

		Physics.Raycast (whereIam, whereIamLooking+Vector3.left, out hit);
		result += whereIam-hit.point;
		Debug.Log ("Raycast3" + result.ToString ());

		/*NavMeshHit navhit;
		if(NavMesh.SamplePosition(whereIam,out navhit,sightDistance,NavMesh.AllAreas)){
			Debug.Log("Hit position");
			result += whereIam -navhit.position;
		}*/

		return result+whereIam;
	}


	Vector3 takeANavigableLook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		NavMeshHit hit;
		Vector3 result = whereIam;

		if (NavMesh.Raycast (whereIam, whereIam + whereIamLooking *sightDistance, out hit, NavMesh.AllAreas)) {
			Debug.Log ("Dentro del Raycast");
			result += hit.position;
		}
		if (NavMesh.Raycast (whereIam, whereIam + (whereIamLooking + Vector3.right * sightDistance), out hit, NavMesh.AllAreas)) {
			Debug.Log ("Dentro del raycast de la derecha");
			result += hit.position;
		}
		if (NavMesh.Raycast (whereIam, whereIam +(whereIamLooking + Vector3.left * sightDistance), out hit, NavMesh.AllAreas)) {
			Debug.Log ("Dentro del raycast de la izquierda");
			result += hit.position;
		}
		//Debug.Log (result.ToString ());
		return result;
	}

	bool isNavigable(Vector3 origin, Vector3 destination){
		NavMeshHit hit;
		return !NavMesh.Raycast (origin,destination,out hit,NavMesh.AllAreas);
	}

	public void eatenFruit(){
		mem.eatenFruit ();
	}

}
