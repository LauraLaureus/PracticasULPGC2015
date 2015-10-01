using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpawnIA : MonoBehaviour {

	private List<Door> doors;
	public GameObject prefab;
	public int numIAsCreated;

	void OnEnable(){
		DungeonGenerator.OnLiveNeeded += generateIA;
	}

	protected void generateIA(List<Door> doorList,int w,int h){
		NavMeshBuilder.BuildNavMesh();
		this.doors = doorList;

		Door d = selectDoor ();
		d.translateInto (w, h);

		GameObject.Instantiate (prefab, new Vector3 (512*d.y_t, 1f, 512*d.x_t), Quaternion.identity);
	}

	Door selectDoor(){
		int index = (int) Random.value * doors.Count;
		return doors [index];
	}
	void OnDisable(){
		DungeonGenerator.OnLiveNeeded -= generateIA;
	}
}
