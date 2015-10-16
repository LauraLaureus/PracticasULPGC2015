using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BunnyIA : MonoBehaviour {

	public enum State{
		Moving,
		Resting,
		Evaluating,
		Eating
	}

	public enum BunnyMoveTarget{
		RandomTarget,
		FoodTarget
	}

	// Use this for initialization
	Rigidbody rb;
	NavMeshAgent navMeshAgent;


	GameObject food;
	public float fatigue = 0;	
	public float hunger =0;
	public State state;
	public BunnyMoveTarget target;
	
	public float hungerBoundary;
	public float fatigueBoundary;


	void Start () {
		rb = GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		target = BunnyMoveTarget.RandomTarget;
		state = State.Evaluating;
		StartCoroutine (FSM ());
		food = getClosest (GameObject.FindGameObjectsWithTag ("Fruit"));
	}

	IEnumerator FSM(){
		while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Evaluating(){

		if (hunger >= hungerBoundary) {
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
		do {
			if (targt == BunnyMoveTarget.FoodTarget) {
				food = getClosest (GameObject.FindGameObjectsWithTag ("Fruit"));
				destination = food.transform.position;
			} else {
				destination = getRandomPositionInNavMesh ();
			}
			navMeshAgent.SetDestination (destination);
			Debug.Log("NavMeshPathStatus: "+ navMeshAgent.path.status);
		} while(navMeshAgent.path.status != NavMeshPathStatus.PathComplete);
		
	}

	GameObject getClosest(GameObject[] list){
		float[] distances = new float[ list.GetLength(0) ];
		for (int i = 0; i < list.GetLength(0); i++) {
			distances[i] = (list[i].transform.position - this.transform.position).sqrMagnitude;
		}
		System.Array.Sort( distances, list);
		return list[0];
	}
	
	
	Vector3 getRandomPositionInNavMesh(){
		List<Vector3> candidates = new List<Vector3>();
		Vector3[] candidatesArray = candidates.ToArray();

		while (candidates.Count == 0) {
			candidates = generateNCloseRandomPositions (5);

			/*Vector3 dir = new Vector3(0,1,0);
			foreach (Vector3 candidate in candidates){
				Debug.DrawRay(candidate,dir);
			}*/

			float[] distances = getDistancesToTarget (candidates);
			NavMeshHit hit;

			Debug.Log("Long lista: " + candidates.Count);
			for(int i = candidates.Count-1; i >0; i--){

				if(Physics.Raycast(this.transform.position, candidates[i] - this.transform.position, 2)){
					candidates.Remove(candidates[i]);
					continue;
				}
					
				//if(!NavMesh.Raycast(this.transform.position, candidates[i],out hit, 1)){

				//}
				if (!navMeshAgent.Raycast (candidates[i],out hit)|| distances[i] < 2) {
					candidates.Remove(candidates[i]);
				}
			}
			Debug.Log("Long lista: " + candidates.Count);
			distances = getDistancesToTarget (candidates);

			candidatesArray = candidates.ToArray();
			System.Array.Sort(distances,candidatesArray);
		}

		return candidatesArray[0];
	}

	List<Vector3> generateNCloseRandomPositions(int n){
		List<Vector3> result = new List<Vector3>();
		for (int i = 0; i < n; i++) {
			result.Add( Random.insideUnitSphere * 5 + transform.position);
		}
		return result;
	}

	float[] getDistancesToTarget(List<Vector3> points){
		float[] distances = new float[points.Count];
		for (int i = 0; i < points.Count; i++) {
			distances [i] = Vector3.Distance(points [i], getClosest(GameObject.FindGameObjectsWithTag("Fruit")).transform.position);
		}
		return distances;
	}

	IEnumerator Moving(){
		Vector3 deltaRigidBody = Vector3.one;
		while (deltaRigidBody != Vector3.zero) {
			deltaRigidBody = rb.position;
			Vector3 nextCorner = navMeshAgent.steeringTarget; //steeringTarget es un punto entre el punto actual y el destino o el destino.
			rb.MovePosition (Vector3.MoveTowards (rb.position, nextCorner, navMeshAgent.speed*Time.deltaTime));
			//Vector3 dir = nextCorner - this.transform.forward;
			//rb.velocity = dir.normalized*navMeshAgent.speed;
			yield return new WaitForEndOfFrame();
			deltaRigidBody -= rb.position;
			Debug.Log("delta de RB: "+ Vector3.Magnitude(deltaRigidBody));
			fatigue += Vector3.Magnitude(deltaRigidBody);
			hunger += 0.8f*Vector3.Magnitude(deltaRigidBody);
		}
	
		Debug.Log ("Ya llegué a mi destino");
		if (fatigue > fatigueBoundary)
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



}
