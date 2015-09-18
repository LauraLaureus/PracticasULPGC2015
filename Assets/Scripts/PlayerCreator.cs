using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {
	
	protected void OnEnable (){
		DungeonGenerator.OnMapCreated += this.CreatePlayer;
	}
	
	protected void CreatePlayer(Door d){

	}
	
	protected void OnDisable(){
		DungeonGenerator.OnMapCreated -= this.CreatePlayer;
	}
}
