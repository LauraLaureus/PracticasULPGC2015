using UnityEngine;
using System.Collections;

public class GregarianBehaviour : MonoBehaviour {

	private Vector3 separation = Vector3.zero;
	private Vector3 cohesion = Vector3.zero;
	private Vector3 aligment = Vector3.zero;
	private Vector3 navigation = Vector3.zero;

	public float w_separation;
	public float w_navigation;
	public float w_inercia;
	public float w_cohesion;
	public float w_aligment = 1;


	private Rigidbody rb;
	public NavMeshAgent navMeshAgent;
	private Vector3 steeringForce;


	//Aquí para hacer la FSM 
	void Start () {
		w_separation = 2f;
		w_navigation = 3f;
		w_inercia = 1f;
		w_cohesion = 1f;
		w_aligment = 1f;
		rb = this.gameObject.GetComponent<Rigidbody> ();
		navMeshAgent = this.gameObject.GetComponent<NavMeshAgent> ();
		steeringForce = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 5f, Vector3.forward);


		steeringForce += w_inercia * steeringForce;
		Debug.DrawLine (this.transform.position,this.transform.position+steeringForce, Color.green);
		//Debug.Log (steeringForce);
		//steeringForce +=  0.001f*randomVector();

		steeringForce += calculateNavigationVector ()* w_navigation;
		steeringForce += calculateSeparationVector (hits) *w_separation;
		//steeringForce += calculateCohesionVector (hits)*w_cohesion;
		steeringForce += calculateAligmentVector(hits) *w_aligment;


		steeringForce = steeringForce.normalized * navMeshAgent.speed; 
		rb.velocity = steeringForce;
		Debug.DrawLine (this.transform.position, this.transform.position + rb.velocity,Color.red);

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
		int count = 0;

		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Gregarian"){

				result += h.collider.gameObject.transform.position;
				count +=1;
			}
		}

		if (count == 0) {
			return Vector3.zero;
		}

		return (result/count) -this.transform.position;
	}

	Vector3 calculateNavigationVector(){
		Vector3 result = this.navMeshAgent.steeringTarget;
		Debug.DrawLine (this.transform.position,result,Color.magenta);
		return result - this.transform.position;
	}

	Vector3 randomVector(){
		return this.transform.position + this.transform.forward + Random.insideUnitSphere * 3;
	}

	Vector3 calculateAligmentVector(RaycastHit[] hits){
		Vector3 result = Vector3.zero;
		int count = 0;
		foreach (RaycastHit h in hits) {
			if (h.collider.gameObject.tag == "Gregarian"){

				result += h.collider.gameObject.transform.forward;
				count +=1;
			}
		}

		if (count == 0) {
			return Vector3.zero;
		}
		Debug.DrawLine(this.transform.position, this.transform.position + (result / count), Color.cyan);
		return result/count;
	}

}
