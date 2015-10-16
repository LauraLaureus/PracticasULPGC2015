using UnityEngine;
using System.Collections;

public class BunnyIA : MonoBehaviour {

	public enum State{
		Moving,
		Resting,
		Evaluating,
		Eating,
		Dying
	}

	public enum BunnyMoveTarget{
		RandomTarget,
		FoodTarget
	}

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMeshAgent;
	private float birthTime=0;

	GameObject food;
	public float fatigue = 0;	
	public float hunger =0;
	public State state;
	public BunnyMoveTarget target;

	public float lifeTime;
	public float hungerBoundary;
	public float fatigueBoundary;


	void Start () {
		rb = GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		target = BunnyMoveTarget.RandomTarget;
		state = State.Evaluating;
		StartCoroutine (FSM ());
		birthTime = Time.time;
		lifeTime = 100;
	}

	IEnumerator FSM(){
		while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Evaluating(){


		if (Time.time > birthTime+lifeTime) {
			Debug.Log("Me muero");
			state = State.Dying;
		
		} else if (hunger >= hungerBoundary) {
			Debug.Log ("Voy a buscar comida");
			target = BunnyMoveTarget.FoodTarget;
			generatePath(target);
			state = State.Moving;
		}else if (fatigue >= fatigueBoundary) {
			Debug.Log ("Estoy cansado. " + fatigueBoundary);
			state = State.Resting;
		} else {
			Debug.Log("Me voy a dar un garbeo por ahí.");
			target = BunnyMoveTarget.RandomTarget;
			generatePath(target);
			state = State.Moving;
		}
		yield return null;
	}

	void generatePath(BunnyMoveTarget targt){
		
		Vector3 destination;
		if (targt == BunnyMoveTarget.FoodTarget) {
			food = getClosest(GameObject.FindGameObjectsWithTag("Fruit"));
			destination = food.transform.position;
		} else {
			destination = getRandomPositionInNavMesh();
		}
		navMeshAgent.SetDestination (destination);
		
	}

	GameObject getClosest(GameObject[] list){
		float[] distances = new float[ list.GetLength(0) ];
		Debug.Log ("Fruits:" + distances);
		for (int i = 0; i < list.GetLength(0); i++) {
			distances[i] = (list[i].transform.position - this.transform.position).sqrMagnitude;
		}
		System.Array.Sort( distances, list);
		return list[0];
	}
	
	
	Vector3 getRandomPositionInNavMesh(){
		Vector3 randomDirection = Random.insideUnitSphere * 5;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition (randomDirection, out hit, 5, 1);
		return hit.position;
	}

	IEnumerator Moving(){
		Vector3 deltaRigidBody = Vector3.one;
		while (deltaRigidBody != Vector3.zero) {
			deltaRigidBody = rb.position;
			Vector3 nextCorner = navMeshAgent.steeringTarget; //steeringTarget es un punto entre el punto actual y el destino o el destino.
			rb.MovePosition (Vector3.MoveTowards (rb.position, nextCorner, navMeshAgent.speed*Time.deltaTime));
			yield return new WaitForEndOfFrame();
			deltaRigidBody -= rb.position;
			fatigue += Vector3.Magnitude(deltaRigidBody);
			hunger += 0.8f*Vector3.Magnitude(deltaRigidBody);
		}
		Debug.Log ("Ya llegué a mi destino");
		if (fatigue >= fatigueBoundary + 5)
			state = State.Dying;
		else if (fatigue > fatigueBoundary)
			state = State.Evaluating;
		else if (target == BunnyMoveTarget.FoodTarget && Vector3.Distance (this.transform.position, food.transform.position) < 2)
			state = State.Eating;
		else
			state = State.Evaluating;

	}


	IEnumerator Resting(){
		float s =Mathf.Abs(Random.value * fatigueBoundary); // un valor entre 0 y 10 s
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


	IEnumerator Eating(){
		if (food != null) {
			Debug.Log ("Ñam, ñam");
			food.GetComponent<FruitBehaviour>().eaten();
			hunger -= 10;
			food = null;

		}

		state = State.Evaluating;
		yield return null;
	}

	IEnumerator Dying(){
		DestroyObject (this.gameObject);
		yield return null;
	}

}
