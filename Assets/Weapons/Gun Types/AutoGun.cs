using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Guns/AutoGun")]
public class AutoGun : GunObject
{
	public override IEnumerator Shoot (Gun gun, Vector3 position, Quaternion rotation, float damage)
	{
		if (gun.triggerDown) {
			if (Time.time > gun.nextShotTime) {
				gun.nextShotTime = Time.time + shotDelay;
				gun.CmdFire (position, rotation, damage);
			}
		}
		yield return null;
	}

}
