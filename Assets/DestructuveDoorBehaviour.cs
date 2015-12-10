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
		//Debug.Log("flag");
		StartCoroutine (state.ToString ());

	}

	IEnumerator Lift(){
        gameObject.transform.Translate(new Vector3(0, (float)0.1 * Time.fixedDeltaTime, 0));
		if(gameObject.transform.position.y > maxHeight){
            state = DoorStatus.Drop;
            Debug.Log("Vamos para abajo");
		}
		yield return null;
	}

	IEnumerator Drop(){
        gameObject.transform.Translate(new Vector3(0, (float)-0.1 * Time.fixedDeltaTime, 0));
		if(gameObject.transform.position.y <= 0){
            state = DoorStatus.Lift;
            Debug.Log("Vamos para arriba");
		}
		yield return null;
	}
}
