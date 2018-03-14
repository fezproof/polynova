using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Stats/Player/Player Base")]
public class PlayerStats : ScriptableObject
{

	[Header ("Weapon")]
	public GunObject gun;
}
