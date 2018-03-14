using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Item System/TierTable")]
public class TierTable : ScriptableObject
{
	[Range (0f, 100f)]public float dropChance = 10;
	public Rarity[] rarities;

	public Item getItem ()
	{
		if (Random.Range (0f, 100f) < dropChance) {
			Rarity rarity = getRarity ();
			Item item = rarity.lootTable.GetItem ();
			item.colour = rarity.colour;
			return item;
		}
		return null;
	}

	private Rarity getRarity ()
	{
		float chance = Random.Range (0f, 100f);
		float current = 0;
		for (int i = 0; i < rarities.Length; i++) {
			current += rarities [i].chance;
			if (chance <= current) {
				return rarities [i];
			}
		}
		return rarities [rarities.Length - 1];
	}

	[System.Serializable]
	public class Rarity
	{
		public string name;
		public Color colour;
		public LootTable lootTable;
		[Range (0f, 100f)]public float chance = 20f;
	}
}
