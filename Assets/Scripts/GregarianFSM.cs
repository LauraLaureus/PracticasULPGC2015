﻿using UnityEngine;
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
		
		if (foundEnemys ()) {
			state = GregarianStates.Flee;
		} else if (amIHungry() && mem.doIknowWhereFruitIs()) {
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

	public bool amIHungry(){
		return hunger > hungerBoundary;
	}

}
