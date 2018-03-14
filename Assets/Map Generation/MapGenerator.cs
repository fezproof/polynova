using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent (typeof(MSMeshGenerator))]
[RequireComponent (typeof(ObjectGenerator))]
[RequireComponent (typeof(NavMeshGenerator))]
public class MapGenerator : MonoBehaviour
{

	private Map map;

	int width;
	int height;

	//	[SyncVar (hook = "onSeedChange")]
	private int seed = 0;
	//	[SyncVar]
	private bool clientReceivedSeed = false;
	//	public bool useRandomSeed;

	int[,] wallMap;
	float[,] noiseMap;

	public void Awake ()
	{
		LevelManager.instance.mapGenerator = this;
	}

	public void Initialise (Map map, int seed)
	{
		this.map = map;

		width = map.levelWidth;
		height = map.levelHeight;

		noiseMap = NoiseMapGenerator.Create (map, seed);

		wallMap = new int[width, height];

		CreateWallMap ();

		ProcessMap ();

		MSMeshGenerator MSGen = GetComponent<MSMeshGenerator> ();
		Material[] mats = new Material[map.possibleTerrain.Length];
		for (int i = 0; i < map.possibleTerrain.Length; i++) {
			mats [i] = map.possibleTerrain [i].terrainMaterial;
		}

		List<MeshFilter> walkables = MSGen.Generate (wallMap, map.tileSize, map.wallHeight, mats, map.wallMaterial);

		ObjectGenerator objGen = GetComponent<ObjectGenerator> ();

		List<GameObject> objects = objGen.GenerateObjects (map, noiseMap);

		NavMeshGenerator meshGen = GetComponent<NavMeshGenerator> ();
		meshGen.Initialise ();

		meshGen.BuildNavMesh (walkables, objects, wallMap.GetLength (0) * map.tileSize, wallMap.GetLength (1) * map.tileSize);

		StaticBatchingUtility.Combine (objects.ToArray (), objects [0].transform.parent.gameObject);

		Light worldLight = (Light)Instantiate (new GameObject (), Vector3.zero, Quaternion.Euler (map.lightOptions.angle), this.transform).AddComponent (typeof(Light));

		worldLight.name = "World Light";

		worldLight.type = LightType.Directional;
		worldLight.intensity = map.lightOptions.intensity;
		worldLight.color = map.lightOptions.lightColour;
		worldLight.shadows = LightShadows.Soft;

		worldLight.shadowStrength = 0.8f;

		if (map.postProcProfile != null) {
			Camera.main.GetComponent<PostProcessingBehaviour> ().profile = map.postProcProfile;
		} else {
			Debug.LogWarning ("Map is missing a Post Processing Profile");
		}

	}

	void CreateWallMap ()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				wallMap [x, y] = GetMapHeight (x, y);
			}
		}
	}

	private int GetMapHeight (int x, int y)
	{
		int result = 0;
		for (int i = map.possibleTerrain.Length - 1; i >= 0; i--) {
			if (noiseMap [x, y] <= map.possibleTerrain [i].height) {
				result = i;
			}
		}
		return result;
	}

	void ProcessMap ()
	{
		List<List<Coord>> wallRegions = GetWallRegions ();
		int wallThresholdSize = 0;

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Coord tile in wallRegion) {
					wallMap [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> roomRegions = GetFloorRegions ();
		int roomThresholdSize = 0;
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				foreach (Coord tile in roomRegion) {
					wallMap [tile.tileX, tile.tileY] = 0;
				}
			} else {
				survivingRooms.Add (new Room (roomRegion, wallMap));
			}
		}
		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;

		ConnectClosestRooms (survivingRooms);
	}

	List<List<Coord>> GetWallRegions ()
	{
		List<List<Coord>> regions = new List<List<Coord>> ();
		int[,] mapFlags = new int[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (mapFlags [x, y] == 0 && wallMap [x, y] == 0) {
					List<Coord> newRegion = GetRegionTiles (x, y);
					regions.Add (newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags [tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<List<Coord>> GetFloorRegions ()
	{

		List<List<Coord>> regions = new List<List<Coord>> ();
		int[,] mapFlags = new int[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (mapFlags [x, y] == 0 && wallMap [x, y] > 0) {
					List<Coord> newRegion = GetRegionTiles (x, y);
					regions.Add (newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags [tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles (int startX, int startY)
	{
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[width, height];
		int tileType = wallMap [startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue ();
			tiles.Add (tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange (x, y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags [x, y] == 0 && wallMap [x, y] == tileType) {
							mapFlags [x, y] = 1;
							queue.Enqueue (new Coord (x, y));
						}
					}
				}
			}
		}
		return tiles;
	}

	void ConnectClosestRooms (List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
	{

		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();

		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add (room);
				} else {
					roomListA.Add (room);
				}
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0) {
					continue;
				}
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected (roomB)) {
					continue;
				}

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
						Coord tileA = roomA.edgeTiles [tileIndexA];
						Coord tileB = roomB.edgeTiles [tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX - tileB.tileX, 2) + Mathf.Pow (tileA.tileY - tileB.tileY, 2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms (allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms (allRooms, true);
		}
	}

	void CreatePassage (Room roomA, Room roomB, Coord tileA, Coord tileB)
	{
		Room.ConnectRooms (roomA, roomB);

		List<Coord> line = GetLine (tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle (c, 2);
		}
	}

	List<Coord> GetLine (Coord from, Coord to)
	{
		List<Coord> line = new List<Coord> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs (dy);
			shortest = Mathf.Abs (dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++) {
			line.Add (new Coord (x, y));

			if (inverted) {
				y += step;
			} else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				} else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	void DrawCircle (Coord c, int r)
	{
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x * x + y * y <= r * r) {
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if (IsInMapRange (drawX, drawY)) {
						if (wallMap [drawX, drawY] != map.possibleTerrain.Length - 1)
							wallMap [drawX, drawY]++;
					}
				}
			}
		}
	}

	bool IsInMapRange (int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	struct Coord
	{
		public int tileX;
		public int tileY;

		public Coord (int x, int y)
		{
			tileX = x;
			tileY = y;
		}
	}

	class Room : IComparable<Room>
	{
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room ()
		{
		}

		public Room (List<Coord> roomTiles, int[,] map)
		{
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room> ();

			edgeTiles = new List<Coord> ();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map [x, y] == 0) {
								edgeTiles.Add (tile);
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom ()
		{
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom ();
				}
			}
		}

		public static void ConnectRooms (Room roomA, Room roomB)
		{
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom ();
			} else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom ();
			}
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected (Room otherRoom)
		{
			return connectedRooms.Contains (otherRoom);
		}

		public int CompareTo (Room otherRoom)
		{
			return otherRoom.roomSize.CompareTo (roomSize);
		}
	}

}
