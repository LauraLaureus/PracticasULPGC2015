using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {
	/*
	public GameObject playerPrefab;
	public CameraController cameraController;

	protected void OnEnable (){
		DungeonGenerator.OnMapCreated += this.CreatePlayer;
	}
	
	protected void CreatePlayer(Door d){
		//Instantiate (player);

		GameObject player = (GameObject) GameObject.Instantiate (playerPrefab, new Vector3 (d.x_t*512, 1f, d.y_t*512), Quaternion.identity);
		cameraController.setPlayer (player);

	}
	
	protected void OnDisable(){
		DungeonGenerator.OnMapCreated -= this.CreatePlayer;
	}*/
}
