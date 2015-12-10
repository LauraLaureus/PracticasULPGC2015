using UnityEngine;
using System.Collections;

public class DestructuveDoorBehaviour : MonoBehaviour {

	public float secondsToChangeState = 3;
	public float maxHeight = 10;

	private DoorStatus state = DoorStatus.Lift;
	private Rigidbody rb;

	public enum DoorStatus{
		Lift,
		Drop
	}

	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		rb.useGravity = false;
	}


	void FixedUpdate(){
		Debug.Log("flag");
		StartCoroutine (state.ToString ());
	}

	IEnumerator Lift(){
		while(gameObject.transform.position.y < maxHeight){
			gameObject.transform.Translate(new Vector3(0,(float)0.1*Time.fixedDeltaTime,0));
		}
		state = DoorStatus.Drop;
		yield return new WaitForSeconds(secondsToChangeState);
	}

	IEnumerator Drop(){
		while(gameObject.transform.position.y > 0){
			gameObject.transform.Translate(new Vector3(0,(float)-0.1*Time.fixedDeltaTime,0));
		}
		state = DoorStatus.Lift;
		yield return new WaitForSeconds(secondsToChangeState);
	}
}
