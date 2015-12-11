using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class SpawnGregarian : MonoBehaviour
{

    private List<Door> doors;
    public GameObject prefab;
    public int numIAsCreated;

    //private List<GameObject> gregarian;

    void OnEnable()
    {
        DungeonGeneratorStable.OnLiveNeeded += generateIA;
        //gregarian = new List<GameObject>();
    }

    protected void generateIA(List<Door> doorList, int w, int h)
    {
        NavMeshBuilder.BuildNavMesh();
        this.doors = doorList;
        Door d = selectDoor();
        d.translateInto(w, h);

        for (int i = 0; i < numIAsCreated; i++)
        {
           Instantiate(prefab, new Vector3(512 * d.y_t, (float)i, 512 * d.x_t), Quaternion.identity);
        }

    }



    Door selectDoor()
    {
        int index = (int)Random.value * doors.Count;
        return doors[index];
    }

    void OnDisable()
    {
        DungeonGeneratorStable.OnLiveNeeded -= generateIA;
    }
}
