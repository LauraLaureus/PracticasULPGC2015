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
		if (fsm.amIHungry () && mem.CountMemories () > 0 && mem.doIknowWhereFruitIs ()) {
			destination = mem.getFruit ();
		} else if (mem.isPersitentDestination()) {
			Debug.Log("is Persistent");
			destination = takeAPhysicalNavigableLook();
		}
		else {
			if(!agent.hasPath || agent.remainingDistance < 1.1f)
				destination = takeAPhysicalNavigableLook();
			else
				//destination = agent.destination;
				return;
		}


		mem.setNewPointInPath (destination);
		agent.SetDestination (destination);

		Debug.DrawRay(agent.destination,(Vector3.up*100),Color.white);

	}



	Vector3 takeAPhysicalNavigableLook(){
		Vector3 whereIam = this.gameObject.transform.position;
		Vector3 whereIamLooking = this.gameObject.transform.forward;

		RaycastHit hit;
		Vector3 result = Vector3.zero;
		Vector3 forward,right,left;

		Physics.Raycast (whereIam, whereIamLooking, out hit);
		forward = hit.point - whereIam;


		Physics.Raycast (whereIam, whereIamLooking+Vector3.right, out hit);
		right = hit.point-whereIam;


		Physics.Raycast (whereIam, whereIamLooking+Vector3.left, out hit);
		left = hit.point-whereIam;

		result = computateDestinationInSight (forward,right, left);

		return result+whereIam;
	}

	Vector3 computateDestinationInSight( Vector3 forward, Vector3 right,Vector3 left){

		/*Control de que evtar paredes*/

		if (right.magnitude < sightDistance / 3f) {
			right = Vector3.zero;
		}

		if (left.magnitude < sightDistance / 3f) {
			left = Vector3.zero;
		}

		if (forward.magnitude < sightDistance / 3f ) {
			if(right.magnitude < 0.1f && left.magnitude <0.1f){
				Debug.Log("To Icengard");
				//return (gameObject.transform.position-mem.getLastPointIStayed()).normalized*sightDistance;
				gameObject.transform.LookAt((gameObject.transform.forward+Vector3.right));
			}
			forward = Vector3.zero;
		}

		if ((forward + left + right).magnitude < 0.1f) {
			NavMeshHit hit;
			if(NavMesh.SamplePosition(gameObject.transform.position, out hit, 2f*sightDistance,NavMesh.AllAreas)){
				Debug.Log("To DigitalWorld");
				return hit.position;
			}else{
				Debug.Log ("To Pueblo Paleta");
				return (gameObject.transform.position-mem.getLastPointIStayed()).normalized*sightDistance;
			}
		}

		return (forward + left + right);


	}


	/*bool isNavigable(Vector3 origin, Vector3 destination){
		NavMeshHit hit;
		return !NavMesh.Raycast (origin,destination,out hit,NavMesh.AllAreas);
	}*/

	public void eatenFruit(GameObject f){
		mem.eatenFruit (f);
	}

}
