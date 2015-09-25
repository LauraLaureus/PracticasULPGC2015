using UnityEngine;
using System.Collections;

public class HeightMapApplicator : MonoBehaviour {

	// TODO crear un evento y pasarle en él el factor y el mapa de celdas al pintor


	public delegate void TerrainGenerated(Door d);
	public static event TerrainGenerated OnTerrainGenerated;

	private Terrain terrain;
	public float factor;
	public float wallheight;
	public PlayerCreator creator;

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
		if (map[indexX,indexY].isBorder || map[indexX,indexY].cellKind == MapCell.CellKind.WALL || map[indexX,indexY].cellKind == MapCell.CellKind.UNUSED)
			return wallheight;
		else
			return 0f;
	}

	void callPlayerCreator(Door d){
		d.x_t *= terrain.terrainData.heightmapWidth;
		d.y_t *= terrain.terrainData.heightmapHeight;
		if (OnTerrainGenerated != null)
			OnTerrainGenerated (d);
	}

	void OnDisable(){
		DungeonGenerator.OnMapCreated -= apply;

	}

}
 