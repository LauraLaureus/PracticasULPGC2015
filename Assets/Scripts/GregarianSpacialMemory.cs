using UnityEngine;
using System.Collections.Generic;

public class GregarianSpacialMemory : MonoBehaviour {

	private Vector3 fruitPosition;
	private Queue<Vector3> path;
	private Vector3 lastPointInPath;
	public int pathPointsRetention;

	public void Start(){
		fruitPosition = Vector3.zero;
		path = new Queue<Vector3> ();
		pathPointsRetention = 50;
	}

	public void setFruit(GameObject f){
		Debug.Log("Huelo a fruta");
		this.fruitPosition = f.transform.position;
	}

	public Vector3 getFruit(){
		return this.fruitPosition;
	}

	public void setNewPointInPath(GameObject o){
		if (path.Count > pathPointsRetention)
			path.Dequeue();
		lastPointInPath = o.transform.position;
		path.Enqueue(o.transform.position);
	}

	public bool doIknowWhereFruitIs(){
		return fruitPosition != Vector3.zero;
	}

	public bool canIgoHere(Vector3 currentPosition,Vector3 destination){
		return true;
	}

	public void eatenFruit(){
		this.fruitPosition = Vector3.zero;
	}
}
