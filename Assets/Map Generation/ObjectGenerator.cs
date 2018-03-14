using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
	Map map;
	int width;
	int height;

	public List<GameObject> GenerateObjects (Map map, float[,] noiseMap)
	{
		this.map = map;
		width = map.levelWidth;
		height = map.levelHeight;

		GameObject terrainParent = new GameObject ("Terrain");
		terrainParent.isStatic = true;
		terrainParent.transform.parent = this.transform;

		int totalWeight = 0;
		for (int i = 0; i < map.possibleObjects.Length; i++) {
			totalWeight += map.possibleObjects [i].chance;
		}

		int current;

		List<GameObject> objects = new List<GameObject> ();

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (map.objectChance > UnityEngine.Random.Range (0f, 1f)) {
					current = 0;
					for (int i = 0; i < map.possibleObjects.Length; i++) {
						if (noiseMap [x, y] >= map.possibleObjects [i].height) {
							current += map.possibleObjects [i].chance;
							if (current > UnityEngine.Random.Range (0, totalWeight + 1)) {
								objects.Add (PlaceObject (map.possibleObjects [i].terrain, x, y, terrainParent.transform));
								break;
							}
						}
					}
				}
			}
		}

		return objects;
	}

	GameObject PlaceObject (GameObject obj, int x, int y, Transform parent)
	{
		Quaternion rotation = Quaternion.identity;
		rotation.eulerAngles = new Vector3 (0f, UnityEngine.Random.Range (0f, 360f), 0f);

		GameObject terrain = Instantiate (obj, new Vector3 ((-width / 2f + x + 0.5f) * map.tileSize, 0, (-height / 2f + y + 0.5f) * map.tileSize), rotation, parent);

		float scale = UnityEngine.Random.Range (0.9f, 1.1f);
		terrain.transform.localScale = new Vector3 (scale, scale, scale);

		return terrain;
	}
}
