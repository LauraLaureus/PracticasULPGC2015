using UnityEngine;
using System.Collections;

public class HeightMapApplicator : MonoBehaviour {

	public Terrain terrain;


	// Use this for initialization
	void Start () {
		int width = (int)terrain.terrainData.heightmapWidth;
		int height = (int)terrain.terrainData.heightmapHeight;
		float[,] heights = new float[width,height];
		for (var j = 0; j < width; j++) {
			for (var i = 0; i < height; i++) {
				//heights [j, i] = Random.Range (0.0f, 1.0f);
				//heights [j, i] = (float)(j*i) /(513f*513f);
				heights[j,i] = 0.0f;
			}
		}
		heights [0, 0] = 1.0f;
		heights [1, 0] = 1.0f;
		heights [0, 1] = 1.0f;
		heights [1, 1] = 1.0f;
		terrain.terrainData.SetHeights (0,0,heights);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
 