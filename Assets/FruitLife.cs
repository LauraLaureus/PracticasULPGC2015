using UnityEngine;
using System.Collections;

public class FruitLife : MonoBehaviour {

	public enum FruitState
	{
		Duplicate,
		DecrementLife
	}

	public float secondsToDuplicate = 5f;
	public int duplicationsToDie = 3;
	private FruitState state;

	// Use this for initialization
	void Start () {
		state = FruitState.Duplicate;
		StartCoroutine("FSM");
	}

	IEnumerator FSM(){
		while (true) //Puede estar mal
			StartCoroutine (state.ToString ());
	}

	IEnumerator Duplicate(){
		yield return new WaitForSeconds (secondsToDuplicate);

	}
	// Update is called once per frame
	void Update () {
	
	}
}
