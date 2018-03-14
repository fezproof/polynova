using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Item System/LootTable")]
public class LootTable : ScriptableObject
{
	public Item[] items;

	public Item GetItem ()
	{
		return items [Random.Range (0, items.Length)];
	}
}
