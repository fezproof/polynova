using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

[CreateAssetMenu (menuName = "Game/Map Generation/Map")]
public class Map : ScriptableObject
{
	[Range (30, 100)]public int levelWidth;
	[Range (30, 100)]public int levelHeight;
	[Range (-5f, 10f)]public float wallHeight;
	[Range (0.1f, 1f)]public float wallLimit = 0.1f;
	[Range (1f, 10f)] public float tileSize;


	[Space (10)]
	[Header ("Randomness options")]
	public float scale;
	[Range (1, 8)]public int octaves;
	[Range (0f, 4f)]public float lacunarity;
	[Range (0f, 1f)]public float persistance;
	public AnimationCurve heightMultiplier = AnimationCurve.Linear (0f, 0f, 1f, 1f);

	public Material wallMaterial;

	public TileInfo[] possibleTerrain;

	[Range (0f, 1f)]public float objectChance = 0.3f;
	public TerrainInfo[] possibleObjects;

	public LightInfo lightOptions;

	public PostProcessingProfile postProcProfile;

	[System.Serializable]
	public struct TileInfo
	{
		public Color terrainColour;
		public Material terrainMaterial;
		[RangeAttribute (0, 1)]
		public float height;
	}

	[System.Serializable]
	public struct TerrainInfo
	{
		public GameObject terrain;
		[RangeAttribute (0, 100)]public int chance;
		[RangeAttribute (0f, 1f)]public float height;
	}

	[System.Serializable]
	public struct LightInfo
	{
		public Vector3 angle;
		[RangeAttribute (0f, 1f)]public float intensity;
		public Color lightColour;
	}

}
