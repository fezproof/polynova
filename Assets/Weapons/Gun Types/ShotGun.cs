using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Guns/ShotGun")]
public class ShotGun : GunObject
{
	public int shotCount = 4;
	[Range (0, 360)]public float spreadAngle = 30f;

	public override IEnumerator Shoot (Gun gun, Vector3 position, Quaternion rotation, float damage)
	{
		if (gun.triggerDown) {
			if (Time.time > gun.nextShotTime) {
				gun.nextShotTime = Time.time + shotDelay;
				for (int i = 0; i < shotCount; i++) {
					float spread = Random.Range (-spreadAngle / 2f, spreadAngle / 2f);
					Quaternion newRot = new Quaternion ();
					newRot = Quaternion.Euler (rotation.eulerAngles.x, rotation.eulerAngles.y + spread, rotation.eulerAngles.z);
					gun.CmdFire (position, newRot, damage);
				}
			}
		}
		yield return null;
	}

}
