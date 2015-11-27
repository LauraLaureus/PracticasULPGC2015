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

        mem.deleteNullObjects(); 
		Vector3 destination = agent.destination; //añadir que el destino no coincide con una fruta!!
        if (Vector3.Angle(destination - this.transform.position, this.transform.forward) > 90f)
        {
            this.gameObject.transform.LookAt(destination);
            return;
        }
        
        if (fsm.amIHungry () && mem.doIknowWhereFruitIs ()) {
		
			if ((agent.destination - mem.getFruit ()).sqrMagnitude < 0.01f) {
                Debug.Log("Estoy cerca de la comida");
				return;
			} else {
                Debug.Log("Tengo hambre mamá");
				destination = mem.getFruit ();
			}
		} else if (fsm.amIWandering()) { 
			destination = takeAPhysicalNavigableLook ();
		} else if (!agent.hasPath) {
			destination = takeAPhysicalNavigableLook ();
		} else {
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
        Debug.DrawRay(whereIam, forward, Color.grey);

		Physics.Raycast (whereIam, whereIamLooking+Vector3.right, out hit);
		right = hit.point-whereIam;
        Debug.DrawRay(whereIam, right, Color.grey);

		Physics.Raycast (whereIam, whereIamLooking+Vector3.left, out hit);
		left = hit.point-whereIam;
        Debug.DrawRay(whereIam, left, Color.grey);
		
        result = computateDestinationInSight (forward,right, left);
        Debug.DrawRay(whereIam, forward, Color.white);

		return result+whereIam;
	}

	Vector3 computateDestinationInSight( Vector3 forward, Vector3 right,Vector3 left){

		/*Control de que evtar paredes*/

       
		if (right.magnitude < sightDistance / 3f) {
            //Debug.Log("Right distance =" + right.magnitude.ToString());
			right = Vector3.zero;
		}

        
		if (left.magnitude < sightDistance / 3f) {
            //Debug.Log("Left distance =" + right.magnitude.ToString());
			left = Vector3.zero;
		}

		if (forward.magnitude < sightDistance / 3f ) {
			if(right.magnitude < 0.1f && left.magnitude <0.1f){
				gameObject.transform.LookAt((gameObject.transform.forward+Vector3.right));
            } 
            //Debug.Log("Forward distance =" + right.magnitude.ToString());
			forward = Vector3.zero;
		}

		if ((forward + left + right).magnitude < 0.1f) {
            return (gameObject.transform.position - mem.getLastPointIStayed()).normalized * sightDistance;
		}
        //Debug.Log("Suma de las tres direcciones:" + (forward + left + right).magnitude.ToString());
		return (forward + left + right);


	}


	public void eatenFruit(GameObject f){
		mem.eatenFruit (f);
	}

}
