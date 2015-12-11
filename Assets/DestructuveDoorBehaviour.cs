using UnityEngine;
using System.Collections;

public class DestructuveDoorBehaviour : MonoBehaviour {

	public float secondsToChangeState = 3;
	public float maxHeight = 10;

	private DoorStatus state = DoorStatus.Lift;
	private Rigidbody rb;
    private float f;

	public enum DoorStatus{
		Lift,
		Drop
	}

	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		rb.useGravity = false;
        f = transform.position.y;
	}

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Player" && state == DoorStatus.Drop)
            other.gameObject.GetComponent<Health>().TakeDamage(100);
    }

	void FixedUpdate(){
		//Debug.Log("flag");
		StartCoroutine (state.ToString ());

	}

	IEnumerator Lift(){
        gameObject.transform.Translate(new Vector3(0, (float)0.7* Time.fixedDeltaTime, 0));
		if(gameObject.transform.position.y > maxHeight){
            state = DoorStatus.Drop;
            Debug.Log("Vamos para abajo");
		}
		yield return null;
	}

	IEnumerator Drop(){
        gameObject.transform.Translate(new Vector3(0, (float)-0.7 * Time.fixedDeltaTime, 0));
		if(gameObject.transform.position.y <= f){
            state = DoorStatus.Lift;
            Debug.Log("Vamos para arriba");
		}
		yield return null;
	}
}
