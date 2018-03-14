using UnityEngine;

public class InventoryUI : MonoBehaviour
{

	public InventoryRS inventory;

	InventoryItem[] slots;

	void Awake ()
	{
		slots = GetComponentsInChildren<InventoryItem> ();
	}

	public void UpdateInventory ()
	{
		Item[] items = inventory.GetItems ();
		for (int i = 0; i < slots.Length; i++) {
			if (i < items.Length && items [i] != null) {
				slots [i].SetSlot (items [i]);
			} else {
				slots [i].ClearSlot ();
			}
		}
	}
}
