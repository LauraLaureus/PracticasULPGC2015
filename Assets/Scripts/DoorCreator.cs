﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorCreator : MonoBehaviour {

    public GameObject doorPrefab;
    private Terrain terrain;

    void OnEnable()
    {
        DungeonGeneratorIA.OnMapCreated += createDoors;
    }

    void createDoors (MapCell[,] map, List<Door> doors) {
        terrain = this.gameObject.GetComponent<Terrain>();
        for (int i = 0; i<doors.Count; i++)
        {
            doors[i].x_t *= terrain.terrainData.heightmapWidth;
            doors[i].y_t *= terrain.terrainData.heightmapHeight;
            Debug.Log("Puerta " + i + " creada en " + doors[i].x_t + ", " + doors[i].y_t);

            if (doors[i].doorDirection == 1)
                GameObject.Instantiate(doorPrefab, new Vector3(doors[i].y_t, 5f, doors[i].x_t),
                Quaternion.FromToRotation(Vector3.forward, Vector3.right));
            else
                GameObject.Instantiate(doorPrefab, new Vector3(doors[i].y_t, 5f, doors[i].x_t),
                Quaternion.identity);

        }
	}

    void OnDisable()
    {
        DungeonGeneratorIA.OnMapCreated -= createDoors;
    }
}