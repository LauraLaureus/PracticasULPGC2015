using UnityEngine;
using System.Collections;

public class GregarianWanderingHeuristic : Heuristic<Vector3> {

	public float apply(Vector3 obj){
		return Random.Range (0f, 1f);
	}

}
