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
		DungeonGenerator.OnLiveNeeded += generateIA;
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
		// ESTO ES SOLO PARA PROBAR LOS VOIDS
		/*NavMeshHit destination;
		Vector3 CenterOfMass = Vector3.zero;

		foreach (GameObject ia in bunnys) {
			CenterOfMass += ia.transform.position;
		}

		bool result = false;
		do {
			result = NavMesh.SamplePosition (bunnys[0].transform.position,out destination, 5f, NavMesh.AllAreas);
		}while(!result);
		Debug.Log (destination.position);
		Debug.DrawLine (destination.position, destination.position + new Vector3(0,50,0));*/

		foreach (GameObject ia in bunnys) {
			ia.GetComponent<NavMeshAgent>().SetDestination(GameObject.Find("FPS Player").transform.position);
		}
	}



	Door selectDoor(){
		int index = (int) Random.value * doors.Count;
		return doors [index];
	}

	void OnDisable(){
		DungeonGenerator.OnLiveNeeded -= generateIA;
	}
}
