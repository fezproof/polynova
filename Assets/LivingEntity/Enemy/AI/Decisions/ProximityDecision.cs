using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Decisions/Look")]
public class ProximityDecision : Decision {

	public override bool Decide(EnemyController controller)
	{
		SetNearestPlayerAsTarget(controller);
		return true; //always stay in current state
	}

	private bool SetNearestPlayerAsTarget(EnemyController controller)
	{
		RaycastHit hit;

		Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.lookRange, Color.green);

		if (Physics.SphereCast(controller.eyes.position, controller.enemyStats.lookSphereCastRadius, controller.eyes.forward, out hit, controller.enemyStats.lookRange)
			&& hit.collider.CompareTag("Player"))
		{
			controller.chaseTarget = hit.transform;
			return true;
		}
		else
		{
			return false;
		}
	}
}


