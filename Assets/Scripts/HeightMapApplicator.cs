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
				heights[i,j] = getSuitableHeightFromMap(map,(int)(i/factor),(int)(j/factor));
			}
		}

		terrain.terrainData.SetHeights (0, 0, heights);
	}

	float getSuitableHeightFromMap(MapCell[,] map, int indexX, int indexY){

		indexX = Mathf.Clamp (indexX, 0, map.GetLength(0)-1);
		indexY = Mathf.Clamp (indexY, 0, map.GetLength(1)-1);
		if (map[indexX,indexY].cellKind == MapCell.CellKind.WALKABLE && !map[indexX,indexY].isBorder )
			return 0f;
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
 