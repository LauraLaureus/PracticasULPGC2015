using UnityEngine;
using System.Collections;

public class HeightMapApplicator : MonoBehaviour {

	// TODO crear un evento y pasarle en él el factor y el mapa de celdas al pintor

	private Terrain terrain;
	public int factor;
	public float wallheight;

	void OnEnable(){
		DungeonGenerator.OnMapCreated += apply;
	}

	protected void apply(MapCell[,] map){
		float[,] heights = setUpBounds (map.GetLength (0), map.GetLength (1));
		applyMap (heights, map);

	}

	float[,] setUpBounds(int width,int lenght){
		terrain = this.gameObject.GetComponent<Terrain>();
		terrain.terrainData.size = new Vector3 (width*factor /16f, terrain.terrainData.size.z,lenght*factor/ 16f);
		return new float[ width * factor, lenght * factor];
	}

	void applyMap(float[,]heights, MapCell[,] map){
		for (int i = 0; i < heights.GetLength(0); i++) {
			for (int j =0; j <heights.GetLength(0); j++){
				heights[i,j] = 0f;
			}
		}
		terrain.terrainData.SetHeights (0,0,heights);
	}

	void OnDisable(){
		DungeonGenerator.OnMapCreated -= apply;

	}

}
 