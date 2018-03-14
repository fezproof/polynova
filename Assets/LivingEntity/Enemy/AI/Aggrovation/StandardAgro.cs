using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu (menuName = "Enemies/Aggros/StandardAggro")]
public class StandardAgro : Aggrovation
{

	public override void Aggro (EnemyController controller)
	{
		List<NetworkPlayer> players	= GameManager.instance.alivePlayers.Items;
		NetworkPlayer target = players [0];
		float	targetAggro = 0;
		float newTargetAggro = 0;

		targetAggro = calcAggro (controller, target);

		for (int i = 1; i < players.Count; i++) {
			if (players [i] != null) {
				newTargetAggro = calcAggro (controller, players [i]);
			}
			if (newTargetAggro > targetAggro) {
				target = players [i];
				targetAggro = newTargetAggro;
			}
		}

		controller.chaseTarget = target.transform;
	}

	public float calcAggro (EnemyController controller, NetworkPlayer player)
	{
		if (player.isDead ())
			return 0f;
		return 1 / Vector3.Distance (controller.transform.position, player.transform.position);
	}
}


