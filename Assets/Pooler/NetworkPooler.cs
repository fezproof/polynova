using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NetworkPooler : MonoBehaviour
{
	private GameObject objectToPool;
	private int amountToPool;
	private bool willGrow = true;
	private int growAmount;

	public LinkedList<GameObject> pool;

	public void Initialise(GameObject _objectToPool, int _amountToPool, bool _willGrow, int _growAmount)
	{
		objectToPool = _objectToPool;
		amountToPool = _amountToPool;
		willGrow = _willGrow;
		growAmount = _growAmount;

		InitialiseSpawnHandlers ();
		InitialisePool ();
	}

	private void InitialisePool ()
	{
		objectToPool.GetComponent<PoolObject> ().pool = this;
		CreatePool (objectToPool, amountToPool);
	}
		
	private void CreatePool (GameObject prefab, int poolSize)
	{
		pool = new LinkedList<GameObject> ();

		for (int i = 0; i < poolSize; i++) {
			GameObject newObject = Instantiate (prefab, this.transform) as GameObject;
			newObject.SetActive (false);
			pool.AddLast (newObject);
		}
	}

	public GameObject ReuseObject (Vector3 position, Quaternion rotation)
	{
		GameObject obj = pool.First.Value;
		if (willGrow && obj.activeInHierarchy) {
			GrowPool (growAmount);
			obj = pool.First.Value;
//			Debug.Log ("Grown: " + objectToPool.name);
		}

		pool.RemoveFirst ();
		pool.AddLast(obj);

		obj.transform.position = position;
		obj.transform.rotation = rotation;
		obj.SetActive (true);

		return obj;
	}

	public GameObject ReuseObject (Vector3 position)
	{
		GameObject obj = pool.First.Value;
		if (willGrow && obj.activeInHierarchy) {
			GrowPool (growAmount);
			obj = pool.First.Value;
		}

		pool.RemoveFirst ();
		pool.AddLast (obj);

		obj.transform.position = position;
		obj.SetActive (true);

		return obj;
	}

	private void GrowPool (int amount)
	{
		for (int i = 0; i < amount; i++) {
			GameObject newObject = Instantiate (objectToPool, this.transform) as GameObject;
			newObject.SetActive (false);
			pool.AddFirst (newObject);
		}
	}

	private void InitialiseSpawnHandlers()
	{
		NetworkIdentity netID = objectToPool.GetComponent<NetworkIdentity> ();
		if (netID != null)
		{
			NetworkHash128 assetId = netID.assetId;
			ClientScene.RegisterSpawnHandler(assetId, Spawn, UnSpawn);
		}
	}
		
	private GameObject Spawn(Vector3 position, NetworkHash128 assetId)
	{
		return ReuseObject(position);
	}

	private void UnSpawn(GameObject obj)
	{
		obj.SetActive (false);
	}
}
