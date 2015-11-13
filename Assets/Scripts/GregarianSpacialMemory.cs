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
		//Debug.Log("No pertenece a la trayectoria");
		return true;
	}

	public void eatenFruit(){
		this.fruitPosition = Vector3.zero;
	}
}
