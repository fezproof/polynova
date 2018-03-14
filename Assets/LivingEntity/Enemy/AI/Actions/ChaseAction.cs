using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemies/Actions/Chase")]
public class ChaseAction : Actions
{
	public override IEnumerator Act (EnemyController controller)
	{
		controller.currentState.sceneGizmoColor = Color.red;
		Chase (controller);
		yield return null;
	}

	private void Chase (EnemyController controller)
	{
		Vector3 targetPosition = controller.chaseTarget.position;
		controller.navMeshAgent.destination = targetPosition;
	}
}


