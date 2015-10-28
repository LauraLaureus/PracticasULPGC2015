using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {

	public GameObject playerPrefab;

	public delegate void PlayerCreated(GameObject player); 
	public static event PlayerCreated OnPlayerCreated;

	protected void OnEnable (){
		HeightMapApplicator.OnTerrainGenerated += this.CreatePlayer;
	}
	
	public void CreatePlayer(Door d){
        GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(d.y_t, 1f, d.x_t), Quaternion.identity);
        player.name = "FPS Player";
        if (OnPlayerCreated != null)
			OnPlayerCreated (player);

	}
	
	protected void OnDisable(){
		HeightMapApplicator.OnTerrainGenerated -= this.CreatePlayer;
	}
}
