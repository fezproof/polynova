using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMapGenerator
{
	public static float[,] Create (Map map, int seed)
	{
		float[,] noiseMap = new float[map.levelWidth, map.levelHeight];

		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[map.octaves];
		for (int i = 0; i < map.octaves; i++) {
			float offsetX = prng.Next (-100000, 100000);
			float offsetY = prng.Next (-100000, 100000);
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		if (map.scale <= 0) {
			map.scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = map.levelWidth / 2f;
		float halfHeight = map.levelHeight / 2f;

		for (int y = 0; y < map.levelHeight; y++) {
			for (int x = 0; x < map.levelWidth; x++) {
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < map.octaves; i++) {
					float sampleX = (x - halfWidth) / map.scale * frequency + octaveOffsets [i].x;
					float sampleY = (y - halfHeight) / map.scale * frequency + octaveOffsets [i].y;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= map.persistance;
					frequency *= map.lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				noiseMap [x, y] = noiseHeight;

			}
		}

		for (int y = 0; y < map.levelHeight; y++) {
			for (int x = 0; x < map.levelWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
				float i = x / (float)map.levelWidth * 2 - 1;
				float j = y / (float)map.levelHeight * 2 - 1;
				float value = Mathf.Max (Mathf.Abs (i), Mathf.Abs (j));
				noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - Evaluate (value));
				noiseMap [x, y] = map.heightMultiplier.Evaluate (noiseMap [x, y]);
			}
		}

		return noiseMap;

	}

	private static float Evaluate (float value)
	{
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow ((b - b * value), a));
	}
}
