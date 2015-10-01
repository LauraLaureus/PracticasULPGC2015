using UnityEngine;
using System.Collections;
using UnityEditor;

public class TerrainPainter : MonoBehaviour{
	
	private Terrain terrain;
	public int factor;
	
	protected void OnEnable()
	{
		DungeonGenerator.OnMapCreated += this.paintTerrain;
	}
	
	protected void paintTerrain(MapCell[,] map, Door d)
	{
		terrain = this.gameObject.GetComponent<Terrain>();
		float[,,] splatmapData = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];
		
		float[] splatWallWeights = new float[terrain.terrainData.alphamapLayers];
		splatWallWeights[0] = 0f;
		splatWallWeights[1] = 1f;
		
		float[] splatFloorWeights = new float[terrain.terrainData.alphamapLayers];
		splatFloorWeights[0] = 1f;
		splatFloorWeights[1] = 0f;
		
		for (int i = 0; i < splatmapData.GetLength(0); i++)
		{
			for (int j = 0; j < splatmapData.GetLength(1); j++)
			{
				if (map[i / factor, j / factor].cellKind == MapCell.CellKind.WALL || map[i / factor, j / factor].cellKind == MapCell.CellKind.UNUSED)
				{
					splatmapData = setSplatWeights(i, j, splatWallWeights, splatmapData);
				}
				else
				{
					splatmapData = setSplatWeights(i, j, splatFloorWeights, splatmapData);
				}
			}
		}
		terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
		terrain.Flush();
	}
	
	float[,,] setSplatWeights(int indexX, int indexY, float[] splatWeights, float[,,] splatmapData)
	{
		for (int i = indexX; i < indexX + factor; i++)
		{
			for (int j = indexY; j < indexY + factor; j++)
			{
				for (int k = 0; k < terrain.terrainData.alphamapLayers; k++)
				{
					splatmapData[indexX, indexY, k] = splatWeights[k];
				}
			}
		}
		
		return splatmapData;
	}
	
	protected void OnDisable()
	{
		DungeonGenerator.OnMapCreated -= this.paintTerrain;
	}
	
}