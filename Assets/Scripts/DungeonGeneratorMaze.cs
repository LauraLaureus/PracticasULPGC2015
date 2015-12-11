using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DungeonGeneratorMaze : MonoBehaviour
{

    public delegate void MapGenerated(MapCell[,] map, List<Door> doors);
    public static event MapGenerated OnMapCreated;

    public delegate void MapGeneratedForIAs(List<int[]> ds);
    public static event MapGeneratedForIAs OnLiveNeeded;

    public delegate void MapGeneratedForCollectable(MapCell[,] map, int zoneIDmark);
    public static event MapGeneratedForCollectable OnItemsNeeded;

    public int width = 64;
    public int height = 64;

    //variables para las habitaciones y tuneles
    public int numberOfRooms = 10;
    int roomsCreated;
    int maxRoomRadius;
    List<int[]> rooms;
    Stack<int[]> posibleCells;
    int zoneIDseed = 0;
    int zoneIDmark;
    List<Door> doors;

    Texture2D textureMap;

    public static MapCell[,] map;

    void Start()
    {

        textureMap = new Texture2D(width, height);
        textureMap.filterMode = FilterMode.Point;

        map = new MapCell[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = new MapCell();
                map[i, j].cellKind = MapCell.CellKind.WALL;

                if (i == 0
                            || i == width - 1
                            || j == 0
                            || j == height - 1
                            )
                {
                    map[i, j].isBorder = true; //caso de bordes
                    map[i, j].zoneID = 0;
                }
                UpdateVisualMap(i, j, Color.gray);
            }
        }
        ShowMap();

        maxRoomRadius = (int) (width * height / 800);
        roomsCreated = 0;
        rooms = new List<int[]>();
        doors = new List<Door>();
        StartCoroutine(CreateRooms());

    }

    void UpdateVisualMap(int x, int y, Color color)
    {
        textureMap.SetPixel(x, y, color);
    }

    void ShowMap()
    {
        textureMap.Apply();
        //GetComponent<Renderer>().material.mainTexture = textureMap;

    }

    void PaintFloor()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j].door != null)
                {
                    if (map[i, j].cellKind == MapCell.CellKind.WALKABLE)
                        UpdateVisualMap(i, j, Color.red);
                    else
                        UpdateVisualMap(i, j, Color.yellow);
                }
                else if (map[i, j].isBorder)
                    UpdateVisualMap(i, j, Color.grey);
                else if (map[i, j].cellKind == MapCell.CellKind.WALL)
                    UpdateVisualMap(i, j, Color.black);
                else
                    UpdateVisualMap(i, j, Color.white);
            }
        }
    }

    void ToolChain()
    {

        if (OnMapCreated != null)
            OnMapCreated(map, doors);

		NavMeshBuilder.BuildNavMesh();
        //Reciclar para spawners
        if (OnLiveNeeded != null)
            OnLiveNeeded(rooms);

        if (OnItemsNeeded != null)
            OnItemsNeeded(map, zoneIDmark);
    }

    void UpdateMap()
    {
        PaintFloor();
        ShowMap();
    }

    IEnumerator CreateRooms()
    {
        int attempt = 0;
        while (numberOfRooms > roomsCreated && attempt < 50)
        {
            attempt++;
            int[] radius = GenerateRandomValues(maxRoomRadius / 2, maxRoomRadius + 1, maxRoomRadius / 2, maxRoomRadius + 1);
            int[] center = GenerateRandomValues(0, width, 0, height);

            //comprobar si se puede construir
            if(isUnusedZone(radius, center))
            {

                //construir
                zoneIDseed++;
                for (int i = center[0] - radius[0] - 1; i <= center[0] + radius[0] + 1; i++)
                {
                    for (int j = center[1] - radius[1] - 1; j<= center[1] + radius[1] + 1; j++)
                    {
                        if (i == center[0] - radius[0] -1
                            || i == center[0] + radius[0] + 1
                            || j == center[1] - radius[1] - 1
                            || j == center[1] + radius[1] + 1
                            )
                        {
                            map[i, j].isBorder = true; //caso de bordes
                            map[i, j].cellKind = MapCell.CellKind.WALL;
                            map[i, j].zoneID = 0;
                        }

                        else
                        {
                            map[i, j].zoneID = zoneIDseed;
                            map[i, j].cellKind = MapCell.CellKind.WALKABLE;
                        }
                    }
                }
                roomsCreated++;
                attempt = 0;
                rooms.Add(center);
                UpdateMap();
                //yield return 0;
            }

        }
        zoneIDseed++;
        zoneIDmark = zoneIDseed;
        Debug.Log("Fin de creacion de habitaciones");
        yield return 0;
        StartCoroutine(CreateTunels());
    }

    int[] GenerateRandomValues(int startVal1, int EndVal1, int startVal2, int EndVal2)
    {
        int[] result = new int[2];
        result[0] = UnityEngine.Random.Range(startVal1, EndVal1);
        result[1] = UnityEngine.Random.Range(startVal2, EndVal2);
        return result;
    }

    bool isUnusedZone(int[] radius, int[] center)
    {
        //Check if it fix on the map
        if ((center[0] - radius[0] < 1)
            || (radius[0] + center[0] > width - 2)
            || (center[1] - radius[1] < 1)
            || (radius[1] + center[1] > height - 2)
            )
            return false;

        //Check if it does not invade other room
        for (int i = center[0] - radius[0]; i <= center[0] + radius[0]; i++)
        {
            if (map[i, center[1] + radius[1]].isBorder
                || map[i, center[1] + radius[1]].cellKind == MapCell.CellKind.WALKABLE
                )
                return false;
            if (map[i, center[1] - radius[1]].isBorder
                || map[i, center[1] - radius[1]].cellKind == MapCell.CellKind.WALKABLE
                )
                return false;

        }
        for (int i = center[1] - radius[1]; i <= center[1] + radius[1]; i++)
        {
            if (map[center[0] + radius[0], i].isBorder
                || map[center[0] + radius[0], i].cellKind == MapCell.CellKind.WALKABLE
                )
                return false;
            if (map[center[0] - radius[0], i].isBorder
                || map[center[0] - radius[0], i].cellKind == MapCell.CellKind.WALKABLE
                )
                return false;
        }

        return true;
    }

    IEnumerator CreateTunels()
    {
        posibleCells = new Stack<int[]>();
        int[] cellPos;
        ScanForFreeZones();
        while (posibleCells.Count > 0)
        {
            cellPos = posibleCells.Pop();

            //Analizar si la celda es minable
            if (map[cellPos[0], cellPos[1]].isBorder
                || map[cellPos[0], cellPos[1]].cellKind == MapCell.CellKind.WALKABLE
                )
            {
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }

            //Si sus vecinos diagonales son pasillos o habitaciones (walkable) y los contiguos no, pasar al siguiente
            if (map[cellPos[0] + 1, cellPos[1] + 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0] + 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] + 1].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALL;
                map[cellPos[0], cellPos[1]].zoneID = 0;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] - 1, cellPos[1] + 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] + 1].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0] - 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALL;
                map[cellPos[0], cellPos[1]].zoneID = 0;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] - 1, cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0] - 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] - 1].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALL;
                map[cellPos[0], cellPos[1]].zoneID = 0;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] + 1, cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0] + 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] - 1].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALL;
                map[cellPos[0], cellPos[1]].zoneID = 0;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }

            //Si tiene mas de un vecino walkable en las posiciones cardinales, pasar al siguiente (para no unir pasillos/habitaciones)
            int numNeighbors = 0;
            int[] posOrigin = null;
            if (map[cellPos[0] + 1, cellPos[1]].cellKind == MapCell.CellKind.WALKABLE)
            {
                numNeighbors++;
                posOrigin = new int[2] { cellPos[0] + 1, cellPos[1] };
            }
                
            if (map[cellPos[0], cellPos[1] + 1].cellKind == MapCell.CellKind.WALKABLE)
            {
                numNeighbors++;
                posOrigin = new int[2] { cellPos[0], cellPos[1] + 1 };
            }
            if (map[cellPos[0] - 1, cellPos[1]].cellKind == MapCell.CellKind.WALKABLE)
            {
                numNeighbors++;
                posOrigin = new int[2] { cellPos[0] - 1, cellPos[1] };
            }
            if (map[cellPos[0], cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE)
            {
                numNeighbors++;
                posOrigin = new int[2] { cellPos[0], cellPos[1] - 1 };
            }
            if (numNeighbors > 1)
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                map[cellPos[0], cellPos[1]].zoneID = 0;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }

            //Tratar la celda
            map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALKABLE;
            if (posOrigin != null)
                map[cellPos[0], cellPos[1]].zoneID = map[posOrigin[0], posOrigin[1]].zoneID;
            else
            {
                zoneIDseed++;
                map[cellPos[0], cellPos[1]].zoneID = zoneIDseed;

            }

            // añadir nodos nuevos a 4 posiciones de manera aleatoria
            AddNeighbors(cellPos);

            UpdateMap();
            //yield return 0;
        }
        Debug.Log("Fin de creacion de pasillos");
        yield return 0;
        StartCoroutine(MergeZones());
    }

    void ScanForFreeZones()
    {
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                if (map[i, j].cellKind == MapCell.CellKind.WALL && !map[i, j].isBorder)
                {
                    posibleCells.Push(new int[] { i, j });
                    return;
                }
            }
        }
    }

    void AddNeighbors(int[] cellPos)
    {
        if (UnityEngine.Random.value < 0.5f)
        {//horizontal first
            if (UnityEngine.Random.value < 0.5f)
            {//left first
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
            }
            else
            {//right first
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
            }
            if (UnityEngine.Random.value < 0.5f)
            { //top first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
            }
            else
            { //bot first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
            }
        }
        else
        {//vertical first
            if (UnityEngine.Random.value < 0.5f)
            {//top first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
            }
            else
            {//bot first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
            }
            if (UnityEngine.Random.value < 0.5f)
            {//left first
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
            }
            else
            {//right first
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
            }

        }
    }

    IEnumerator MergeZones()
    {
        ScanForDoors();

        Door door;
        for (int i = 0; i< doors.Count; i++)
        {
            door = doors[i];
            int zoneID1 = door.zoneFrom;
            int zoneID2 = door.zoneTo;
            map[door.x, door.y].cellKind = MapCell.CellKind.WALKABLE;
            map[door.x, door.y].isBorder = false;
            DeleteDoorsBetweenZones(i, zoneID1, zoneID2);
            UpdateMap();
            //yield return new WaitForSeconds(0.3f);
        }
        Debug.Log("Fin de union de habitaciones y pasillos");
        yield return 0;
        StartCoroutine(FixTunels());
    }

    void ScanForDoors()
    {
        for(int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j <height - 1; j++)
            {
                if (map[i, j].isBorder)
                {
                    if (map[i + 1, j].zoneID < zoneIDmark 
                        && map[i + 1, j].zoneID != 0
                        && map[i - 1, j].zoneID > zoneIDmark)
                    {
                        CreateDoor(i, j, i + 1, j);
                    }
                    if (map[i - 1, j].zoneID < zoneIDmark 
                        && map[i - 1, j].zoneID != 0
                        && map[i + 1, j].zoneID > zoneIDmark)
                    {
                        CreateDoor(i, j, i - 1, j);
                    }
                    if (map[i, j + 1].zoneID < zoneIDmark 
                        && map[i, j + 1].zoneID != 0
                        && map[i, j - 1].zoneID > zoneIDmark)
                    {
                        CreateDoor(i, j, i, j + 1);
                    }
                    if (map[i, j - 1].zoneID < zoneIDmark
                        && map[i, j - 1].zoneID != 0
                        && map[i, j + 1].zoneID > zoneIDmark)
                    {
                        CreateDoor(i, j, i, j - 1);
                    }
                }
            }
            UpdateMap();
        }
    }

    void CreateDoor(int i, int j, int fromX, int fromY)
    {
        Door door = new Door(i, j, map[fromX, fromY].zoneID, map);
        door.updateDoorDirection();
        door.updateDoorZones();
        door.translateInto(width, height);
        map[i, j].door = door;
        doors.Add(door);
    }

    void DeleteDoorsBetweenZones(int start, int zoneID1, int zoneID2)
    {
        for (int i = start + 1; i < doors.Count; i++)
        {
            if (doors[i].zoneFrom == zoneID1 && doors[i].zoneTo == zoneID2)
            {
                map[doors[i].x, doors[i].y].door = null;
                doors.RemoveAt(i);
                i--;
            }
        }
    }

    IEnumerator FixTunels()
    {
        List<int[]> deadEnds = FindDeadEnds();
        int[] deadEnd;
        while (deadEnds.Count > 0)
        {
            deadEnd = deadEnds[0];

            //si tiene solo un vecino o menos...
            if (numberOfNeighbors(deadEnd) < 2)
            {
                //tratar celda
                map[deadEnd[0], deadEnd[1]].cellKind = MapCell.CellKind.WALL;
                map[deadEnd[0], deadEnd[1]].isBorder = true;
                map[deadEnd[0], deadEnd[1]].zoneID = 0;
                if (map[deadEnd[0], deadEnd[1]].door != null)
                {
                    doors.Remove(map[deadEnd[0], deadEnd[1]].door);
                    map[deadEnd[0], deadEnd[1]].door = null;
                }

                //Añadir vecinos walkables
                if (map[deadEnd[0] + 1, deadEnd[1]].cellKind == MapCell.CellKind.WALKABLE) deadEnds.Add(new int[] { deadEnd[0] + 1, deadEnd[1] });
                if (map[deadEnd[0] - 1, deadEnd[1]].cellKind == MapCell.CellKind.WALKABLE) deadEnds.Add(new int[] { deadEnd[0] - 1, deadEnd[1] });
                if (map[deadEnd[0], deadEnd[1] + 1].cellKind == MapCell.CellKind.WALKABLE) deadEnds.Add(new int[] { deadEnd[0], deadEnd[1] + 1 });
                if (map[deadEnd[0], deadEnd[1] - 1].cellKind == MapCell.CellKind.WALKABLE) deadEnds.Add(new int[] { deadEnd[0], deadEnd[1] - 1 });
            }
            
            deadEnds.RemoveAt(0);
            UpdateMap();
            //yield return 0;
        }
        Debug.Log("Fin de borrado de pasillos sin salida");
        yield return 0;
        ToolChain();
    }

    int numberOfNeighbors(int[] deadEnd)
    {
        int numNeighbors = 0;

        if (map[deadEnd[0] + 1, deadEnd[1]].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;

        if (map[deadEnd[0], deadEnd[1] + 1].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;
        
        if (map[deadEnd[0] - 1, deadEnd[1]].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;

        if (map[deadEnd[0], deadEnd[1] - 1].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;

        return numNeighbors;
    }

    List<int[]> FindDeadEnds()
    {
        List<int[]> result = new List<int[]>();
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                if (map[i, j].cellKind == MapCell.CellKind.WALKABLE
                    && numberOfNeighbors(new int[] { i, j }) < 2)
                    result.Add(new int[] { i, j });
            }
        }
        return result;
    }

}
