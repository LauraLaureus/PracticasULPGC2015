using UnityEngine;

using System.Collections;
using System;

public class HeightMapApplicator : MonoBehaviour {

	public delegate void TerrainGenerated(Door d);
	public static event TerrainGenerated OnTerrainGenerated;



	private Terrain terrain;
	public float factor;
	public float wallheight;


	void OnEnable(){
		DungeonGeneratorStable.OnMapCreated += apply;
	}

	protected void apply(MapCell[,] map,Door door){

		terrain = this.gameObject.GetComponent<Terrain> ();
		//Debug.Log ("Dimensiones del mapa de alturas:" + terrain.terrainData.heightmapWidth + "x" + terrain.terrainData.heightmapHeight);
		float[,] heights = terrain.terrainData.GetHeights (0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapWidth);
		applyCellMap (heights,map);
		callPlayerCreator (door);
	}

	void applyCellMap(float[,] heights , MapCell[,]map){
	
		for (int i = 0; i < heights.GetLength(0); i++) {
			for(int j = 0; j < heights.GetLength(1); j++){
				heights[i,j] = getSuitableHeightFromMap(map,i,j, heights);
			}
		}
        heights = generateDistanceTransform(heights);
		terrain.terrainData.SetHeights (0, 0, heights);
	}

    float getSuitableHeightFromMap(MapCell[,] map, int indexX, int indexY, float[,] heights){

		int indexXmap = Mathf.Clamp ((int)(indexX / factor), 0, map.GetLength(0)-1);
		int indexYmap = Mathf.Clamp ((int)(indexY / factor), 0, map.GetLength(1)-1);
        if (map[indexXmap, indexYmap].cellKind == MapCell.CellKind.WALKABLE && !map[indexXmap, indexYmap].isBorder)
            return 0f;
        else
            return wallheight;
	}

    private float[,] generateDistanceTransform(float[,] heights)
    {
        for (int i = 1; i < heights.GetLength(0); i++)
        {
            for (int j = 1; j < heights.GetLength(1); j++)
            {

                if (heights[j, i] == 0.0f) continue;
                float min = wallheight;

                for (int x = j - 1; x <= j + 1; x++)
                {
                    if ((x >= 0) && (x < heights.GetLength(1)))
                    {
                        if (heights[x, i - 1] < min)
                            min = heights[x, i - 1];
                    }
                }
                if (heights[j - 1, i] < min)
                    min = heights[j - 1, i];
                if (min < wallheight)
                    heights[j, i] = min + wallheight * 2 / factor;

            }
        }

        for (int i = heights.GetLength(0) - 2; i >= 0; i--)
        {
            for (int j = heights.GetLength(1) - 2; j >= 0; j--)
            {
                if (heights[j, i] == 0) continue;

                float min = heights[j, i];

                for (int x = j - 1; x <= j + 1; x++)
                {
                    if ((x >= 0) && (x < heights.GetLength(1)))
                    {
                        if (heights[x, i + 1] < min)
                            min = heights[x, i + 1];
                    }
                }

                if (heights[j + 1, i] < min)
                    min = heights[j + 1, i];


                if (min < heights[j, i])
                    heights[j, i] = min + wallheight * 2 / factor;

            }
        }
        return heights;
    }

    void callPlayerCreator(Door d){
		Debug.Log ("Puerta:" + d.x_t + " " + d.y_t);
		d.x_t *= terrain.terrainData.heightmapWidth;
		d.y_t *= terrain.terrainData.heightmapHeight;
		Debug.Log ("Puerta:" + d.x_t + " " + d.y_t);
		if (OnTerrainGenerated != null)
			OnTerrainGenerated (d);
	}
	

	void OnDisable(){
        DungeonGeneratorStable.OnMapCreated -= apply;

	}

}
 