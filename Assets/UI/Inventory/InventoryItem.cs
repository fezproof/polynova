using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
	public Image icon;
	public Image background;

	private static Color blank = new Color (0f, 0f, 0f, 0f);

	public void SetSlot (Item item)
	{
		icon.sprite = item.icon;
		icon.color = Color.white;
		if (item.colour == blank)
			background.color = Color.white;
		else
			background.color = item.colour;
	}

	public void ClearSlot ()
	{
		icon.sprite = null;
		icon.color = new Color (0, 0, 0, 0);
		background.color = Color.grey;
	}
}
