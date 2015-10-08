using UnityEngine;
using System.Collections;

public class RandomIA : MonoBehaviour {

	public enum State{
		Moving,
		Resting,
		Evaluating
	}

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMesh;

	public State state;
	public float maxTimeForResting=10; 
	public float fatigue = 0;

	void Start () {
		rb = GetComponent<Rigidbody>();
		navMesh = GetComponent<NavMeshAgent>();
		state = State.Evaluating;
		StartCoroutine (FSM ());
	}

	IEnumerator FSM(){
		while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Moving(){
		Vector3 randomDirection = Random.insideUnitSphere * 2;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, 2, 1);
		Vector3 finalPosition = hit.position;
		navMesh.SetDestination(finalPosition);

		fatigue +=Mathf.Abs( Vector3.Distance (finalPosition, transform.position));

		Vector3 deltaRigidBody = Vector3.one;
		while (deltaRigidBody != Vector3.zero) {
			deltaRigidBody = rb.position;
			Vector3 nextCorner = navMesh.steeringTarget; //steeringTarget es un punto entre el punto actual y el destino o el destino.
			rb.MovePosition (Vector3.MoveTowards (rb.position, nextCorner, navMesh.speed*Time.fixedDeltaTime));
			yield return new WaitForEndOfFrame();
			deltaRigidBody -= rb.position;
		}
		Debug.Log ("Ya llegué a mi destino");
		state = State.Evaluating;

	}


	IEnumerator Resting(){
		float s = Random.value * maxTimeForResting; // un valor entre 0 y 10 s
		float init = Time.time;
		Debug.Log ("Descansaré " + s.ToString () + "segundos");
		while (Time.time <= init+s)
			yield return Time.deltaTime;

		Debug.Log("YA!");
		if (fatigue.Equals (Mathf.Infinity)) {
			fatigue = s;
		} else {
			fatigue -= s;
		}

		state = State.Evaluating;
	}

	IEnumerator Evaluating(){
		if (fatigue < maxTimeForResting) {
			Debug.Log("Domir es para humanos!!");
			yield return null;
			state = State.Moving;
		} else {
			yield return null;
			Debug.Log("Estoy cansado.");
			state = State.Resting;
		}

	}

	
}
