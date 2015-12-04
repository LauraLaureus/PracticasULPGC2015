using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class SpawnPointIA : MonoBehaviour {

    
    public int numIAs =4;
    public GameObject prefab;

    void OnEnable()
    {
        DungeonGeneratorMaze.OnLiveNeeded += generateGregarian;
        numIAs = 4;
    }

    public void generateGregarian(List<int[]> roomCenters){

        //NOTA quito una posición que es la del índice 1 para poner en esa posición la fruta.
        int index = (int)(Random.value * roomCenters.Count-1)+1;
        int[] chosenCenter = roomCenters[index];
        //float factor = gameObject.GetComponent<HeightMapApplicatorIA>().factor;
        gameObject.GetComponent<HeightMapApplicatorIA>();
        float factor = 8;
        
        for (int i = 0; i < chosenCenter.Length; i++) {
            chosenCenter[i] *= (int)factor;
        }

        for (int i = 0; i < numIAs; i++) {
            Instantiate(prefab, new Vector3(chosenCenter[1], 1f, chosenCenter[0]), Quaternion.identity);
        }
    }


    void OnDisable()
    {
        DungeonGeneratorMaze.OnLiveNeeded -= generateGregarian;
    }
}
