using UnityEngine;

using System.Collections;

public class HeightMapApplicator : MonoBehaviour {

	public delegate void TerrainGenerated(Door d);
	public static event TerrainGenerated OnTerrainGenerated;



	private Terrain terrain;
	public float factor;
	public float wallheight;


	void OnEnable(){
		DungeonGenerator.OnMapCreated += apply;
	}

	protected void apply(MapCell[,] map,Door door){

		terrain = this.gameObject.GetComponent<Terrain> ();
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

		terrain.terrainData.SetHeights (0, 0, heights);
	}

	float getSuitableHeightFromMap(MapCell[,] map, int indexX, int indexY, float[,] heights){

		int indexXmap = Mathf.Clamp ((int)(indexX / factor), 0, map.GetLength(0)-1);
		int indexYmap = Mathf.Clamp ((int)(indexY / factor), 0, map.GetLength(1)-1);
        if (map[indexXmap, indexYmap].cellKind == MapCell.CellKind.WALKABLE && !map[indexXmap, indexYmap].isBorder)
            return 0f;

        else if (indexX > 0 && heights[indexX - 1, indexY] < wallheight)
            return heights[indexX - 1, indexY] + wallheight/factor;
        
        else
            return wallheight;
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
		DungeonGenerator.OnMapCreated -= apply;

	}

}
 