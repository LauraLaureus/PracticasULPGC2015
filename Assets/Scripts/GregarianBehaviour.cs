﻿using UnityEngine;
using System.Collections;

public class GregarianBehaviour : MonoBehaviour {


	
	private Rigidbody rb;
	private NavMeshAgent navMeshAgent;
	private Vector3 steeringForce;



	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		navMeshAgent = this.gameObject.GetComponent<NavMeshAgent> ();
		steeringForce = Vector3.zero;
		StartCoroutine (startFSM());
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Fruit") {
			this.hunger -=50;
		}
	}

	/*---------FSM---------*/

	public enum GregarianState{
		Evaluate,
		Wandering,
		Flee,
		ChasingFruit
	}

	/*--------Variables de estado---------*/
	public int hunger;
	public GregarianState state;
	public int hungerThreshold;

	public IEnumerator startFSM(){
		while (true)
			yield return StartCoroutine (state.ToString ());
	}
	
	IEnumerator Wandering(){
		navMeshAgent.angularSpeed = 1;
		navMeshAgent.speed = 1.5f;
		hunger += 1;
		
		state = GregarianState.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator Flee(){
		
		navMeshAgent.angularSpeed = 5;
		navMeshAgent.speed = 5f;
		hunger += 5;
		
		state = GregarianState.Evaluate;
		
		yield return 0;
	}
	
	IEnumerator ChasingFruit(){
		navMeshAgent.angularSpeed = 3;
		navMeshAgent.speed = 3f;
		hunger += 2;

		state = GregarianState.Evaluate;
		yield return 0;
	}
	
	IEnumerator Evaluate(){

		if (foundEnemys ()) {
			state = GregarianState.Flee;
		} else if (hunger > hungerThreshold) {
			state = GregarianState.ChasingFruit;
			if(navMeshAgent.remainingDistance <= 0.01)
				Debug.Log ("Tengo Hambre.");
				navMeshAgent.SetDestination(GregarianNavigator.navigate(state,transform.position,transform.forward));
		} else {
			state = GregarianState.Wandering;
			if(navMeshAgent.remainingDistance <= 0.1)
				navMeshAgent.SetDestination(GregarianNavigator.navigate(state,transform.position,transform.forward));
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

	/*BOIDS*/
	void FixedUpdate () {
	
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 5f, Vector3.forward);
		Debug.DrawLine (this.transform.position,this.transform.position+this.transform.forward, Color.blue);

		steeringForce += GregarianWeights.w_inercia * steeringForce;
		Debug.DrawLine (this.transform.position,this.transform.position+steeringForce, Color.green);

		steeringForce +=  0.001f*randomVector();

		steeringForce += calculateNavigationVector ()* GregarianWeights.w_navigation;
		steeringForce += calculateSeparationVector (hits) *GregarianWeights.w_separation;
		steeringForce += calculateCohesionVector (hits)*GregarianWeights.w_cohesion;
		steeringForce += calculateAligmentVector(hits) *GregarianWeights.w_aligment;


		steeringForce = Vector3.ClampMagnitude (steeringForce, navMeshAgent.speed);
		steeringForce.y = 0;
		transform.rotation = Quaternion.Lerp (
				transform.rotation,
				Quaternion.LookRotation (steeringForce, Vector3.up),
				navMeshAgent.angularSpeed * Time.deltaTime
		);
		rb.velocity = transform.forward * navMeshAgent.speed;

		Debug.DrawLine (this.transform.position, this.transform.position + steeringForce,Color.red);

	}

	Vector3 calculateSeparationVector(RaycastHit [] hits){
		Vector3 result = Vector3.zero;

		foreach (RaycastHit h in hits) {

			if (h.collider.gameObject.tag == "Gregarian"){

				Vector3 toGregarian = this.transform.position - h.collider.gameObject.transform.position;
				float towardsMateWeight = toGregarian.magnitude > 0? 1.0f / (toGregarian.magnitude * 0.1f) : 0.001f;
				result += toGregarian.normalized * towardsMateWeight;
			}
		}
		Debug.DrawLine (this.transform.position,this.transform.position+result,Color.yellow);

		return result;
	}

	Vector3 calculateCohesionVector(RaycastHit[] hits){

		Vector3 result = Vector3.zero;
	
		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Gregarian"){

				Vector3 toGregarian =  h.collider.gameObject.transform.position - this.transform.position;
				result += toGregarian;
			}
		}

		Debug.DrawLine (this.transform.position,this.transform.position+result,Color.black);
		return result;
	}

	Vector3 calculateNavigationVector(){
		Vector3 result = this.navMeshAgent.steeringTarget;
		Debug.DrawLine (this.transform.position,result,Color.magenta);
		return result - this.transform.position;
	}

	Vector3 randomVector(){
		Vector3 r = this.transform.position + Random.insideUnitSphere * 3;
		return r - transform.position;
	}

	Vector3 calculateAligmentVector(RaycastHit[] hits){
		Vector3 result = Vector3.zero;
		int count = 0;
		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Gregarian"){

				result += h.collider.gameObject.GetComponent<Rigidbody>().velocity;
				count +=1;
			}
		}

		if (count == 0) {
			return Vector3.zero;
		}

		result /= count;
		Debug.DrawLine(this.transform.position, this.transform.position + result , Color.cyan);
		return result ;
	}

}
