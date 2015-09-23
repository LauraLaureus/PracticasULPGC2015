using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
		
	private GameObject player;
	private Vector3 offset;

	private float distance = 3;
	private float height = 3;


	public void setPlayer(GameObject p){
		this.player = p;

	}

	// Update is called once per frame
	void LateUpdate () {
		if (player != null) {
			Vector3 playerpos = player.transform.position;
			Vector3 offset = -player.transform.forward*distance + player.transform.up*height;
			transform.position = playerpos + offset;
		}
	}
}
