using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DungeonGeneratorMaze : MonoBehaviour
{

    public delegate void MapGenerated(MapCell[,] map, List<Door> doors);
    public static event MapGenerated OnMapCreated;

    public delegate void MapGeneratedForIAs(List<Door> ds, int w, int h);
    public static event MapGeneratedForIAs OnLiveNeeded;

    public int width = 64;
    public int height = 64;

    //variables para las habitaciones
    public int numberOfRooms = 10;
    int roomsCreated;
    int maxRoomRadius;
    List<int[]> rooms;

    //variabes para los tuneles
    Stack<int[]> posibleCells;

    Texture2D textureMap;

    public static MapCell[,] map;
    List<Door> doors;

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
                    map[i, j].isBorder = true; //caso de bordes
                map[i, j].cellKind = MapCell.CellKind.WALL;
                UpdateVisualMap(i, j, Color.gray);
            }
        }
        ShowMap();

        maxRoomRadius = (int) (width * height / 800);
        roomsCreated = 0;
        rooms = new List<int[]>();
        StartCoroutine(CreateRooms());

    }

    void UpdateVisualMap(int x, int y, Color color)
    {
        textureMap.SetPixel(x, y, color);
    }

    void ShowMap()
    {
        textureMap.Apply();
        GetComponent<Renderer>().material.mainTexture = textureMap;

    }

    void PaintFloor()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j].isBorder)
                    UpdateVisualMap(i, j, Color.black);
                else if (map[i, j].cellKind == MapCell.CellKind.WALL)
                    UpdateVisualMap(i, j, Color.grey);
                else
                    UpdateVisualMap(i, j, Color.white);
            }
        }
    }

    void ToolChain()
    {
        traslateDoors();

        if (OnMapCreated != null)
            OnMapCreated(map, doors);

        //Reciclar para spawners
        if (OnLiveNeeded != null)
            OnLiveNeeded(doors, width, height);
    }

    void traslateDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].translateInto(width, height);
        }
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
                for (int i = center[0] - radius[0] - 1; i <= center[0] + radius[0] + 1; i++)
                {
                    for (int j = center[1] - radius[1] - 1; j<= center[1] + radius[1] + 1; j++)
                    {
                        if (i == center[0] - radius[0] -1
                            || i == center[0] + radius[0] + 1
                            || j == center[1] - radius[1] - 1
                            || j == center[1] + radius[1] + 1
                            )
                            map[i, j].isBorder = true; //caso de bordes
                        else
                            map[i, j].cellKind = MapCell.CellKind.WALKABLE;
                    }
                }
                attempt = 0;
                rooms.Add(center);
                UpdateMap();
                yield return 0;
            }

        }
        Debug.Log("Fin de creacion de habitaciones");
        StartCoroutine(CreateTunels());
    }

    int[] GenerateRandomValues(int startVal1, int EndVal1, int startVal2, int EndVal2)
    {
        int[] result = new int[2];
        result[0] = Random.Range(startVal1, EndVal1);
        result[1] = Random.Range(startVal2, EndVal2);
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
        ScanForFreeZones();
        while (posibleCells.Count > 0)
        {
            int[] cellPos = posibleCells.Pop();

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
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] - 1, cellPos[1] + 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] + 1].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0] - 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] - 1, cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0] - 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] - 1].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }
            if (map[cellPos[0] + 1, cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE
                && map[cellPos[0] + 1, cellPos[1]].cellKind != MapCell.CellKind.WALKABLE
                && map[cellPos[0], cellPos[1] - 1].cellKind != MapCell.CellKind.WALKABLE
                )
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }

            //Si tiene mas de un vecino walkable en las posiciones cardinales, pasar al siguiente (para no unir pasillos/habitaciones)
            int numNeighbors = 0;
            if (map[cellPos[0] + 1, cellPos[1]].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;
            if (map[cellPos[0], cellPos[1] + 1].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;
            if (map[cellPos[0] - 1, cellPos[1]].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;
            if (map[cellPos[0], cellPos[1] - 1].cellKind == MapCell.CellKind.WALKABLE) numNeighbors++;
            if (numNeighbors > 1)
            {
                map[cellPos[0], cellPos[1]].isBorder = true;
                if (posibleCells.Count <= 0) ScanForFreeZones();
                continue;
            }

            //Tratar la celda
            map[cellPos[0], cellPos[1]].cellKind = MapCell.CellKind.WALKABLE;

            // añadir nodos nuevos a 4 posiciones de manera aleatoria
            AddNeighbors(cellPos);

            UpdateMap();
            yield return 0;
        }
        Debug.Log("Fin de creacion de pasillos");
        StartCoroutine(CreateDoors());
    }

    void ScanForFreeZones()
    {
        Debug.Log("Escaneando zonas para pasillos...");
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                if (map[i, j].cellKind == MapCell.CellKind.WALL && !map[i, j].isBorder)
                {
                    Debug.Log("Celda añadida --> " + i + ", " + j);
                    posibleCells.Push(new int[] { i, j });
                    return;
                }
            }
        }
    }

    void AddNeighbors(int[] cellPos)
    {
        if (Random.value < 0.5f)
        {//horizontal first
            if (Random.value < 0.5f)
            {//left first
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
            }
            else
            {//right first
                posibleCells.Push(new int[] { cellPos[0] - 1, cellPos[1] });
                posibleCells.Push(new int[] { cellPos[0] + 1, cellPos[1] });
            }
            if (Random.value < 0.5f)
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
            if (Random.value < 0.5f)
            {//top first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
            }
            else
            {//bot first
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] - 1 });
                posibleCells.Push(new int[] { cellPos[0], cellPos[1] + 1 });
            }
            if (Random.value < 0.5f)
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

    IEnumerator CreateDoors()
    {

        yield return 0;
        StartCoroutine(FixTunels());
    }

    IEnumerator FixTunels()
    {

        yield return 0;
        //ToolChain();
    }

    void MergeZone(int oldZoneLabel, int newZoneLabel, List<int[]> zones)
    {
        int[] oldZone = zones.Find(z => z[0] == oldZoneLabel);
        int[] newZone = zones.Find(z => z[0] == newZoneLabel);
        newZone[1] = newZone[1] + oldZone[1];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j].zoneID == oldZoneLabel)
                    map[i, j].zoneID = newZoneLabel;
            }
        }
    }

    void CheckVicinity(List<int[]> checkList, MapCell.CellKind matchKind)
    {
        int[] position = checkList[0];
        List<int[]> tmpList = new List<int[]>();
        if (position[0] > 0)
            if (map[position[0] - 1, position[1]].cellKind == matchKind && map[position[0], position[1]].zoneID == 0)
                tmpList.Add(new int[] { position[0] - 1, position[1] });
        if (position[0] < width - 1)
            if (map[position[0] + 1, position[1]].cellKind == matchKind && map[position[0], position[1]].zoneID == 0)
                tmpList.Add(new int[] { position[0] + 1, position[1] });
        if (position[1] > 0)
            if (map[position[0], position[1] - 1].cellKind == matchKind && map[position[0], position[1]].zoneID == 0)
                tmpList.Add(new int[] { position[0], position[1] - 1 });
        if (position[1] < height - 1)
            if (map[position[0], position[1] + 1].cellKind == matchKind && map[position[0], position[1]].zoneID == 0)
                tmpList.Add(new int[] { position[0], position[1] + 1 });

        for (int i = 0; i < tmpList.Count; i++)
            if (!checkList.Contains(tmpList[i]))
                checkList.Add(tmpList[i]);
    }

    bool CheckIsDoor(int x, int y)
    {
        if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
            if (((
                map[x - 1, y].cellKind == MapCell.CellKind.WALL &&
                map[x + 1, y].cellKind == MapCell.CellKind.WALL &&
                map[x, y - 1].cellKind == MapCell.CellKind.WALKABLE &&
                map[x, y + 1].cellKind == MapCell.CellKind.WALKABLE) ||
                 (
                map[x - 1, y].cellKind == MapCell.CellKind.WALKABLE &&
                map[x + 1, y].cellKind == MapCell.CellKind.WALKABLE &&
                map[x, y - 1].cellKind == MapCell.CellKind.WALL &&
                map[x, y + 1].cellKind == MapCell.CellKind.WALL)) &&
                (
                map[x, y - 1].door == null &&
                map[x, y + 1].door == null &&
                map[x - 1, y].door == null &&
                map[x + 1, y].door == null))
                return true;
        return false;
    }

    public int[] getCenterOfGravity() //para habitaciones?
    {
        
        int[] gravityCenter = new int[2] { 0, 0 };
        /*
        for (int i = 0; i < miners.Count; i++)
        {
            int[] posMiner = miners[i].GetPos();
            gravityCenter[0] += posMiner[0];
            gravityCenter[1] += posMiner[1];
        }
        gravityCenter[0] = gravityCenter[0] / miners.Count;
        gravityCenter[1] = gravityCenter[1] / miners.Count;
        */
        return gravityCenter;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel("DungeonTest");

    }


}
