using UnityEngine;
using System.Collections;

public class GregarianBehaviour : MonoBehaviour {

	private Vector3 separation = Vector3.zero;
	private Vector3 cohesion = Vector3.zero;
	private Vector3 aligment = Vector3.zero;

	public float w_separation = 1;
	public float w_cohesion = 1;
	private float w_aligment = 1;

	private Rigidbody rb;
	private NavMeshAgent navMeshAgent;

	//Aquí para hacer la FSM 
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		navMeshAgent = this.gameObject.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 5f, Vector3.forward);

		separation = calculateSeparationVector (hits);
		cohesion = calculateCohesionVector (hits);
		//aligment = calculateAligmentVector(hits);

		Vector3 steeringForce = separation * w_separation + cohesion * w_cohesion;

		rb.velocity = steeringForce.normalized;

		//rb.AddForce (steeringForce.normalized * navMeshAgent.speed);
	}

	Vector3 calculateSeparationVector(RaycastHit [] hits){
		Vector3 result = Vector3.zero;

		//Debug.Log ("Num colliders:" + hits.GetLength (0));

		foreach (RaycastHit h in hits) {

			if (h.collider.gameObject.tag == "Gregarian"){

				Vector3 toGregarian = this.transform.position - h.collider.gameObject.transform.position;
				//Vector3 toGregarian = h.collider.gameObject.transform.position -this.transform.position;
				result += toGregarian.normalized/Vector3.Distance(this.transform.position,h.collider.gameObject.transform.position);
			}
		}
		Debug.Log (result);
		return result;
	}

	Vector3 calculateCohesionVector(RaycastHit[] hits){

		Vector3 result = Vector3.zero;
		int count = 0;

		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Gregarian"){

				result += h.collider.gameObject.transform.position;
				count +=1;
			}
		}

		return result/count;
	}
}
