using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinerIA: MonoBehaviour
{
    static int idSeed = 0;

    int id;
    public MapCell[,] map;
    int mapWidth;
    int mapHeight;
    int[] pos;
    List<int[]> posiblePositions = new List<int[]>();
    state currentState;
    DungeonGeneratorIA dungeon;


    enum state
    {
        Birth,
        Dig,
        Relax,
    }

    public void Configure(int x, int y, MapCell[,] map, DungeonGeneratorIA dungeon)
    {
        id = idSeed;
        idSeed++;
        this.map = map;
        mapWidth = map.GetLength(0) - 2;
        mapHeight = map.GetLength(1) - 2;
        pos = new int[2];
        pos[0] = x;
        pos[1] = y;
        currentState = state.Birth;
        this.dungeon = dungeon;
        StartCoroutine(FSM());
    }

    public int[] GetPos()
    {
        return pos;
    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    void ChangeState (state nextState)
    {
        currentState = nextState;
    }

    IEnumerator Birth()
    {
        while(currentState == state.Birth)
        {
            yield return 0;
        }
        
    }

    IEnumerator Relax()
    {
        while(currentState == state.Relax)
        {
            if (Random.value < 0.8f) ChangeState(state.Dig);
            yield return 0;
        }
    }

    public void StartDigging()
    {
        ChangeState(state.Dig);
    }

    IEnumerator Dig()
    {
        while (currentState == state.Dig)
        {
            int[] newPos = RandomMove();
            while (newPos == null)
            {
                if (Random.value < 0.5f)
                    newPos = FindWallHorizontalFirst();
                else
                    newPos = FindWallHVerticalFirst();
            }
            posiblePositions.Clear();
            pos = newPos;
            map[pos[0], pos[1]].cellKind = MapCell.CellKind.WALKABLE;

            dungeon.CellDigged();
            if (Random.value < 0.2f) ChangeState(state.Relax);
            yield return 0;
        }
    }
    public string GetState()
    {
        return currentState.ToString();
    }
    int[] RandomMove()
    {
        int leftLimit = Mathf.Clamp(pos[0] - 1, 1, mapWidth);
        int rightLimit = Mathf.Clamp(pos[0] + 1, 1, mapWidth);
        int topLimit = Mathf.Clamp(pos[1] - 1, 1, mapHeight);
        int bottomLimit = Mathf.Clamp(pos[1] + 1, 1, mapHeight);
        CheckPosition(leftLimit, pos[1]);
        CheckPosition(rightLimit, pos[1]);
        CheckPosition(pos[0], topLimit);
        CheckPosition(pos[0], bottomLimit);
        if (posiblePositions.Count == 0)
            return null;
        if (Random.value < 0.05f)
            return posiblePositions[id % posiblePositions.Count];
        return posiblePositions[Random.Range(0, posiblePositions.Count)];
    }

    void CheckPosition(int x, int y)
    {
        if (x == pos[0] && y == pos[1])
            return;
        int[] p = new int[] { x, y };
        if (map[p[0], p[1]].cellKind <= MapCell.CellKind.WALL && posiblePositions.Find(l => l[0] == p[0] && l[1] == p[1]) == null)
        {
            posiblePositions.Add(p);
        }
    }

    //Mejorar este truño
    int[] FindWallHorizontalFirst()
    {
        int direction = (Random.value < 0.5f) ? -1 : 1;
        int tmpPos = pos[0];
        while (tmpPos < mapWidth && tmpPos > 0)
        {
            if (map[tmpPos, pos[1]].cellKind <= MapCell.CellKind.WALL)
                return new int[] { tmpPos, pos[1] };
            tmpPos += direction;
        }
        tmpPos = pos[0];
        direction = -direction;
        while (tmpPos < mapWidth && tmpPos > 0)
        {
            if (map[tmpPos, pos[1]].cellKind <= MapCell.CellKind.WALL)
                return new int[] { tmpPos, pos[1] };
            tmpPos += direction;
        }
        tmpPos = pos[1];
        while (tmpPos < mapHeight && tmpPos > 0)
        {
            if (map[pos[0], tmpPos].cellKind <= MapCell.CellKind.WALL)
                return new int[] { pos[0], tmpPos };
            tmpPos += direction;
        }
        tmpPos = pos[1];
        direction = -direction;
        while (tmpPos < mapHeight && tmpPos > 0)
        {
            if (map[pos[0], tmpPos].cellKind <= MapCell.CellKind.WALL)
                return new int[] { pos[0], tmpPos };
            tmpPos += direction;
        }
        return null;
    }

    int[] FindWallHVerticalFirst()
    {
        int direction = (Random.value < 0.5f) ? -1 : 1;
        int tmpPos = pos[1];
        while (tmpPos < mapHeight && tmpPos > 0)
        {
            if (map[pos[0], tmpPos].cellKind <= MapCell.CellKind.WALL)
                return new int[] { pos[0], tmpPos };
            tmpPos += direction;
        }
        tmpPos = pos[1];
        direction = -direction;
        while (tmpPos < mapHeight && tmpPos > 0)
        {
            if (map[pos[0], tmpPos].cellKind <= MapCell.CellKind.WALL)
                return new int[] { pos[0], tmpPos };
            tmpPos += direction;
        }
        tmpPos = pos[0];
        while (tmpPos < mapWidth && tmpPos > 0)
        {
            if (map[tmpPos, pos[1]].cellKind <= MapCell.CellKind.WALL)
                return new int[] { tmpPos, pos[1] };
            tmpPos += direction;
        }
        tmpPos = pos[0];
        direction = -direction;
        while (tmpPos < mapWidth && tmpPos > 0)
        {
            if (map[tmpPos, pos[1]].cellKind <= MapCell.CellKind.WALL)
                return new int[] { tmpPos, pos[1] };
            tmpPos += direction;
        }
        return null;
    }

}