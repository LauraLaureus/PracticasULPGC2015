using UnityEngine;
using System.Collections;

public class DestructuveDoorBehaviour : MonoBehaviour {

	public float secondsToChangeState = 1;
	public float maxHeight = 10;

	private DoorStatus state = DoorStatus.Lift;
	private Rigidbody rb;
	private float f;

	public enum DoorStatus{
		Lift,
		Drop
	}

	void Start () {
		f = transform.position.y;
		rb = gameObject.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		StartCoroutine("FSM");
	}

	IEnumerator FSM(){
		while (true) {
			StartCoroutine (state.ToString ());
		}
	}

	IEnumerator Lift(){
		Debug.Log ("flag");
		rb.useGravity = false;
		while(gameObject.transform.position.y < maxHeight){
			gameObject.transform.Translate(0,1*Time.deltaTime,0);
		}
		state = DoorStatus.Drop;
		yield return new WaitForSeconds(secondsToChangeState);


	}

	IEnumerator Drop(){
		rb.useGravity = true;
		RaycastHit hit;
		Physics.Raycast (transform.position, -transform.up,out hit);
		while ((hit.point - transform.position).magnitude <0.01f) {
			Physics.Raycast (transform.position, -transform.up,out hit);
		}
		state = DoorStatus.Lift;
		yield return new WaitForSeconds(secondsToChangeState);
	}
}
