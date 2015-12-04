using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DungeonGeneratorIA : MonoBehaviour
{

    public delegate void MapGenerated(MapCell[,] map, List<Door> doors);
    public static event MapGenerated OnMapCreated;

    public delegate void MapGeneratedForIAs(List<int[]> ds);
    public static event MapGeneratedForIAs OnLiveNeeded;

    public int width;
    public int height;

    Texture2D textureMap;

    public static MapCell[,] map;
    List<Door> doors;
    int diggedAmount = 0;
    List<MinerIA> miners;
    bool minersAlive = false;
    List<int[]> rooms;
    List<int[]> roomsCenters;

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
                UpdateVisualMap(i, j, Color.gray);
            }
        }

        roomsCenters = new List<int[]>();
        miners = new List<MinerIA>();
        for (int i = 0; i < 4; i++)
        {            
            MinerIA miner = gameObject.AddComponent<MinerIA>();
            miner.Configure(width / 2, height / 2, map, this);
            miners.Add(miner);
        }
        for (int i = 0; i < miners.Count; i++)
        {
            int[] pos = miners[i].GetPos();
            UpdateVisualMap(pos[0], pos[1], Color.blue);
        }
        ShowMap();
        DigMap();

    }

    void DigMap()
    {
        minersAlive = true;
        for (int i = 0; i < miners.Count; i++)
        {
            miners[i].StartDigging();
        }
    }

    void StopDigging()
    {
        for (int i = 0; i < miners.Count; i++)
        {
            miners[i].StopAllCoroutines();
        }
        minersAlive = false;
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
                if (map[i, j].cellKind <= MapCell.CellKind.WALL)
                    UpdateVisualMap(i, j, Color.gray);
                else
                    UpdateVisualMap(i, j, Color.white);
            }
        }
    }

    void PaintMiners()
    {
        for (int i = 0; i < miners.Count; i++)
        {
            int[] pos = miners[i].GetPos();
            UpdateVisualMap(pos[0], pos[1], Color.blue);
        }
    }

    public int[][] GetMinersPos()
    {
        int[][] result = new int[miners.Count] [];
        for (int i = 0; i < miners.Count; i++)
        {
            result[i] = miners[i].GetPos();
        }
        return result;
    }

    void CleanMap()
    { //Zonas no pisables
        List<int[]> checkList = new List<int[]>();
        List<int[]> zones = new List<int[]>();
        int currentZone = 2;
        zones.Add(new int[] { currentZone, 0 });
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j].cellKind == MapCell.CellKind.UNUSED)
                {
                    checkList.Add(new int[] { i, j });
                    while (checkList.Count > 0)
                    {
                        int[] position = checkList[0];
                        if (map[position[0], position[1]].cellKind == MapCell.CellKind.UNUSED)
                        {
                            CheckVicinity(checkList, MapCell.CellKind.UNUSED);
                            map[position[0], position[1]].zoneID = currentZone;
                            map[position[0], position[1]].cellKind = MapCell.CellKind.WALL;
                            zones[currentZone - 2][1] = zones[currentZone - 2][1] + 1;
                        }
                        checkList.RemoveAt(0);
                    }
                    if (zones[currentZone - 2][1] > 0)
                    {
                        currentZone++;
                        zones.Add(new int[] { currentZone, 0 });
                    }
                }
            }
        }

        //Buscamos las paredes
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                {
                    map[i, j].isBorder = true;
                }
                else if (map[i, j].cellKind == MapCell.CellKind.WALL)
                {
                    int counter = 0;
                    if (map[i - 1, j].cellKind == MapCell.CellKind.WALL)
                        counter++;
                    if (map[i + 1, j].cellKind == MapCell.CellKind.WALL)
                        counter++;
                    if (map[i, j - 1].cellKind == MapCell.CellKind.WALL)
                        counter++;
                    if (map[i, j + 1].cellKind == MapCell.CellKind.WALL)
                        counter++;
                    if (counter > 0 && counter < 4)
                        map[i, j].isBorder = true;
                }
            }
        }
        //limpieza de lista de zonas: Optimizar añadiendo pos de celda en lista de zonas.
        for (int z = 0; z < zones.Count; z++)
        {
            if (zones[z][1] < 3)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (map[i, j].zoneID == zones[z][0])
                        {
                            map[i, j].cellKind = MapCell.CellKind.WALKABLE;
                            map[i, j].zoneID = 0;
                            UpdateVisualMap(i, j, Color.green);
                        }
                    }
                }
                zones.RemoveAt(z);
                z--;
            }
            else
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (map[i, j].cellKind == MapCell.CellKind.WALL)
                            if (map[i, j].isBorder)
                                UpdateVisualMap(i, j, Color.black);
                            else
                                UpdateVisualMap(i, j, Color.gray);
                    }
                }
            }
        }
    }

    List<int[]> DefineRooms()
    {
        List<int[]> checkList = new List<int[]>();
        List<int[]> zones = new List<int[]>();
        doors = new List<Door>();
        int currentZone = -2;
        zones.Add(new int[] { currentZone, 0 });
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j].cellKind == MapCell.CellKind.WALKABLE && map[i, j].zoneID == 0)
                {
                    checkList.Add(new int[] { i, j });
                    while (checkList.Count > 0)
                    {
                        int[] position = checkList[0];
                        if (map[position[0], position[1]].cellKind == MapCell.CellKind.WALKABLE)
                        {
                            if (CheckIsDoor(position[0], position[1]))
                            {
                                Door door = new Door(position[0], position[1], currentZone, map);
                                doors.Add(door);
                                map[position[0], position[1]].cellKind = MapCell.CellKind.WALKABLE;
                                map[position[0], position[1]].zoneID = currentZone;
                                map[position[0], position[1]].door = door;
                                UpdateVisualMap(position[0], position[1], Color.magenta);
                            }
                            else
                            {
                                CheckVicinity(checkList, MapCell.CellKind.WALKABLE);
                                map[position[0], position[1]].zoneID = currentZone;
                                zones[-currentZone - 2][1] = zones[-currentZone - 2][1] + 1;
                                UpdateVisualMap(position[0], position[1], Color.white);
                            }
                        }
                        checkList.RemoveAt(0);
                    }
                    if (zones[-currentZone - 2][1] > 0)
                    {
                        currentZone--;
                        zones.Add(new int[] { currentZone, 0 });
                    }
                }
            }
        }

        if (zones[zones.Count - 1][1] == 0)
            zones.RemoveAt(zones.Count - 1);

        doors.ForEach(item => item.updateDoorDirection());
        doors.ForEach(item => item.updateDoorZones());
        PaintZones(zones);

        Debug.Log("Zonas: " + (zones.Count - 1));
        Debug.Log("Puertas: " + (doors.Count - 1));

        //Reordenamos las zonas de mas pequeñas a mas grandes
        zones.Sort((z1, z2) => z1[1] < z2[1] ? -1 : 1);
        //Rehacemos las zonas muy pequeñas. //hacer que las puertas den a dos zonas para al eliminar las zonas pequeñas asignarlas a la zona contigua

        while (true)
        {
            int initialDoors = doors.Count;
            for (int z = 0; z < zones.Count; z++)
            {
                int currentZoneLabel = zones[z][0];
                int currentZoneItems = zones[z][1];
                if (currentZoneItems < 5)
                {
                    for (int i = 0; i < doors.Count; i++)
                    {
                        if (doors[i].zoneFrom == currentZoneLabel)
                        {
                            map[doors[i].x, doors[i].y].zoneID = currentZoneLabel;
                            MergeZone(currentZoneLabel, doors[i].zoneTo, zones);
                            doors.ForEach(d => d.zoneFrom = (d.zoneFrom == currentZoneLabel) ? doors[i].zoneTo : d.zoneFrom);
                            doors.ForEach(d => d.zoneTo = (d.zoneTo == currentZoneLabel) ? doors[i].zoneTo : d.zoneTo);
                            UpdateVisualMap(doors[i].x, doors[i].y, Color.yellow);
                            map[doors[i].x, doors[i].y].door = null;
                            doors.RemoveAt(i);

                            i--;
                            break;
                        }
                        else if (doors[i].zoneTo == currentZoneLabel)
                        {
                            map[doors[i].x, doors[i].y].zoneID = currentZoneLabel;
                            MergeZone(currentZoneLabel, doors[i].zoneFrom, zones);
                            doors.ForEach(d => d.zoneFrom = (d.zoneFrom == currentZoneLabel) ? doors[i].zoneFrom : d.zoneFrom);
                            doors.ForEach(d => d.zoneTo = (d.zoneTo == currentZoneLabel) ? doors[i].zoneFrom : d.zoneTo);
                            UpdateVisualMap(doors[i].x, doors[i].y, Color.yellow);
                            map[doors[i].x, doors[i].y].door = null;
                            doors.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    zones.RemoveAt(z);
                    z--;
                }
            }
            if (initialDoors == doors.Count)
                break;
        }

        //eliminamos puertas que dan a la misma zona y las duplicadas
        for (int i = 0; i < doors.Count; i++)
        {
            //puertas repetidas
            for (int j = i + 1; j < doors.Count; j++)
            {
                if (doors[i].compareWith(doors[j]))
                {
                    doors.RemoveAt(j);
                    j--;
                }
            }
            //puertas a la misma zona
            int x = doors[i].x;
            int y = doors[i].y;
            if (doors[i].doorDirection == 0)
            {
                if (map[x - 1, y].zoneID == map[x + 1, y].zoneID)
                {
                    map[x, y].zoneID = map[x - 1, y].zoneID;
                    map[x, y].door = null;
                    doors.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                if (map[x, y - 1].zoneID == map[x, y + 1].zoneID)
                {
                    map[x, y].zoneID = map[x, y - 1].zoneID;
                    map[x, y].door = null;
                    doors.RemoveAt(i);
                    i--;
                }
            }

        }

        Debug.Log("Zonas: " + (zones.Count));
        Debug.Log("Puertas: " + (doors.Count));

        return zones;
    }

    void PaintZones(List<int[]> zones)
    {
        for (int z = 0; z < zones.Count; z++)
        {
            float p = (float)(z + 1) / (float)(zones.Count - 1);
            Color zoneColor = new Color(p * Random.value, p * Random.value, p * Random.value);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j].zoneID == zones[z][0])
                    {
                        UpdateVisualMap(i, j, zoneColor);
                    }
                }
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
            OnLiveNeeded(rooms);
    }

    void traslateDoors()
    {
        for (int i = 0; i<doors.Count; i++)
        {
            doors[i].translateInto(width, height);
        }
    }

    public void AddRoomCenter(int[] roomCenter)
    {
        roomsCenters.Add(roomCenter);
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


    public void CellDigged()
    {
        //repintar la posición pos y updatear
        diggedAmount++;

        if (diggedAmount > width * height * 0.25f)
        {
            StopDigging();

            CleanMap();

            rooms = DefineRooms();

            PaintZones(rooms);

            ShowMap();

            ToolChain();
        }
    }

    public int[] getCenterOfGravity()
    {
        int[] gravityCenter = new int[2] { 0, 0 };
        for (int i = 0; i < miners.Count; i++)
        {
            int[] posMiner = miners[i].GetPos();
            gravityCenter[0] += posMiner[0];
            gravityCenter[1] += posMiner[1];
        }
        gravityCenter[0] = gravityCenter[0] / miners.Count;
        gravityCenter[1] = gravityCenter[1] / miners.Count;
        return gravityCenter;
    }

    void Update()
    {
        if (minersAlive)
        {
            PaintFloor();
            PaintMiners();
            
            ShowMap();
        }

        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel("DungeonTest");

    }


}
