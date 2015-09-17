using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	protected void OnEnable (){
		DungeonGenerator.OnMapCreated += CreateUnit ();
	}

	virtual protected void CreateUnit(){

	}

	protected void OnDisable(){
		DungeonGenerator.OnMapCreated -= CreateUnit ();
	}
}
