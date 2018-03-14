using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
	NavMeshDataInstance navInstance;
	NavMeshData navMesh;
	List<NavMeshBuildSource> navSources = new List<NavMeshBuildSource> ();

	public void Initialise ()
	{
		navMesh = new NavMeshData ();
		navInstance = NavMesh.AddNavMeshData (navMesh);
	}

	void OnDisable ()
	{
		navInstance.Remove ();
	}

	void OnEnable ()
	{
		if (!navInstance.valid) {
			navMesh = new NavMeshData ();
			navInstance = NavMesh.AddNavMeshData (navMesh);
		}
	}

	public void BuildNavMesh (List<MeshFilter> meshFilters, List<GameObject> objects, float width, float height)
	{
		navSources.Clear ();

		foreach (MeshFilter meshFilter in meshFilters) {
			Mesh mesh = meshFilter.sharedMesh;
			NavMeshBuildSource s = new NavMeshBuildSource ();
			s.shape = NavMeshBuildSourceShape.Mesh;
			s.sourceObject = mesh;
			s.transform = meshFilter.transform.localToWorldMatrix;
			s.area = 0;
			navSources.Add (s);
		}

		foreach (GameObject obj in objects) {
			NavMeshBuildSource s = new NavMeshBuildSource ();
			s.shape = NavMeshBuildSourceShape.Mesh;
			s.sourceObject = obj.GetComponent<MeshFilter> ().sharedMesh;
			s.transform = obj.transform.localToWorldMatrix;
//			s.size = obj.GetComponent<MeshRenderer> ().bounds.size;
//			s.area = 0;
			navSources.Add (s);
		}

		NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID (0);
		buildSettings.overrideVoxelSize = true;
		buildSettings.voxelSize = 0.05f;
		buildSettings.agentHeight = 0.9f;
		buildSettings.agentRadius = 0.4f;


		Bounds bounds = CreateBounds (width, height);

		NavMeshBuilder.UpdateNavMeshData (navMesh, buildSettings, navSources, bounds);
	}


	Bounds CreateBounds (float width, float height)
	{
		return new Bounds (Vector3.zero, new Vector3 (width, 5f, height));
	}
}
