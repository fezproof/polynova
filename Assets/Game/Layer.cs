using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Game/Layer")]
public class Layer : ScriptableObject
{
	public string layerName;

	public Level[] possibleLevels;

}
