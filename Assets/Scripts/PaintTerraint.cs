using UnityEngine;
using System.Collections;
using UnityEditor;

public class TerrainPainter : MonoBehaviour
{
	
	private Terrain terrain;
	public int factor;
	
	protected void OnEnable()
	{
		DungeonGenerator.OnMapCreated += this.paintTerrain;
	}
	
	protected void paintTerrain(MapCell[,] map, Door d)
	{
		createSplats();
		float[,,] splatmapData = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];
		terrain = this.gameObject.GetComponent<Terrain>();
		
		float[] splatWallWeights = new float[terrain.terrainData.alphamapLayers];
		splatWallWeights[0] = 1f;
		splatWallWeights[1] =1f;
		float[] splatFloorWeights = new float[terrain.terrainData.alphamapLayers];
		splatFloorWeights[0] = 0f;
		splatFloorWeights[1] = 1f;
		
		for (int i = 0; i < splatmapData.GetLength(0); i++)
		{
			for (int j = 0; j < splatmapData.GetLength(1); j++)
			{
				if ((map[i/factor, j/factor].cellKind == MapCell.CellKind.WALL && map[i/factor, j/factor].isBorder) || map[i / factor, j / factor].cellKind == MapCell.CellKind.UNUSED)
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
					splatmapData[indexY, indexX, j] = splatWeights[j];
				}
			}
		}
		
		return splatmapData;
	}
	
	void createSplats()
	{
		//Creo texturas 2D a partir de una imagen
		Texture2D floorTexture = new Texture2D(2, 2);
		TextAsset image = new TextAsset();
		AssetDatabase.CreateAsset(image, "Materials/wallImage.jpg");
		//wallTexture.LoadImage(image.bytes);
		image = (TextAsset) Resources.Load("Materials/floorImage.jpg") as TextAsset;
		floorTexture.LoadImage(image.bytes);
		
		//Meto las texturas en un SplatPrototype
		SplatPrototype wallSplat = new SplatPrototype();
		//wallSplat.texture = wallTexture;
		wallSplat.tileOffset = new Vector2(0, 0);
		wallSplat.tileSize = new Vector2(15, 15);
		wallSplat.texture.Apply(true);
		
		SplatPrototype floorSplat = new SplatPrototype();
		floorSplat.texture = floorTexture;
		floorSplat.tileOffset = new Vector2(0, 0);
		floorSplat.tileSize = new Vector2(factor, factor);
		floorSplat.texture.Apply(true);
		
		
		terrain.terrainData.splatPrototypes = new SplatPrototype[] { wallSplat, floorSplat };
		
	}
	
	protected void OnDisable()
	{
		DungeonGenerator.OnMapCreated -= this.paintTerrain;
	}
}