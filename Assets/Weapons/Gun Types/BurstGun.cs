using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Guns/BurstGun")]
public class BurstGun : GunObject
{
	public int burstCount = 3;

	public float timeBetweenShots = 0.1f;

	public override IEnumerator Shoot (Gun gun, Vector3 position, Quaternion rotation, float damage)
	{
		if (gun.triggerDown) {
			if (Time.time > gun.nextShotTime) {
				gun.nextShotTime = Time.time + shotDelay;
				WaitForSeconds wait = new WaitForSeconds (timeBetweenShots);
				for (int i = 0; i < burstCount; i++) {
					gun.CmdFire (position, rotation, damage);
					yield return wait;
				}
			}
		}
	}

}
