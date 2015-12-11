using UnityEngine;

using System.Collections;
using System.Collections.Generic;


public class SpawnPointIA : MonoBehaviour {

    
    public int numIAs =4;
    public GameObject prefab;
    public float secondsToRespawn;

    private List<int[]> roomCenters;

    void OnEnable()
    {
        DungeonGeneratorMaze.OnLiveNeeded += generateGregarian;
        numIAs = 4;
    }

    void Start() {
        StartCoroutine("FSM");
    }

    IEnumerator FSM() {
        while (true) {
            generateGregarian(null);
            yield return new WaitForSeconds(480);
        }
    }


    public void generateGregarian(List<int[]> roomCenters){

        if (this.roomCenters == null && roomCenters == null) return;
        else if (this.roomCenters == null)
        {
            this.roomCenters = roomCenters;
        }
        else {
            roomCenters = this.roomCenters;
        }

        //NOTA quito una posición que es la del índice 1 para poner en esa posición la fruta.
        int index = (int)(Random.value * roomCenters.Count-1)+1;
        int[] chosenCenter = roomCenters[index];
        float factor = 8;
        
        for (int i = 0; i < chosenCenter.Length; i++) {
            chosenCenter[i] *= (int)factor;
        }

        for (int i = 0; i < numIAs; i++) {
            Instantiate(prefab, new Vector3(chosenCenter[1]+Random.value*2, 2f, chosenCenter[0]+Random.value*2), Quaternion.identity);
        }
    }


    void OnDisable()
    {
        DungeonGeneratorMaze.OnLiveNeeded -= generateGregarian;
    }
}
