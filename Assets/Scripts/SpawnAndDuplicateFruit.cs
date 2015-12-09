using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnAndDuplicateFruit : MonoBehaviour {

	public int numIAs =4;
	public GameObject prefab;
	public float secondsToDuplicate;

	private List<GameObject> fruitsCreated;
	private FruitState state;

	public enum FruitState
	{
		DuplicatePopulation,
		RemoveOlds
	}



	void OnEnable()
	{
		DungeonGeneratorMaze.OnLiveNeeded += generateFruit;
		numIAs = 4;
		fruitsCreated = new List<GameObject> ();
	}

	public void generateFruit(List<int[]> roomCenters){
		int[] roomCenter = roomCenters [0];

		for (int i = 0; i < numIAs; i++) {
			fruitsCreated.Add((GameObject)Instantiate (prefab, new Vector3 (8*roomCenter[1], (float)i, 8*roomCenter[0]), Quaternion.identity));
		}
		state = FruitState.DuplicatePopulation;
		StartCoroutine ("FSM");
	}

	IEnumerator FSM(){
		while (true) {
			yield return StartCoroutine(state.ToString());
		}
	}
	

 	IEnumerator DuplicatePopulation(){
		int duplicatePopulation = fruitsCreated.Count-1;
		for (int i = 0; i <= duplicatePopulation; i++){
			Vector3 position = fruitsCreated[i].transform.position;
			position.y = 1f;
			fruitsCreated.Add((GameObject)Instantiate (prefab, position , Quaternion.identity));
		}
		yield return new WaitForSeconds(secondsToDuplicate);
		
	}

	public void FruitDestroyed (GameObject fruit){
		fruitsCreated.Remove(fruit);
	}

	void OnDisable()
	{
		DungeonGeneratorMaze.OnLiveNeeded -= generateFruit;
	}
}
