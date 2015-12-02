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
	public float hunger;
	public float hungerBoundary;

	private NavMeshAgent agent;
	private GregarianSpacialMemory mem;

	public IEnumerator StartFSM(){
		mem = gameObject.GetComponent<GregarianSpacialMemory> ();
		state = GregarianStates.Wandering;
		agent = gameObject.GetComponent<NavMeshAgent> ();
		while (true)
			yield return StartCoroutine (state.ToString ());
	}


	
	IEnumerator Wandering(){
		agent.angularSpeed = 1;
		agent.speed = 1.5f;
		hunger += 0.1f;
		
		state = GregarianStates.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator Flee(){
		
		agent.angularSpeed = 5;
		agent.speed = 5f;
		hunger += 3f;
		
		state = GregarianStates.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator ChasingFruit(){
		agent.angularSpeed = 3;
		agent.speed = 3f;
		hunger += 0.5f;
		
		state = GregarianStates.Evaluate;
		yield return 0;
	}
	
	IEnumerator Evaluate(){
        if (hunger > 10 * hungerBoundary) {
            Debug.Log("Me muero de hambre");
            DestroyObject(this.gameObject);
        }
		else if (mem.areEnemiesClose()) {
			state = GregarianStates.Flee;
		} else if (amIHungry() && mem.doIknowWhereFruitIs()) {
			state = GregarianStates.ChasingFruit;
		} else {
			state = GregarianStates.Wandering;
		}
		yield return 0;
	}
	


	public bool amIHungry(){
		return hunger > hungerBoundary;
	}

    public bool amIWandering() {
        return this.state == GregarianStates.Wandering || this.state == GregarianStates.Evaluate;
    }

}
