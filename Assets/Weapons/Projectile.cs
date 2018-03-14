using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	public Color trailColour;
	public float damage;
	public float damageRadius;

	public Rigidbody body;

	private float speed = 10;
	public float range = 20;
	private float lifeTime;

	private LayerMask collisionMask;
	private static int playerLayer = 8;
	private static int enemyLayer = 9;
	private static int obstacleLayer = 10;
	[HideInInspector]
	public static int killEnemyMask = 1 << enemyLayer | 1 << obstacleLayer;
	[HideInInspector]
	public static int killPlayerMask = 1 << playerLayer | 1 << obstacleLayer;

	void OnEnable ()
	{
		lifeTime = range / speed;
		Invoke ("Destroy", lifeTime);
	}

	public void SetSpeed (float newSpeed)
	{
		speed = newSpeed;
		body.velocity = Vector3.zero;
		body.AddForce (transform.forward * speed, ForceMode.Impulse);
	}

	public void SetLayerMask (int mask)
	{
		collisionMask = mask;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (((1 << collisionMask) & other.gameObject.layer) != 0) {
			OnHitObject (other, transform.position, true);
		}
	}

	/*
	 * Checks if a collider is damageable,
	 * damages the collider and destroys the projectile.
	 */
	void OnHitObject (Collider c, Vector3 point, bool destroyProjectileOnAnyCollision)
	{
		IDamageable damageableObject = c.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			Debug.Log ("OnHitObject");
			damageableObject.TakeHit (damage, point, transform.forward);
			Destroy ();
		} else if (destroyProjectileOnAnyCollision) {
			Debug.Log ("Hit2");
			Destroy ();
		}
	}

	void Destroy ()
	{
		gameObject.SetActive (false);
	}

	void OnDisable ()
	{
		CancelInvoke ();
	}
}
