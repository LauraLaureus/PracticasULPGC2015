using UnityEngine;
using System.Collections.Generic;

public class GregarianSpacialMemory : MonoBehaviour {

	private Vector3 fruitPosition;
	private List<Vector3> path;
	private Vector3 lastPointInPath;
	public int pathPointsRetention;

	public void Start(){
		fruitPosition = Vector3.zero;
		path = new List<Vector3> ();
		path.Add (this.gameObject.transform.position);
		pathPointsRetention = 10;
	}

	public void setFruit(GameObject f){
		Debug.Log("Huelo a fruta");
		this.fruitPosition = f.transform.position;
	}

	public Vector3 getFruit(){
		return this.fruitPosition;
	}

	public void setNewPointInPath(Vector3 o){
		if (path.Count > pathPointsRetention)
			path.RemoveAt (path.Count-1);
		lastPointInPath = o;
		path.Add(o);
	}

	public int CountMemories(){
		return path.Count;
	}

	public Vector3 getLastPointIStayed(){
		return path [path.Count-1];
	}

	public bool doIknowWhereFruitIs(){
		return fruitPosition != Vector3.zero;
	}

	public bool canIgoHere(Vector3 currentPosition,Vector3 destination){
		float angle = Vector3.Angle (currentPosition - lastPointInPath, destination - currentPosition);
		//Debug.Log ("Angle is:" + angle.ToString ());
		return  angle < 90F && doesntBelongToPath(destination);
	}

	private bool doesntBelongToPath(Vector3 v){
		foreach (Vector3 point in path) {
			if (v == point) return false;
		}
		return true;
	}

	public void eatenFruit(){
		this.fruitPosition = Vector3.zero;
	}

	public bool isPersitentDestination(){
		int count = 0;
		Vector3 last = path[path.Count-1];
		for (int i = 2; i < path.Count/5; i++) {
			if(path[path.Count -i] == last) count +=1;
		}
		return count > 5;
	}
}
