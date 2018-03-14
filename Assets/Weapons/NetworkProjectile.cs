using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//NOTE: Spawning functions are within the NetworkManager script

public class NetworkProjectile : NetworkBehaviour
{

	public Color trailColour;
	public float damage;
	public float damageRadius;

	public Rigidbody body;

	private float moveDistance = 0;

	public Quaternion rotation;
	public float speed = 5;
	public float range = 20;
	private float lifeTime;

	private float skinWidth = 0.1f;
	//compensates for enemy approaching bullet - ep7. 10:00

	private int collisionMask;
	private static int playerLayer = 8;
	private static int enemyLayer = 9;
	private static int obstacleLayer = 10;
	[HideInInspector]
	public static int killEnemyMask = 1 << enemyLayer | 1 << obstacleLayer;
	[HideInInspector]
	public static int killPlayerMask = 1 << playerLayer | 1 << obstacleLayer;

	//	public NetworkHash128 assetId { get; set; }

	public delegate GameObject SpawnDelegate (Vector3 position, NetworkHash128 assetId);

	public delegate void UnSpawnDelegate (GameObject spawned);

	void OnEnable ()
	{
		lifeTime = range / speed;
		Invoke ("Destroy", lifeTime);
	}

	public void SetSpeed (float newSpeed)
	{
		body.velocity = (transform.forward * newSpeed);
//		body.AddForce (transform.forward * speed);
	}

	public void SetLayerMask (int mask)
	{
		collisionMask = mask;
	}

	private void OnRotationUpdate (Quaternion newRotation)
	{
		rotation = newRotation;
		this.transform.rotation = rotation;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (!isServer)
			return;
		if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) {
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;
			Physics.Raycast (ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide);
			OnHitObject (other, hit.point, true);
		}
	}

	/*
	 * Checks if a collider is damageable,
	 * damages the collider and destroys the projectile.
	 */
	void OnHitObject (Collider c, Vector3 hitPoint, bool destroyProjectileOnAnyCollision)
	{
		IDamageable damageableObject = c.GetComponentInParent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeHit (damage, hitPoint, transform.forward);
			Destroy ();
		} else if (destroyProjectileOnAnyCollision) {
			Destroy ();
		}
	}

	void Destroy ()
	{
		NetworkServer.UnSpawn (this.gameObject);
		gameObject.SetActive (false);
	}

	void OnDisable ()
	{
		CancelInvoke ();
	}

}
