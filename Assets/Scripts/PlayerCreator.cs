using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {

	public GameObject player;
	
	protected void OnEnable (){
		DungeonGenerator.OnMapCreated += this.CreatePlayer;
	}
	
	protected void CreatePlayer(Door d){
		player.transform.position = new Vector3(d.x_t,0.005f,d.y_t);
		Instantiate (player);
	}
	
	protected void OnDisable(){
		DungeonGenerator.OnMapCreated -= this.CreatePlayer;
	}
}
