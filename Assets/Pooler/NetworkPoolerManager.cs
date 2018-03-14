using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPoolerManager : MonoBehaviour
{

	private List<NetworkPooler> pools;

	public GameObject[] objectsToPool;
	public int[] amountsToPool;
	public bool[] willGrows;
	public int[] growAmounts;

	public void Initialise ()
	{
		pools = new List<NetworkPooler> ();

		for (int i = 0; i < objectsToPool.Length; i++) {
			if (objectsToPool [i] != null) {
				GameObject childObject = new GameObject ();
				childObject.name = objectsToPool [i].name;
				childObject.transform.parent = this.transform;

				NetworkPooler pool = childObject.AddComponent<NetworkPooler> ();
				pool.Initialise (objectsToPool [i], amountsToPool [i], willGrows [i], growAmounts [i]);
				pools.Add (pool);
			}
		}
	}
}
