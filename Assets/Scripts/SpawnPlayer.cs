using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlayer : MonoBehaviour {

	public GameObject playerPrefab;

	void OnEnable (){
		DungeonGeneratorMaze.OnLiveNeeded += CreatePlayer;
		Debug.Log ("Creando al jugador");
	}
	
	public void CreatePlayer(List<int[]> roomCenters){
		int[] roomCenter = roomCenters [roomCenters.Count/2];
		GameObject player =(GameObject) Instantiate(playerPrefab, new Vector3(roomCenter[1]*8, 2f, roomCenter[0]*8), Quaternion.identity);
		player.GetComponent<Rigidbody> ().MovePosition (new Vector3(roomCenter [1]*8, 2f, roomCenter [0]*8));
	}
	
	void OnDisable(){
		DungeonGeneratorMaze.OnLiveNeeded -= CreatePlayer;
	}
}
