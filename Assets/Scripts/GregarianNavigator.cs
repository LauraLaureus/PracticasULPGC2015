using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GregarianNavigator : MonoBehaviour {

	public static Vector3 navigate(GregarianBehaviour.GregarianState state,Vector3 whereIam,Vector3 whereIamLooking){
		Vector3 result = greedyAlgoritm (state, whereIam,whereIamLooking);
		//TODO cuando implemente la memoria de recorrido. Guardar aquí este punto.
		return result;
	}
	
	static Vector3 greedyAlgoritm(GregarianBehaviour.GregarianState state,Vector3 whereIam,Vector3 whereIamLooking){

		List<Vector3> candidates = new List<Vector3>();
		Vector3[] candidatesArray = candidates.ToArray();
		
		while (candidates.Count == 0) {

			//TODO generate todos los puntos que están a 1 metro del objeto y que son alcanzables
			candidates = generatePointsInSight(whereIam,whereIamLooking,1);
			//candidates = generateNCloseRandomPositions (50,whereIam);
			//candidates = removeNonReacheable(candidates,whereIam);
			if(candidates.Count == 0) continue;

			List<float> heuristics = new List<float>();
			switch(state){

			case GregarianBehaviour.GregarianState.ChasingFruit:
			{ 
				heuristics = applyHeuristicToSet(new GregarianChasingFruitHeuristic(),candidates.ToArray());
				break;
			}
			case GregarianBehaviour.GregarianState.Wandering:{
				heuristics = applyHeuristicToSet(new GregarianWanderingHeuristic(),candidates.ToArray());
				break;
			}
			}

			System.Array.Sort(heuristics.ToArray(),candidates.ToArray());
		}
		
		return candidates.ToArray()[0];
	}

	private static List<float> applyHeuristicToSet(Heuristic<Vector3> heuristic,Vector3[] candidates){
		List<float> result = new List<float>();
		foreach(Vector3 candidate in candidates){
			result.Add(heuristic.apply(candidate));
		}
		return result;
	}

	private static List<Vector3> generateNCloseRandomPositions(int n,Vector3 whereIam){
		List<Vector3> result = new List<Vector3>();
		for (int i = 0; i < n; i++) {
			result.Add( Random.insideUnitSphere * 5 + whereIam);
		}
		return result;
	}

	private static List<Vector3> removeNonReacheable(List<Vector3> candidates,Vector3 whereIam){

		for(int i = candidates.Count-1; i >0; i--){
			
			if(Physics.Raycast(whereIam, candidates[i] - whereIam, 2)){
				candidates.Remove(candidates[i]);
				continue;
			}
			
		}

		return candidates;

	}

	private static List<Vector3> generatePointsInSight(Vector3 whereIam,Vector3 whereIamLooking, float distance){
	
		List<Vector3> result = new List<Vector3>();

		float y = whereIam.y;//DEBUGNOTE: sumarle whereIamLooking?? Se supone que nos movemos en el mismo plano.

		//Se puede ir para delante??
		if(Physics.Raycast(whereIam, whereIam+whereIamLooking*distance)){
			//Generar puntos hacia delante

			float z = whereIam.z+(whereIamLooking.z*distance);
			float angleInRadians = 10f/Mathf.PI;

			//Generate 10 candidates
			for(int i = 0; i < 10; i++){
				result.Add(new Vector3(whereIam.x + Random.Range(-angleInRadians,angleInRadians),y,z));
			}

		}

		return result;
	}
}
