using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
	

	public TierTable[] tierTables;

	public GameObject itemPrefab;

	private void Start ()
	{
		for (int t = 0; t < tierTables.Length; t++) {
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 100 / (t * t + 1); j++) {
					Item item = tierTables [t].getItem ();
					if (item != null) {
						GameObject obj = Instantiate (itemPrefab, new Vector3 (t * 10 + i, 0, j), Quaternion.identity, transform);
						obj.GetComponent<Renderer> ().material.color = item.colour;
						Debug.Log (item.name);
					} else {
						Debug.Log ("Failed");
					}
				}
			}
		}
	}
}
