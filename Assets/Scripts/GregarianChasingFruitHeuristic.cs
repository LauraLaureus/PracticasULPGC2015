using UnityEngine;
using System.Collections;

public class GregarianChasingFruitHeuristic : Heuristic<Vector3>{


	public  float apply(Vector3 obj){
		float i = 1;
		for (; i <=10; i++) {
			RaycastHit[] hits = Physics.SphereCastAll (obj, i, Vector3.forward);
			foreach(RaycastHit hit in hits){
				if(hit.collider.tag == "Fruit") return 1/i;
			}
		}
		return 0f;
	}
}
