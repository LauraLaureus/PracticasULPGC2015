using UnityEngine;
using System.Collections;

public class BunnyIA : MonoBehaviour {

	public enum State{
		Moving,
		Resting,
		Evaluating
	}

	public enum BunnyMoveTarget{
		RandomTarget,
		FoodTarget
	}

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMesh;

	public State state;
	public BunnyMoveTarget target;
	public float maxTimeForResting=10; 
	public float fatigue = 0;
	public float hunger = 0;

	void Start () {
		rb = GetComponent<Rigidbody>();
		navMesh = GetComponent<NavMeshAgent>();
		target = BunnyMoveTarget.RandomTarget;
		state = State.Evaluating;
		StartCoroutine (FSM ());
	}

	IEnumerator FSM(){
		while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Moving(){
		Vector3 finalPosition;
		if (target == BunnyMoveTarget.RandomTarget) {
			Vector3 randomDirection = Random.insideUnitSphere * 2;
			randomDirection += transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition (randomDirection, out hit, 2, 1);
			finalPosition = hit.position;
			navMesh.SetDestination (finalPosition);
		} else {
			navMesh.SetDestination(GameObject.Find("Fruit(Clone)").transform.position);
			finalPosition = navMesh.steeringTarget;

		}


		float delta = Mathf.Abs( Vector3.Distance (finalPosition, transform.position));
		fatigue += delta;
		hunger += delta / 4;

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

		if (fatigue >= maxTimeForResting) {
			Debug.Log ("Estoy cansado.");
			state = State.Resting;
		} else if (hunger >= maxTimeForResting) {
			Debug.Log ("Voy a buscar comida");
			target = BunnyMoveTarget.FoodTarget;
			state = State.Moving;
		} else {
			Debug.Log("Me voy a dar un garbeo por ahí.");
			target = BunnyMoveTarget.RandomTarget;
			state = State.Moving;
		}
		yield return null;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Fruit(Clone)") {
			Debug.Log("Ñam,Ñam que rico");
			hunger = 0;
			other.gameObject.SetActive (false);
		}

	}
}
