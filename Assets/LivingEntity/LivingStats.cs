using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Stats/Base")]
public class LivingStats : ScriptableObject
{

	[Header ("Attac")]
	public float attackDamage = 10f;

	[Header ("Protec")]
	public float baseHealth = 50f;
	public float baseRegen = 0f;
	public float moveSpeed = 2f;

	[Header ("Effec")]
	public Mesh model;
	public Vector3 modelOffset = new Vector3 (0f, 0.5f, 0f);

	[HideInInspector]
	new public string name;

}
