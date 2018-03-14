using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Gun : NetworkBehaviour
{
	public GunObject gunObject;

	public LayerMask hitMask;

	private NetworkPooler projectilePool;

	[HideInInspector] public bool triggerDown;

	public float nextShotTime;

	void OnEnable ()
	{
		projectilePool = gunObject.bullet.GetComponent<PoolObject> ().pool;
	}

	public void Shoot (Vector3 position, Quaternion rotation, float damage)
	{
		StartCoroutine (gunObject.Shoot (this, position, rotation, damage));
	}

	[Command]
	public void CmdFire (Vector3 position, Quaternion rotation, float damage)
	{
		GameObject obj = projectilePool.ReuseObject (position, rotation);
		NetworkProjectile projectile = obj == null ? null : obj.GetComponent<NetworkProjectile> ();
		projectile.damage = damage;
		projectile.SetLayerMask (hitMask.value);
		projectile.SetSpeed (gunObject.bulletSpeed);
//		obj.GetComponentInChildren<SpriteRenderer> ().color = gunObject.bulletColour;
		NetworkServer.Spawn (obj);
	}

	public void Aim (Vector3 aimPoint)
	{
		transform.LookAt (aimPoint);
	}

	public void OnTriggerDown ()
	{
		triggerDown = true;
	}

	public void OnTriggerUp ()
	{
		triggerDown = false;
	}

	public void Destroy ()
	{
		Destroy (this);
	}
}
