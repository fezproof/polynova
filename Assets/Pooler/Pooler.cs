using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pooler : MonoBehaviour
{
	public bool willGrow = true;
	public int growAmount;
	public GameObject[] objectsToPool;
	public int[] amountsToPool;
	public Dictionary<int, Queue<GameObject>> pools;
	private Dictionary<int, GameObject> parentObjects;

	public static Pooler current;

	void Awake ()
	{
		gameObject.SetActive (false);
	}

	public void Initialise()
	{
		current = this;
		pools = new Dictionary<int, Queue<GameObject>> ();
		parentObjects = new Dictionary<int, GameObject> ();
		for (int i = 0; i < objectsToPool.Length; i++) {
			CreatePool (objectsToPool [i], amountsToPool [i]);
		}
	}

	private void CreatePool (GameObject prefab, int poolSize)
	{
		int poolKey = prefab.GetInstanceID ();

		if (!pools.ContainsKey (poolKey)) {

			GameObject parentObject = new GameObject ();
			parentObject.name = prefab.name + " pool";
			parentObject.transform.parent = this.transform;
			parentObjects.Add (poolKey, parentObject);

			pools.Add (poolKey, new Queue<GameObject> ());

			for (int i = 0; i < poolSize; i++) {
				GameObject newObject = Instantiate (prefab, parentObject.transform) as GameObject;
				newObject.SetActive (false);
				pools [poolKey].Enqueue (newObject);
			}
		}
	}

	public GameObject ReuseObject (GameObject prefab, Vector3 position, Quaternion rotation)
	{
		int poolKey = prefab.GetInstanceID ();

		if (pools.ContainsKey (poolKey)) {

			GameObject obj = pools [poolKey].Dequeue ();
			pools [poolKey].Enqueue (obj);
			if (willGrow && obj.activeInHierarchy) {
				GrowPool (prefab, growAmount);
				Debug.Log ("Grown: " + prefab.name);
			}
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.SetActive (true);
            

			return obj;
		}

		return null;
	}

	private void GrowPool (GameObject prefab, int amount)
	{
		int poolKey = prefab.GetInstanceID ();
		for (int i = 0; i < amount; i++) {
			GameObject newObject = Instantiate (prefab, parentObjects [poolKey].transform) as GameObject;
			newObject.SetActive (false);
			pools [poolKey].Enqueue (newObject);
		}
	}
}
