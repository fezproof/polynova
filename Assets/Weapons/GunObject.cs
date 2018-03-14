using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObject : ScriptableObject
{
	public string gunName;

	[Range (1f, 20f)]public float bulletSpeed = 1f;

	public float shotDelay = 0.5f;

	public NetworkProjectile bullet;

	public Color bulletColour = Color.blue;

	public virtual IEnumerator Shoot (Gun gun, Vector3 position, Quaternion rotation, float damage)
	{
		yield return null;
	}
}
