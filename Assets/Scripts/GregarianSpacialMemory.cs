using UnityEngine;
using System.Collections.Generic;

public class GregarianSpacialMemory : MonoBehaviour {

	private List<GameObject> fruitPosition;
	private List<Vector3> path;
	private Vector3 lastPointInPath;
	public int pathPointsRetention;

	public void OnEnable(){
		fruitPosition = new List<GameObject> ();
		path = new List<Vector3> ();
		path.Add (this.gameObject.transform.position);
		pathPointsRetention = 10;
	}

	public void setFruit(GameObject f){
		Debug.Log("Huelo a fruta");

		if (fruitPosition.Contains (f))
			return;
		fruitPosition.Add (f);
		
	}

	public Vector3 getFruit(){
		return fruitPosition[fruitPosition.Count-1].transform.position;
	}

	public void setNewPointInPath(Vector3 o){
		if (path == null)
			return;
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
		return fruitPosition.Count > 0;
	}

	public bool canIgoHere(Vector3 currentPosition,Vector3 destination){
		float angle = Vector3.Angle (currentPosition - lastPointInPath, destination - currentPosition);
		return  angle < 90F && doesntBelongToPath(destination);
	}

	private bool doesntBelongToPath(Vector3 v){
		foreach (Vector3 point in path) {
			if (v == point) return false;
		}
		return true;
	}

	public void eatenFruit(GameObject f){
		this.fruitPosition.Remove (f);
	}

	public bool isPersitentDestination(){

		if (path == null)
			return false;
		int count = 0;
		Vector3 last = path[path.Count-1];
		for (int i = 2; i < path.Count/5; i++) {
			if(path[path.Count -i] == last) count +=1;
		}
		return count > 5;
	}

    public bool isCloseInPath(Vector3 v) {
        for (int i = 0; i < path.Count / 5; i++)
        {
            if (path[path.Count - i - 1] == v) return true;
        }
        return false;
    }
}
