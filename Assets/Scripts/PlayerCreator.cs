using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {

	public GameObject playerPrefab;
	public CameraController cameraController;

	protected void OnEnable (){
		HeightMapApplicator.OnTerrainGenerated += this.CreatePlayer;
	}
	
	public void CreatePlayer(Door d){
		//Instantiate (playerPrefab);
		GameObject player = (GameObject) GameObject.Instantiate (playerPrefab, new Vector3 (d.y_t, 1f, d.x_t), Quaternion.identity);
		cameraController.setPlayer (player);

	}
	
	protected void OnDisable(){
		HeightMapApplicator.OnTerrainGenerated -= this.CreatePlayer;
	}
}
