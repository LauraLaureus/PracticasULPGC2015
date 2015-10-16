using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FruitBehaviour : MonoBehaviour {



	public void eaten(){
		GameObject.Find ("SpawnAndGrowFruit").GetComponent<SpawnAndGrowFruit> ().FruitDestroyed (this.gameObject);
		DestroyObject (this.gameObject);
	}
	
}
