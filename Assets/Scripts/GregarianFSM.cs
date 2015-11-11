using UnityEngine;
using System.Collections;

public class GregarianFSM : MonoBehaviour {

	public enum GregarianStates
	{
		Evaluate,
		Wandering,
		Flee,
		ChasingFruit
	}

	public GregarianStates state;
	public int hunger;
	public int hungerBoundary;

	private NavMeshAgent agent;


	public IEnumerator StartFSM(){
		state = GregarianStates.Wandering;
		agent = gameObject.GetComponent<NavMeshAgent> ();
		while (true)
			yield return StartCoroutine (state.ToString ());
	}

	IEnumerator Test(){
		Debug.Log("Testing External Script for FSM");
		yield return new WaitForSeconds (60f);
	}

	
	IEnumerator Wandering(){
		agent.angularSpeed = 1;
		agent.speed = 1.5f;
		hunger += 1;
		
		state = GregarianStates.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator Flee(){
		
		agent.angularSpeed = 5;
		agent.speed = 5f;
		hunger += 5;
		
		state = GregarianStates.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator ChasingFruit(){
		agent.angularSpeed = 3;
		agent.speed = 3f;
		hunger += 2;
		
		state = GregarianStates.Evaluate;
		yield return 0;
	}
	
	IEnumerator Evaluate(){
		
		if (foundEnemys ()) {
			state = GregarianStates.Flee;
		} else if (hunger > hungerBoundary) {
			state = GregarianStates.ChasingFruit;
		} else {
			state = GregarianStates.Wandering;
		}
		yield return 0;
	}
	
	bool foundEnemys(){
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 5f, Vector3.forward);
		int countGregarianEnemys = 0;
		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Bunny") countGregarianEnemys +=1;
		}
		return countGregarianEnemys > 0;
	}

}
