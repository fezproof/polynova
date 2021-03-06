using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Guns/MultiGun")]
public class MultiGun : GunObject
{
	[Range (1, 12)]public int shotCount = 4;
	[Range (0f, 360f)]public float spreadAngle = 30f;

	public override IEnumerator Shoot (Gun gun, Vector3 position, Quaternion rotation, float damage)
	{
		if (gun.triggerDown) {
			if (Time.time > gun.nextShotTime) {
				gun.nextShotTime = Time.time + shotDelay;
				float spread = -spreadAngle / 2f;
				for (int i = 0; i < shotCount; i++) {
					Quaternion newRot = new Quaternion ();
					newRot = Quaternion.Euler (rotation.eulerAngles.x, rotation.eulerAngles.y + spread + (spreadAngle / (shotCount - 1)) * i, rotation.eulerAngles.z);
					gun.CmdFire (position, newRot, damage);
				}
			}
		}
		yield return null;
	}

}
