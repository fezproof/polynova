using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "Item System/Item")]
public class Item : ScriptableObject
{
	new public string name;
	public string description;

	public Sprite icon;

	public Color colour;
}
