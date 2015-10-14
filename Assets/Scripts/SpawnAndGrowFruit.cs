using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpawnAndGrowFruit : MonoBehaviour {

	//DONE hacer que funcione a como el spawn de IA: genere una fruta en una puerta. 
	//DONE hacer que guarde una lista de cada una de las frutas generadas.
	//DONE generar más fruta (este parámetro será público) cada S segundos (público también)
	//TODO las frutas comidas por las IAs deben mandar un evento con su identificador para eliminarlas de la lista.
	//TODO si la lista baja de 5 frutas generar más en otras puertas.

	public enum FruitState
	{
		DuplicatePopulation,
		RemoveOlds
	}

	private List<Door> doors;
	private List<GameObject> fruitsCreated;
	public GameObject prefab;
	public int numFruitCreated;
	public int numFruitRemoved;
	public float secondsToDuplicate;

	private FruitState state;

	void OnEnable(){
		DungeonGenerator.OnLiveNeeded += generateFruit;
		fruitsCreated = new List<GameObject>();
	}

	protected void generateFruit(List<Door> doorList,int w,int h){
		for (int i = 0; i<numFruitCreated; i++) {
			this.doors = doorList;
		
			Door d = selectDoor ();
			this.doors.Remove(d);
			d.translateInto (w, h);
		
			fruitsCreated.Add((GameObject)Instantiate (prefab, new Vector3 (512 * d.y_t, (float)i, 512 * d.x_t), Quaternion.identity));
		}
	}

	void Start(){
		state = FruitState.DuplicatePopulation;
		StartCoroutine ("FSM");
	}

	IEnumerator DuplicatePopulation(){
		int duplicatePopulation = fruitsCreated.Count-1;
		for (int i = 0; i <= duplicatePopulation; i++){
			Vector3 position = fruitsCreated[i].transform.position;
			position.y = 1f;
			fruitsCreated.Add((GameObject)Instantiate (prefab, position , Quaternion.identity));
		}
		yield return new WaitForSeconds(secondsToDuplicate);
		state = FruitState.RemoveOlds;

	}

	IEnumerator FSM(){
		while (true) {
			yield return StartCoroutine(state.ToString());
		}
	}

	IEnumerator RemoveOlds(){
		//Debug.Log ("Eliminamos " + fruitsCreated.Count / 4 + " frutas");
		fruitsCreated.RemoveRange (0, fruitsCreated.Count / 4);
		yield return null;
		state = FruitState.DuplicatePopulation;
	}

	Door selectDoor(){
		int index = (int) Random.value * doors.Count;
		return doors [index];
	}
	void OnDisable(){
		DungeonGenerator.OnLiveNeeded -= generateFruit;
	}
}
