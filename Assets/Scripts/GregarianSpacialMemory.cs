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
        deleteNullObjects();
        if (fruitPosition[fruitPosition.Count - 1] == null) return path[0];
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
    

    public Vector3 getLastPointIStayed(){
		return path [path.Count-1];
	}

	public bool doIknowWhereFruitIs(){
        deleteNullObjects();
		return fruitPosition.Count > 0;
	}
   
	public void eatenFruit(GameObject f){
		this.fruitPosition.Remove (f);
	}
   

    public void deleteNullObjects() {
        if (fruitPosition.Count == 0) return;
		Debug.Log ("Antes de eliminar había:" + fruitPosition.Count.ToString ());
        for (int i = fruitPosition.Count - 1; i >= 0; i--) {
            if (fruitPosition[i] == null) {
				Debug.Log ("Se eliminó algo");
                fruitPosition.RemoveAt(i);
            }
        }
		Debug.Log ("Después de eliminar había:" + fruitPosition.Count.ToString ());
    }
}
