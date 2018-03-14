using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Enemies/Actions/Shoot")]
public class ShootAction : Actions
{

	public override IEnumerator Act (EnemyController controller)
	{
		Shoot (controller);
		yield return null;
	}

	private void Shoot (EnemyController controller)
	{
		NavMeshHit navHit;
		if (!controller.navMeshAgent.Raycast (controller.chaseTarget.position, out navHit)) {
			Vector3 playerDirection = (controller.chaseTarget.position) - controller.transform.position;
			playerDirection.y = 0; //make sure bullets don't travel up or down
			Quaternion rotation = Quaternion.LookRotation (playerDirection);
			controller.Shoot (controller.eyes.position, rotation);
		}
	}
}

