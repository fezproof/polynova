using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMeshGenerator : MonoBehaviour
{
	private SquareGrid squareGrid;

	List<Vector3> vertices;
	List<int> triangles;

	Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>> ();
	List<List<int>> outlines = new List<List<int>> ();
	HashSet<int> checkedVertices = new HashSet<int> ();

	List<MeshFilter> walkables;

	public List<MeshFilter> Generate (int[,] wallMap, float tileSize, float wallHeight, Material[] mat, Material wallMat)
	{
		triangleDictionary.Clear ();
		outlines.Clear ();
		checkedVertices.Clear ();

		walkables = new List<MeshFilter> ();

		GenerateWallMesh (wallMap, tileSize, wallHeight, mat [0], wallMat);

		for (int i = 1; i < mat.Length; i++) {
			GenerateFloorMesh (wallMap, i, tileSize, mat [i]);
		}

		return walkables;
	}

	private void GenerateFloorMesh (int[,] wallMap, int targetIndex, float squareSize, Material mat)
	{
		
		Mesh mesh = CreateFloorMesh (wallMap, targetIndex, squareSize);

		walkables.Add (CreateMesh (mesh, mat));
	}

	private void GenerateWallMesh (int[,] wallMap, float squareSize, float wallHeight, Material mat, Material wallMat)
	{
		Mesh roofMesh = CreateRoofMesh (wallMap, squareSize, wallHeight);
		Mesh wallMesh = CreateWallMesh (wallHeight);

		CreateMesh (roofMesh, mat);
		CreateMesh (wallMesh, wallMat);
	}

	MeshFilter CreateMesh (Mesh mesh, Material material)
	{
		GameObject meshObj = new GameObject ("Mesh: " + material.name);
		meshObj.transform.parent = this.transform;

		meshObj.AddComponent<MeshFilter> ();
		meshObj.AddComponent<MeshCollider> ();
		meshObj.AddComponent<MeshRenderer> ();

		MeshFilter filter = meshObj.GetComponent<MeshFilter> ();
		filter.mesh = mesh;
		meshObj.GetComponent<MeshCollider> ().sharedMesh = mesh;
		meshObj.GetComponent<MeshRenderer> ().material = material;

		return filter;
	}

	Mesh CreateFloorMesh (int[,] map, int targetIndex, float squareSize)
	{
		squareGrid = new SquareGrid (map, squareSize, 0, targetIndex);

		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength (1); y++) {
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}

		Mesh mesh = new Mesh ();

		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();

		int tileAmount = 1;
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			float percentX = Mathf.InverseLerp (-map.GetLength (0) / 2 * squareSize, map.GetLength (0) / 2 * squareSize, vertices [i].x) * tileAmount;
			float percentY = Mathf.InverseLerp (-map.GetLength (0) / 2 * squareSize, map.GetLength (0) / 2 * squareSize, vertices [i].z) * tileAmount;
			uvs [i] = new Vector2 (percentX, percentY);
		}
		mesh.uv = uvs;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();

		return mesh;
	}

	Mesh CreateRoofMesh (int[,] map, float squareSize, float wallHeight)
	{
		squareGrid = new SquareGrid (map, squareSize, wallHeight, 0);

		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength (1); y++) {
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}

		Mesh mesh = new Mesh ();


		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();

		int tileAmount = 1;
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			float percentX = Mathf.InverseLerp (-map.GetLength (0) / 2 * squareSize, map.GetLength (0) / 2 * squareSize, vertices [i].x) * tileAmount;
			float percentY = Mathf.InverseLerp (-map.GetLength (0) / 2 * squareSize, map.GetLength (0) / 2 * squareSize, vertices [i].z) * tileAmount;
			uvs [i] = new Vector2 (percentX, percentY);
		}
		mesh.uv = uvs;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();

		return mesh;
	}

	Mesh CreateWallMesh (float wallHeight)
	{
		

		MeshCollider currentCollider = GetComponent<MeshCollider> ();
		Destroy (currentCollider);

		CalculateMeshOutlines ();

		List<Vector3> wallVertices = new List<Vector3> ();
		List<int> wallTriangles = new List<int> ();
		Mesh wallMesh = new Mesh ();

		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count - 1; i++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add (vertices [outline [i]]); // left
				wallVertices.Add (vertices [outline [i + 1]]); // right
				wallVertices.Add (vertices [outline [i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add (vertices [outline [i + 1]] - Vector3.up * wallHeight); // bottom right

				wallTriangles.Add (startIndex + 0);
				wallTriangles.Add (startIndex + 2);
				wallTriangles.Add (startIndex + 3);

				wallTriangles.Add (startIndex + 3);
				wallTriangles.Add (startIndex + 1);
				wallTriangles.Add (startIndex + 0);
			}
		}
		wallMesh.vertices = wallVertices.ToArray ();
		wallMesh.triangles = wallTriangles.ToArray ();
		wallMesh.RecalculateBounds ();
		wallMesh.RecalculateNormals ();
		wallMesh.RecalculateTangents ();

		return wallMesh;
	}

	void Generate2DColliders ()
	{

		EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D> ();
		for (int i = 0; i < currentColliders.Length; i++) {
			Destroy (currentColliders [i]);
		}

		CalculateMeshOutlines ();

		foreach (List<int> outline in outlines) {
			EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D> ();
			Vector2[] edgePoints = new Vector2[outline.Count];

			for (int i = 0; i < outline.Count; i++) {
				edgePoints [i] = new Vector2 (vertices [outline [i]].x, vertices [outline [i]].z);
			}
			edgeCollider.points = edgePoints;
		}

	}

	void TriangulateSquare (Square square)
	{
		switch (square.configuration) {
		case 0:
			break;

		// 1 points:
		case 1:
			if (square.topLeft.value != square.bottomRight.value) {
				MeshFromPoints (square.centreLeft, square.centre, square.centreBottom, square.bottomLeft);
			} else {
				MeshFromPoints (square.centreLeft, square.centreBottom, square.bottomLeft);
			}
			break;
		case 2:
			if (square.topRight.value != square.bottomLeft.value) {
				MeshFromPoints (square.bottomRight, square.centreBottom, square.centre, square.centreRight);
			} else {
				MeshFromPoints (square.bottomRight, square.centreBottom, square.centreRight);
			}
			break;
		case 4:
			if (square.bottomRight.value != square.topLeft.value) {
				MeshFromPoints (square.topRight, square.centreRight, square.centre, square.centreTop);
			} else {
				MeshFromPoints (square.topRight, square.centreRight, square.centreTop);
			}
			break;
		case 8:
			if (square.bottomLeft.value != square.topRight.value) {
				MeshFromPoints (square.topLeft, square.centreTop, square.centre, square.centreLeft);
			} else {
				MeshFromPoints (square.topLeft, square.centreTop, square.centreLeft);
			}
			break;

		// 2 points:
		case 3:
			MeshFromPoints (square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 6:
			MeshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
			break;
		case 9:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreLeft);
			break;
		case 5:
			if (square.topLeft.value != square.bottomRight.value) {
				MeshFromPoints (square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			} else if (square.topLeft.value < square.bottomLeft.value) {
				MeshFromPoints (square.centreTop, square.topRight, square.centreRight);
				MeshFromPoints (square.centreBottom, square.bottomLeft, square.centreLeft);
			} else {
				MeshFromPoints (square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			}
			break;
		case 10:
			if (square.bottomLeft.value != square.topRight.value) {
				MeshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
			} else if (square.bottomLeft.value < square.topLeft.value) {
				MeshFromPoints (square.topLeft, square.centreTop, square.centreLeft);
				MeshFromPoints (square.centreRight, square.bottomRight, square.centreBottom);
			} else {
				MeshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
			}
			break;

		// 3 point:
		case 7:
			MeshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 11:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;

		// 4 point:
		case 15:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			checkedVertices.Add (square.topLeft.vertexIndex);
			checkedVertices.Add (square.topRight.vertexIndex);
			checkedVertices.Add (square.bottomRight.vertexIndex);
			checkedVertices.Add (square.bottomLeft.vertexIndex);
			break;
		}


	}

	void MeshFromPoints (params Node[] points)
	{
		AssignVertices (points);

		if (points.Length >= 3)
			CreateTriangle (points [0], points [1], points [2]);
		if (points.Length >= 4)
			CreateTriangle (points [0], points [2], points [3]);
		if (points.Length >= 5)
			CreateTriangle (points [0], points [3], points [4]);
		if (points.Length >= 6)
			CreateTriangle (points [0], points [4], points [5]);

	}

	void AssignVertices (Node[] points)
	{
		for (int i = 0; i < points.Length; i++) {
			if (points [i].vertexIndex == -1) {
				points [i].vertexIndex = vertices.Count;
				vertices.Add (points [i].position);
			}
		}
	}

	void CreateTriangle (Node a, Node b, Node c)
	{
		triangles.Add (a.vertexIndex);
		triangles.Add (b.vertexIndex);
		triangles.Add (c.vertexIndex);

		Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary (triangle.vertexIndexA, triangle);
		AddTriangleToDictionary (triangle.vertexIndexB, triangle);
		AddTriangleToDictionary (triangle.vertexIndexC, triangle);
	}

	void AddTriangleToDictionary (int vertexIndexKey, Triangle triangle)
	{
		if (triangleDictionary.ContainsKey (vertexIndexKey)) {
			triangleDictionary [vertexIndexKey].Add (triangle);
		} else {
			List<Triangle> triangleList = new List<Triangle> ();
			triangleList.Add (triangle);
			triangleDictionary.Add (vertexIndexKey, triangleList);
		}
	}

	void CalculateMeshOutlines ()
	{

		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains (vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex (vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add (vertexIndex);

					List<int> newOutline = new List<int> ();
					newOutline.Add (vertexIndex);
					outlines.Add (newOutline);
					FollowOutline (newOutlineVertex, outlines.Count - 1);
					outlines [outlines.Count - 1].Add (vertexIndex);
				}
			}
		}

		SimplifyMeshOutlines ();
	}

	void SimplifyMeshOutlines ()
	{
		for (int outlineIndex = 0; outlineIndex < outlines.Count; outlineIndex++) {
			List<int> simplifiedOutline = new List<int> ();
			Vector3 dirOld = Vector3.zero;
			for (int i = 0; i < outlines [outlineIndex].Count; i++) {
				Vector3 p1 = vertices [outlines [outlineIndex] [i]];
				Vector3 p2 = vertices [outlines [outlineIndex] [(i + 1) % outlines [outlineIndex].Count]];
				Vector3 dir = p1 - p2;
				if (dir != dirOld) {
					dirOld = dir;
					simplifiedOutline.Add (outlines [outlineIndex] [i]);
				}
			}
			outlines [outlineIndex] = simplifiedOutline;
		}
	}

	void FollowOutline (int vertexIndex, int outlineIndex)
	{
		outlines [outlineIndex].Add (vertexIndex);
		checkedVertices.Add (vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline (nextVertexIndex, outlineIndex);
		}
	}

	int GetConnectedOutlineVertex (int vertexIndex)
	{
		List<Triangle> trianglesContainingVertex = triangleDictionary [vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex [i];

			for (int j = 0; j < 3; j++) {
				int vertexB = triangle [j];
				if (vertexB != vertexIndex && !checkedVertices.Contains (vertexB)) {
					if (IsOutlineEdge (vertexIndex, vertexB)) {
						return vertexB;
					}
				}
			}
		}

		return -1;
	}

	bool IsOutlineEdge (int vertexA, int vertexB)
	{
		List<Triangle> trianglesContainingVertexA = triangleDictionary [vertexA];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
			if (trianglesContainingVertexA [i].Contains (vertexB)) {
				sharedTriangleCount++;
				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}
		return sharedTriangleCount == 1;
	}

	struct Triangle
	{
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;

		public Triangle (int a, int b, int c)
		{
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3];
			vertices [0] = a;
			vertices [1] = b;
			vertices [2] = c;
		}

		public int this [int i] {
			get {
				return vertices [i];
			}
		}


		public bool Contains (int vertexIndex)
		{
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	public class SquareGrid
	{
		public Square[,] squares;

		public SquareGrid (int[,] map, float squareSize, float height, int wantedValue)
		{
			int nodeCountX = map.GetLength (0);
			int nodeCountY = map.GetLength (1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for (int x = 0; x < nodeCountX; x++) {
				for (int y = 0; y < nodeCountY; y++) {
					Vector3 pos = new Vector3 (-mapWidth / 2f + x * squareSize + squareSize / 2f, height, -mapHeight / 2f + y * squareSize + squareSize / 2f);
					controlNodes [x, y] = new ControlNode (pos, map [x, y] == wantedValue, map [x, y], squareSize);
				}
			}

//			for (int x = 0; x < nodeCountX; x++) {
//				for (int y = 0; y < nodeCountY; y++) {
//					if (map [x, y] != wantedValue && wantedValue != 0) {
//						if (x != nodeCountX - 1 && y != nodeCountY - 1) {
//							if (map [x + 1, y] != 0 && map [x, y + 1] != 0) {
//								controlNodes [x, y].active = true;
//							}
//						}
//					}
//
//				}
//			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];
			for (int x = 0; x < nodeCountX - 1; x++) {
				for (int y = 0; y < nodeCountY - 1; y++) {
					squares [x, y] = new Square (controlNodes [x, y + 1], controlNodes [x + 1, y + 1], controlNodes [x + 1, y], controlNodes [x, y]);
				}
			}

		}
	}

	public class Square
	{

		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public Node centre;
		public int configuration;
		public bool hasCentre;

		public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
		{
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;

			centre = new Node (new Vector3 (centreTop.position.x, centreTop.position.y, centreRight.position.z));

			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;

		}

	}

	public class Node
	{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node (Vector3 _pos)
		{
			position = _pos;
		}
	}

	public class ControlNode : Node
	{

		public bool active;
		public int value;
		public Node above, right;

		public ControlNode (Vector3 _pos, bool _active, int _value, float squareSize) : base (_pos)
		{
			active = _active;
			value = _value;
			above = new Node (position + Vector3.forward * squareSize / 2f);
			right = new Node (position + Vector3.right * squareSize / 2f);
		}

	}

}
