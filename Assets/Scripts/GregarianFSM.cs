using UnityEngine;
using System.Collections;

public class GregarianFSM : MonoBehaviour {

	public enum GregarianStates
	{
		Test
	}

	public GregarianStates state = GregarianStates.Test;
	public int hunger;
	public int hungerBoundary;

	private NavMeshAgent agent;


	public IEnumerator StartFSM(){
		agent = gameObject.GetComponent<NavMeshAgent> ();
		while (true)
			yield return StartCoroutine (state.ToString ());
	}

	IEnumerator Test(){
		Debug.Log("Testing External Script for FSM");
		yield return new WaitForSeconds (60f);
	}
}
