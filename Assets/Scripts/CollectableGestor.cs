using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CollectableGestor : MonoBehaviour {

    int items;
    public int factor = 8;

    public GameObject collectablePrefab;

    void OnEnable()
    {
        PlayerControler.OnCollectablePicked += ItemCollected;
        DungeonGeneratorMaze.OnItemsNeeded += CreateCollectables;
    }

    private void CreateCollectables(MapCell[,] map, int zoneIDmark)
    {
        items = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].zoneID > zoneIDmark)
                {
                    Instantiate(collectablePrefab, new Vector3(j * factor + factor / 2, 1f, i * factor + factor / 2), Quaternion.identity);
                    items++;
                }
            }
        }
    }

    private void ItemCollected()
    {
        items--;
        if (items <= 0)
            Debug.Log("Fin del juego! Victoria!");
        else
            Debug.Log("Faltan " + items + " coleccionables!");
    }

}
