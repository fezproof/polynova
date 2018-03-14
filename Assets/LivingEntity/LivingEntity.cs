using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LivingEntity : NetworkBehaviour, IDamageable
{

	[HideInInspector][SyncVar]
	public string playerName;

	[HideInInspector]
	public HealthBar healthBar;

	[SyncVar (hook = "OnChangeHealth")]
	protected float health;

	protected float healthRegen;
	protected float moveSpeed;

	protected float attackDamage;

	protected Mesh model;
	protected Vector3 modelOffset;

	public LivingStats baseStats;

	protected bool dead;

	protected MeshFilter meshFilter;

	protected MeshCollider meshCollider;

	protected virtual void Awake ()
	{
		
	}

	protected virtual void OnEnable ()
	{
	}

	protected virtual void Start ()
	{
	}

	protected virtual void FixedUpdate ()
	{
		if (!dead) {
			if (health < baseStats.baseHealth) {
				health += healthRegen * Time.fixedDeltaTime;
				if (health > baseStats.baseHealth) {
					health = baseStats.baseHealth;
				}
			}
		}

	}

	public virtual void SetStats ()
	{
		health = baseStats.baseHealth;
		healthRegen = baseStats.baseRegen;
		moveSpeed = baseStats.moveSpeed;

		model = baseStats.model;

		attackDamage = baseStats.attackDamage;

		meshFilter = GetComponentInChildren<MeshFilter> ();
		meshFilter.transform.localPosition = baseStats.modelOffset;
		meshFilter.sharedMesh = baseStats.model;

		meshCollider = GetComponentInChildren<MeshCollider> ();
		if (meshCollider != null)
			meshCollider.sharedMesh = baseStats.model;

	}

	public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		if (!isServer)
			return;
		TakeDamage (damage);
	}

	public virtual void TakeDamage (float damage)
	{
		if (!isServer)
			return;
		health -= damage;

		if (healthBar != null) {
			healthBar.SetHealth ();
		}

		if (health <= 0 && !dead) {
			Die ();
		}
	}

	protected void Die ()
	{
		dead = true;
		if (healthBar != null) {
			healthBar.Destroy ();
		}
		Destroy ();
	}

	public bool isDead ()
	{
		return dead;
	}

	public float GetHealth ()
	{
		return health;
	}

	protected virtual void Destroy ()
	{
		gameObject.SetActive (false);
	}

	//----Health bar stuff----

	private void OnChangeHealth (float newHealth)
	{
		health = newHealth;
		if (healthBar != null) {
			healthBar.SetHealth ();
		}

		if (health <= 0 && !dead) {
			Die ();
		}
	}
}
