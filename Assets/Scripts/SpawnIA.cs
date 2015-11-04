using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpawnIA : MonoBehaviour {

	private List<Door> doors;
	public GameObject prefab;
	public int numIAsCreated;

	private List<GameObject> bunnys;

	void OnEnable(){
		DungeonGeneratorStable.OnLiveNeeded += generateIA;
		bunnys = new List<GameObject>();
	}

	protected void generateIA(List<Door> doorList,int w,int h){
		NavMeshBuilder.BuildNavMesh();

		for (int i = 0; i<numIAsCreated; i++) {
			this.doors = doorList;
			Door d = selectDoor ();
			d.translateInto (w, h);
			this.doors.Remove(d);
			bunnys.Add((GameObject)Instantiate (prefab, new Vector3 (512 * d.y_t, (float)i, 512 * d.x_t), Quaternion.identity));
		}
	

		foreach (GameObject ia in bunnys) {
			ia.GetComponent<NavMeshAgent>().SetDestination(GameObject.Find("Bunny 2.0(Clone)").transform.position);
		}
	}



	Door selectDoor(){
		int index = (int) Random.value * doors.Count;
		return doors [index];
	}

	void OnDisable(){
		DungeonGeneratorStable.OnLiveNeeded -= generateIA;
	}
}
