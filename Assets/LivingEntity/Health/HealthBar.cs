using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[RequireComponent (typeof(LivingEntity))]
public class HealthBar : MonoBehaviour
{

	private LivingEntity target;
	public Vector3 offset;

	public HealthBarElement healthBarPrefab;
	private HealthBarElement healthBar;

	void Awake ()
	{
		gameObject.SetActive (false);
	}

	void OnEnable ()
	{
		GameObject obj = healthBarPrefab.GetComponent<PoolObject> ().pool.ReuseObject (transform.position, Quaternion.identity);
		healthBar = obj == null ? null : obj.GetComponent<HealthBarElement> ();
		healthBar.offset = offset;

		target = GetComponent<LivingEntity> ();
		target.healthBar = this;
	}

	void Start ()
	{
		Initialise ();
	}

	public void Initialise()
	{
		SetHealth ();
		SetPlayerName (target.playerName);
	}

	void FixedUpdate ()
	{
		healthBar.transform.position = Camera.main.WorldToScreenPoint (transform.position) + offset * Screen.height / Screen.dpi;
	}

	public void SetPlayerName (string playerName)
	{
		if (playerName != null)
			healthBar.SetPlayerName (playerName);
	}

	public void SetHealth ()
	{
		healthBar.SetHealth (GetHealthPercent ());
	}

	private float GetHealthPercent ()
	{
		return target.GetHealth () / target.baseStats.baseHealth;
	}

	void OnDisable ()
	{
		if (healthBar != null) {
			healthBar.Destroy ();
		}
	}

	public void Destroy ()
	{
		healthBar.Destroy ();
		gameObject.SetActive (false);
	}
}
