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
        RandomDig,
        TunelDig,
        RhombusDig,
        SquareDig
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
        this.dungeon = dungeon;
        probabilities = new float[4] { 10.0f, 3.0f, 3.0f, 1.0f };
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
        CompileRulette();
        StartCoroutine(FSM());
    }

    void CompileRulette()
    {
        float total = 0;
        int i;
        for (i = 0; i < probabilities.Length; i++) total += probabilities[i];
        float winner = Random.Range(0, total);

        float sum = 0;
        for (i = 0; i < probabilities.Length; i++)
        {
            sum += probabilities[i];
            if (sum > winner)
                break;
        }
        ChangeState((state)i);
        //ChangeState(state.RhombusDig);
    }

    void EditProbabilities(state stateCaller)
    {
        
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
            //EditProbabilities();
            CompileRulette();
            yield return 0;
        }
    }
    
    IEnumerator RhombusDig()
    {
        while (currentState == state.RhombusDig)
        {
            int maxRadius = GenerateRandomRadius();

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
        while (currentState == state.SquareDig)
        {
            int maxRadius = GenerateRandomRadius();

            int actualRadius = 0;
            int[] squareCenter = new int[] { pos[0], pos[1] };

            while (actualRadius < maxRadius)
            {
                actualRadius++;
                //cara norte
                pos = new int[] { squareCenter[0] - actualRadius, squareCenter[1] - actualRadius }; //posicion inicial para giro en espiral
                for (int i = 0; i < actualRadius*2; i++)
                {
                    pos[0] += 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //cara este
                pos = new int[] { squareCenter[0] + actualRadius, squareCenter[1] - actualRadius };
                for (int i = 0; i < actualRadius*2; i++)
                {
                    pos[1] += 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //cara sur
                pos = new int[] { squareCenter[0] + actualRadius, squareCenter[1] + actualRadius };
                for (int i = 0; i < actualRadius*2; i++)
                {
                    pos[0] -= 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
                //cara oeste
                pos = new int[] { squareCenter[0] - actualRadius, squareCenter[1] + actualRadius };
                for (int i = 0; i < actualRadius*2; i++)
                {
                    pos[1] -= 1;

                    if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;

                    DigPosition();
                    yield return 0;
                }
            }
            CompileRulette();
            yield return 0;
        }
    }

    IEnumerator TunelDig()
    {
        while (currentState == state.TunelDig)
        {
            int[] direction = GetRandomDirection();
            int tunelLength = GenerateRandomLength(direction); //controlar que no se salga
            for (int i = 0; i< tunelLength ; i++)
            {
                pos[0] += direction[0];
                pos[1] += direction[1];
                if (map[pos[0], pos[1]].cellKind == MapCell.CellKind.WALKABLE) continue;
                DigPosition();
                yield return 0;
            }
            //editar probabilidades
            CompileRulette();
            yield return 0;
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
            randomLength = Random.Range(0, 20);
            finalPosition = new int[2] {pos[0] + direction[0]*randomLength ,pos[1] + direction[1]*randomLength };
        } while ((finalPosition[0] < 1) || (finalPosition[0] > mapWidth) || (finalPosition[1] > mapHeight) || (finalPosition[1] < 1));
        return randomLength;
    }

    int GenerateRandomRadius()
    {
        int randomRadius;
        do
        {
            randomRadius = Random.Range(0, 4);
        } while ((randomRadius+pos[0] > mapWidth ) || (pos[0] - randomRadius < 1) || ((pos[1] - randomRadius < 1)) ||  (randomRadius+pos[1] > mapHeight));
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
        while (tmpPos < mapWidth-1 && tmpPos > 0)
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