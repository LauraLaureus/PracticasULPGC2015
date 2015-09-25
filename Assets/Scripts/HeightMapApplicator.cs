using UnityEngine;
using System.Collections;

public class HeightMapApplicator : MonoBehaviour {

	// TODO crear un evento y pasarle en él el factor y el mapa de celdas al pintor
	//TODO meter al jugador.

	private Terrain terrain;
	public float factor;
	public float wallheight;
	public PlayerCreator creator;

	void OnEnable(){
		DungeonGenerator.OnMapCreated += apply;
	}

	protected void apply(MapCell[,] map){
		terrain = this.gameObject.GetComponent<Terrain> ();
		float[,] heights = terrain.terrainData.GetHeights (0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapWidth);
		//Debug.Log ("Dimensiones dungeon:" + map.GetLength (0) + "x" + map.GetLength (1));
		//Debug.Log ("Dimensiones heightMap:" + heights.GetLength (0) + "x" + heights.GetLength (1));
		//Debug.Log ("Factor:" + factor);
		applyCellMap (heights,map);

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
		//Debug.Log ("x:" + indexX + "y:" + indexY);
		indexX = Mathf.Clamp (indexX, 0, map.GetLength(0)-1);
		indexY = Mathf.Clamp (indexY, 0, map.GetLength(1)-1);
		if (map[indexX,indexY].isBorder || map[indexX,indexY].cellKind == MapCell.CellKind.WALL || map[indexX,indexY].cellKind == MapCell.CellKind.UNUSED)
			return wallheight;
		else
			return 0f;
	}



	void OnDisable(){
		DungeonGenerator.OnMapCreated -= apply;

	}

}
 