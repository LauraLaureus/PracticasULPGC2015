using UnityEngine;
using System.Collections;

public class GregarianBehaviour : MonoBehaviour {

	private Vector3 separation = Vector3.zero;
	private Vector3 cohesion = Vector3.zero;
	private Vector3 aligment = Vector3.zero;
	private Vector3 navigation = Vector3.zero;

	public float w_separation = 0.1f;
	public float w_navigation = 0.9f;
	public float w_cohesion = 1;
	private float w_aligment = 1;


	private Rigidbody rb;
	public NavMeshAgent navMeshAgent;



	//Aquí para hacer la FSM 
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		navMeshAgent = this.gameObject.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 5f, Vector3.forward);

		navigation = calculateNavigationVector ();
		separation = calculateSeparationVector (hits);
		//cohesion = calculateCohesionVector (hits);
		//aligment = calculateAligmentVector(hits);

		Vector3 steeringForce = separation * w_separation + navigation * w_navigation + 0.001f*randomVector();

		rb.velocity = steeringForce.normalized * navMeshAgent.speed;
		Debug.DrawLine (this.transform.position, this.transform.position + rb.velocity);

	}

	Vector3 calculateSeparationVector(RaycastHit [] hits){
		Vector3 result = Vector3.zero;

		//Debug.Log ("Num colliders:" + hits.GetLength (0));

		foreach (RaycastHit h in hits) {

			if (h.collider.gameObject.tag == "Gregarian"){

				Vector3 toGregarian = this.transform.position - h.collider.gameObject.transform.position;
				float towardsMateWeight = toGregarian.magnitude > 0? 1.0f / (toGregarian.magnitude * 0.1f) : 0.001f;
				result += toGregarian.normalized * towardsMateWeight;
			}
		}
		Debug.DrawLine (this.transform.position,this.transform.position+result);
		//Debug.Log (result);
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

	Vector3 calculateNavigationVector(){
		Vector3 result = this.navMeshAgent.steeringTarget;
		Debug.DrawLine (this.transform.position,result);
		return result - this.transform.position;
	}

	Vector3 randomVector(){
		return this.transform.position + this.transform.forward * 3 + Random.insideUnitSphere * 3;
	}
}
