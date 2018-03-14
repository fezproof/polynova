using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryRS : RuntimeSet<Item>
{
	public GameEvent OnInventoryUpdate;

	public override bool Add (Item t)
	{
		if (Items.Count < 8) {
			base.Add (t);
			OnInventoryUpdate.Raise ();
			return true;
		} else {
			return false;
		}
	}

	public Item[] GetItems ()
	{
		return Items.ToArray ();
	}

	public void ClearInv ()
	{
		Items.Clear ();
		Items.Capacity = 8;
	}
}
