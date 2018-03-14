using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Enemies/Decisions/Look")]
public class LookDecision : Decision
{

	public override bool Decide (EnemyController controller)
	{
		bool targetVisible = Look (controller);
		return targetVisible;
	}

	private bool Look (EnemyController controller)
	{
		NavMeshHit hit;

		Debug.DrawRay (controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.lookRange, Color.green);

		if (!NavMesh.Raycast (controller.eyes.position, controller.chaseTarget.position, out hit, NavMesh.AllAreas))
		{
			controller.waitTime = 0f;
			return true;
		} else {
			controller.waitTime = Random.Range (1f - controller.refreshRate, 1f + controller.refreshRate);
			return false;
		}
	}
}


