using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlayer : MonoBehaviour {

	public GameObject playerPrefab;
	/*
	public delegate void PlayerCreated(GameObject player); 
	public static event PlayerCreated OnPlayerCreated;*/

	void OnEnable (){
		DungeonGeneratorMaze.OnLiveNeeded += CreatePlayer;
		Debug.Log ("Creando al jugador");
	}
	
	public void CreatePlayer(List<int[]> roomCenters){
		int[] roomCenter = roomCenters [roomCenters.Count/2];

		Instantiate(playerPrefab, new Vector3(roomCenter[1], 2f, roomCenter[0]), Quaternion.identity);
		
	}
	
	void OnDisable(){
		DungeonGeneratorMaze.OnLiveNeeded -= CreatePlayer;
	}
}
