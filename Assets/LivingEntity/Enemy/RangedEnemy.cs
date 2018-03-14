using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(Gun))]
public class RangedEnemy : EnemyController
{
	public RangedEnemyStats rangedStats;

	private Gun gun;

	[HideInInspector]
	public float attackDistanceThreshold = 0.5f;
	protected float targetCollisionRadius = 0.5f;

	public delegate GameObject SpawnDelegate (Vector3 position, NetworkHash128 assetId);

	public delegate void UnSpawnDelegate (GameObject spawned);


	protected override void Awake ()
	{
		base.Awake ();
		gun = GetComponent<Gun> ();
	}

	protected override void Start ()
	{
		base.Start ();
	}

	public override void SetStats ()
	{
		base.SetStats ();
		gun.gunObject = rangedStats.gun;
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
	}

	public override void Shoot (Vector3 position, Quaternion direction)
	{
		gun.OnTriggerDown ();
		gun.Shoot (position, direction, attackDamage);
	}

	public override void StopShooting ()
	{
		gun.OnTriggerUp ();
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		//HitEffects (damage, hitPoint, hitDirection);
		base.TakeHit (damage, hitPoint, hitDirection);
	}

	private void HitEffects (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
	}
}
