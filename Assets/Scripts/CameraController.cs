using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
		
	private GameObject player;
	private Vector3 offset;

	public float distance = 3; 
	public float height = 3;

	void OnEnable(){
		PlayerCreator.OnPlayerCreated += setPlayer;
	}

	public void setPlayer(GameObject p){
		this.player = p;

	}

	// Update is called once per frame
	void LateUpdate () {
		if (player != null) {
			Vector3 playerpos = player.transform.position;
            transform.position = playerpos;
            transform.LookAt(player.transform);
		}
	}

	void OnDisable(){
		PlayerCreator.OnPlayerCreated -= setPlayer;
	}
}
