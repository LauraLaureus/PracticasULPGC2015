using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	GameObject player;

	private Vector3 offset;

	void onEnable(){
		PlayerCreator.onFollow += iFollow;
	}

	void iFollow(GameObject player){
		this.player = player;
		offset = transform.position - player.transform.position;
		Debug.Log ("Called");
	}

	void onDisable(){
		PlayerCreator.onFollow -= iFollow;
	}


	// Update is called once per frame
	void LateUpdate () {
		if (offset.Equals(null))
			transform.position = player.transform.position + offset;
	}
}
