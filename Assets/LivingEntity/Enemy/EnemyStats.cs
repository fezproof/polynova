using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Stats/Enemies/Enemy Base")]
public class EnemyStats : ScriptableObject
{
	[Header ("AI")]
	public State initialState;
	public float lookRange = 30f;
	public float lookSphereCastRadius = 0.5f;

	[Header ("Effects")]
	public ParticleSystem sitEffect;

	[Header ("Loot")]
	public TierTable itemTierTable;
}
