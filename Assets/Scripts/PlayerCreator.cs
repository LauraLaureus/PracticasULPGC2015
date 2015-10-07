using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {

	public GameObject player;

	public delegate void PlayerCreated(GameObject player); 
	public static event PlayerCreated OnPlayerCreated;

	protected void OnEnable (){
		HeightMapApplicator.OnTerrainGenerated += this.CreatePlayer;
	}
	
	public void CreatePlayer(Door d){

        player.transform.position = new Vector3(d.y_t, 1f, d.x_t);
        if (OnPlayerCreated != null)
			OnPlayerCreated (player);

	}
	
	protected void OnDisable(){
		HeightMapApplicator.OnTerrainGenerated -= this.CreatePlayer;
	}
}
