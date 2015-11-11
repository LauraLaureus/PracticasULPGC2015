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
    float[] probabilities;


    enum state
    {
        Birth,
        RandomDig,
        RhombusDig,
        SquareDig,
        TunelDig
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
        probabilities = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
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

    public void StartDigging()
    {
        EditProbabilities(1.0f, 1.0f, 1.0f, 1.0f);
        CompileRulette();
    }

    void CompileRulette()
    {
        //generar código
    }

    void EditProbabilities(float RandomDig, float RhombusDig, float SquareDig, float TunelDig)
    {

    }

    IEnumerator Birth()
    {
        while(currentState == state.Birth)
        {
            yield return 0;
        }
        
    }

    IEnumerator RandomDig()
    {
        while (currentState == state.RandomDig)
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

            DigPosition();
            EditProbabilities(Ran);
            CompileRulette();
            yield return 0;
        }
    }
    
    IEnumerator RhombusDig()
    {
        while (currentState == state.RhombusDig)
        {
            int maxRadius = GenerateRandomRadius(); //controlar que no se sale del mapa

            int actualRadius = 0;
            int[] rhombusCenter = new int[] { pos[0], pos[1] };

            while (actualRadius < maxRadius)
            {
                actualRadius++;
                //diagonal noreste
                pos = new int[] { rhombusCenter[0] + actualRadius, rhombusCenter[1] }; //posicion inicial para giro en espiral
                for (int i=0; i < actualRadius; i++)
                {
                    pos[0] -= 1;
                    pos[1] += 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //diagonal sureste
                pos = new int[] { rhombusCenter[0], rhombusCenter[1] + actualRadius };
                for (int i = 0; i < actualRadius; i++)
                {
                    pos[0] -= 1;
                    pos[1] -= 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //diagonal suroeste
                pos = new int[] { rhombusCenter[0] - actualRadius, rhombusCenter[1] };
                for (int i = 0; i < actualRadius; i++)
                {
                    pos[0] += 1;
                    pos[1] -= 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //diagonal noroeste
                pos = new int[] { rhombusCenter[0], rhombusCenter[1] - actualRadius };
                for (int i = 0; i < actualRadius; i++)
                {
                    pos[0] += 1;
                    pos[1] += 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
            }
            //editar probabilidades
            CompileRulette();
            yield return 0;

        }

    }

    IEnumerator SquareDig()
    {
        yield return 0;
    }

    IEnumerator TunelDig()
    {
        
        while (currentState == state.TunelDig)
        {
            int[] direction = GetRandomDirection();
            int tunelLength = GenerateRandomLength(direction); //controlar que no se salga
            for (; tunelLength > 0; tunelLength--)
            {
                pos[0] += direction[0];
                pos[1] += direction[1];
                if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;
                DigPosition();
                yield return 0;
            }
            //editar probabilidades
            CompileRulette();
        }
    }

    int[] GetRandomDirection()
    {
        int value;
        if (Random.value < 0.5f)
            value = 1;
        else
            value = -1;
        if (Random.value < 0.5f)
            return new int[] { value, 0 };
        else
            return new int[] { 0, value };
    }

    int GenerateRandomLength(int[] direction)
    {
        int randomLength;
        int[] finalPosition;
        do
        {
            randomLength = Random.Range(0, 15);
            finalPosition = new int[2] {pos[0] + direction[0]*randomLength ,pos[1] + direction[1]*randomLength };
        } while ((finalPosition[0] <= 0) || (finalPosition[0] >= mapWidth-1) || (finalPosition[1] >= mapHeight-1) || (finalPosition[1] <= 0));
        Debug.Log("Posición final --> " + finalPosition[0] + ", " + finalPosition[1]);
        return randomLength;
    }

    int GenerateRandomRadius()
    {
        int randomRadius;
        do
        {
            randomRadius = Random.Range(0, 10);
        } while ((randomRadius > mapWidth - pos[0]) || (randomRadius > mapHeight - pos[1]));
        return randomRadius;
    }

    void DigPosition()
    {
        map[pos[0], pos[1]].cellKind = MapCell.CellKind.WALKABLE;
        dungeon.CellDigged();
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

    int[] FindWallHorizontalFirst()
    {
        int[] tempPos = FindInHorizontal();
        if (tempPos != null) return tempPos;
        return FindInVertical();
    }

    int[] FindWallHVerticalFirst()
    {
        int[] tempPos = FindInVertical();
        if (tempPos != null) return tempPos;
        return FindInHorizontal();
    }

    int[] FindInHorizontal()
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
        return null;
    }

    int[] FindInVertical()
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
        return null;
    }

}