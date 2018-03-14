using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapGenerator : MonoBehaviour
{
	public int minNodes = 20;
	public int maxNodes = 80;

	public float minSpacing = 10f;
	public float maxSpacing = 15f;

	private Node startNode;
	private List<Node> nodeMap;
	private Node endNode;

	// Use this for initialization
	void Awake ()
	{
		if (maxNodes < minNodes) {
			Debug.LogError ("Min must be smaller than max");
			return;
		}

		CreateNodes ();
		AnalyzeNodes ();
//		ProcessNodes ();
		Debug.Log (nodeMap.Count);
	}

	void OnDrawGizmos ()
	{
		foreach (Node node in nodeMap) {
			if (node == startNode)
				Gizmos.color = Color.green;
			else if (node == endNode)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.white;
			Gizmos.DrawWireSphere (node.position, node.radius);
			foreach (Node child in node.children) {
				Gizmos.DrawLine (node.position, child.position);
			}
		}
	}

	private void CreateNodes ()
	{
		
		nodeMap = new List<Node> (maxNodes);
		startNode = new Node (0, 0, minSpacing);
		nodeMap.Add (startNode);

		int nodeCount = 1;
		Vector2 currentLocation = new Vector2 (0, 0);
		Node currentNode = startNode;
		Vector2 tempLocation;

		Queue<Node> previousNodes = new Queue<Node> ();

		int dirCheckCount = 0;

		while (nodeCount < minNodes) {
			Vector2 tempDir = ChooseDirection ();
			tempLocation = currentLocation + tempDir;
			Node tempNode;
			if (CheckSphereCollision (tempLocation, out tempNode)) {
				if (CheckPointCollision (tempLocation, tempNode)) {
					if (!currentNode.children.Contains (tempNode)) {
						currentNode.Link (tempNode);
					}
				}
				dirCheckCount++;
				if (dirCheckCount > 10) {
					dirCheckCount = 0;
					currentNode = previousNodes.Dequeue ();
					currentLocation.x = currentNode.position.x;
					currentLocation.y = currentNode.position.z;
				}
			} else {
				dirCheckCount = 0;
				currentLocation = tempLocation;
				tempNode = new Node (currentLocation.x, currentLocation.y, minSpacing);
				nodeMap.Add (tempNode);
				currentNode.Link (tempNode);
				previousNodes.Enqueue (currentNode);
				currentNode = tempNode;
				nodeCount++;
			}
		}

		endNode = currentNode;

	}

	private Vector2 ChooseDirection ()
	{
		float angle = UnityEngine.Random.value * Mathf.PI * 2;
		Vector2 dir = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
		return dir * minSpacing * 2;
	}

	private bool CheckSphereCollision (Vector2 position, out Node outNode)
	{
		
		foreach (Node node in nodeMap) {
			float distance = Mathf.Pow (position.x - node.position.x, 2f) + Mathf.Pow (position.y - node.position.z, 2f);
			if (4 * node.radius * node.radius >= distance) {
				outNode = node;
				return true;
			}
		}
		outNode = null;
		return false;
	}

	private bool CheckSphereCollision (Node inNode)
	{

		foreach (Node node in nodeMap) {
			float distance = Mathf.Pow (inNode.position.x - node.position.x, 2f) + Mathf.Pow (inNode.position.z - node.position.z, 2f);
			if ((inNode.radius + node.radius) * (inNode.radius + node.radius) >= distance) {
				return true;
			}
		}
		return false;
	}

	private bool CheckPointCollision (Vector3 position, Node checkNode)
	{
		float distance = Mathf.Pow (position.x - checkNode.position.x, 2f) + Mathf.Pow (position.y - checkNode.position.z, 2f);
		if (checkNode.radius * checkNode.radius >= distance) {
			return true;
		}
		return false;
	}

	private void AnalyzeNodes ()
	{

		List<Node> removeNodes = new List<Node> ();
		List<Node> newNodes = new List<Node> ();

		do {
			newNodes.Clear ();
			removeNodes.Clear ();
			foreach (Node node in nodeMap) {
				if (!removeNodes.Contains (node)) {
					foreach (Node child in node.children) {
						if (!removeNodes.Contains (child)) {
							foreach (Node grandChild in child.children) {
								if (!removeNodes.Contains (grandChild)) {
									if (grandChild.children.Contains (node)) {
										Vector3 newPos = (node.position + child.position + grandChild.position) / 3f;
										float newRadius = 1.2f * (node.radius + child.radius + grandChild.radius) / 3f;

										Node newNode = new Node (newPos.x, newPos.z, newRadius);
										newNodes.Add (newNode);

										foreach (Node oldChild in node.children) {
											if (oldChild.position != child.position && oldChild.position != grandChild.position) {
												newNode.Link (oldChild);
												node.Unlink (oldChild);
											}
										}
										foreach (Node oldChild in child.children) {
											if (oldChild.position != node.position && oldChild.position != grandChild.position) {
												newNode.Link (oldChild);
												child.Unlink (oldChild);
											}
										}
										foreach (Node oldChild in grandChild.children) {
											if (oldChild.position != child.position && oldChild.position != node.position) {
												newNode.Link (oldChild);
												grandChild.Unlink (oldChild);
											}
										}

										removeNodes.Add (node);
										removeNodes.Add (child);
										removeNodes.Add (grandChild);
									}
								}
							}
						}
					}
				}
			}

			nodeMap.AddRange (newNodes);

			foreach (Node node in removeNodes) {
				nodeMap.Remove (node);
			}

		} while (newNodes.Count != 0);
	}

	//	private void ProcessNodes ()
	//	{
	//		List<Vector3> vertices = new List<Vector3> ();
	//		List<int> triangles = new List<int> ();
	//		List<Vector2> uvs = new List<Vector2> ();
	//
	//		foreach (Node node in nodeMap) {
	//			AddSquare (node.position, minSpacing / 2f, vertices, triangles, uvs);
	//
	//			foreach (Node child in node.children) {
	//				Vector3 midpoint = (node.position + child.position) / 2f;
	//				AddSquare (midpoint, minSpacing / 2f, vertices, triangles, uvs);
	//			}
	//
	//			// 0 = topRight, 1 = topLeft, 2 = bottomRight, 3 = bottomLeft
	//			if (node.hasNeighbor [0] && node.hasNeighbor [1]) {
	//				Vector3 midpoint = node.position + new Vector3 (0f, 0f, 0.75f) * minSpacing;
	//				AddSquare (midpoint, minSpacing / 4f, vertices, triangles, uvs);
	//			}
	//
	//			if (node.hasNeighbor [0] && node.hasNeighbor [2]) {
	//				Vector3 midpoint = node.position + new Vector3 (0.75f, 0f, 0f) * minSpacing;
	//				AddSquare (midpoint, minSpacing / 4f, vertices, triangles, uvs);
	//			}
	//
	//			if (node.hasNeighbor [2] && node.hasNeighbor [3]) {
	//				Vector3 midpoint = node.position + new Vector3 (0f, 0f, -0.75f) * minSpacing;
	//				AddSquare (midpoint, minSpacing / 4f, vertices, triangles, uvs);
	//			}
	//
	//			if (node.hasNeighbor [1] && node.hasNeighbor [3]) {
	//				Vector3 midpoint = node.position + new Vector3 (-0.75f, 0f, 0f) * minSpacing;
	//				AddSquare (midpoint, minSpacing / 4f, vertices, triangles, uvs);
	//			}
	//		}
	//
	//		Mesh mesh = new Mesh ();
	//		mesh.name = "Terrain Mesh";
	//		mesh.SetVertices (vertices);
	//		mesh.uv = uvs.ToArray ();
	//		mesh.SetTriangles (triangles, 0);
	//		mesh.RecalculateNormals ();
	//		mesh.RecalculateTangents ();
	//		mesh.RecalculateBounds ();
	//
	//		GetComponent<MeshFilter> ().sharedMesh = mesh;
	//	}

	private void AddSquare (Vector3 midpoint, float size, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
	{
		int count = vertices.Count;
		vertices.Add (midpoint + (new Vector3 (1f, 0f, 0f)) * size);
		vertices.Add (midpoint + (new Vector3 (-1f, 0f, 0f)) * size);
		vertices.Add (midpoint + (new Vector3 (0f, 0f, 1f)) * size);
		vertices.Add (midpoint + (new Vector3 (0f, 0f, -1f)) * size);

		uvs.Add (new Vector2 (vertices [count + 0].x, vertices [count + 0].z));
		uvs.Add (new Vector2 (vertices [count + 1].x, vertices [count + 1].z));
		uvs.Add (new Vector2 (vertices [count + 2].x, vertices [count + 2].z));
		uvs.Add (new Vector2 (vertices [count + 3].x, vertices [count + 3].z));

		triangles.Add (count + 0);
		triangles.Add (count + 1);
		triangles.Add (count + 2);

		triangles.Add (count + 0);
		triangles.Add (count + 3);
		triangles.Add (count + 1);
	}

	private class Node
	{
		public Vector3 position;
		public float radius;

		public List<Node> children;

		public Node (float x, float y, float radius)
		{
			position = new Vector3 (x, 0, y);
			children = new List<Node> ();
			this.radius = radius;
		}

		public void Link (Node node)
		{
			children.Add (node);
			node.children.Add (this);
		}

		public void Unlink (Node node)
		{
//			children.Remove (node);
			node.children.Remove (this);
		}
	}

}
