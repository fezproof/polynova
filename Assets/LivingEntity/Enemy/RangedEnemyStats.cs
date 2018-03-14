using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Stats/Enemies/Ranged")]
public class RangedEnemyStats : ScriptableObject
{
	[Header ("Weapon")]
	public GunObject gun;
}
