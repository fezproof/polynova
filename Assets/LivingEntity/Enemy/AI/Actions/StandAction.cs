using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemies/Actions/Stand")]
public class StandAction : Actions {

	public float offset = 0.1f;
	public float duration = 1;

	public override IEnumerator Act (EnemyController controller)
	{
		controller.currentState.sceneGizmoColor = Color.magenta;
		yield return Stand (controller, offset, duration);
		controller.navMeshAgent.isStopped = false;
		yield return null;
	}

	private IEnumerator Stand(EnemyController controller, float offset, float duration)
	{
//		Vector3 startPos = controller.transform.position;
//		Vector3 endPos = controller.transform.position + Vector3.up * offset;
//
//		float startTime = Time.time;
//		float percent = 0;
//
//		while(percent < 1)
//		{
//			controller.transform.position = Vector3.Lerp(startPos, endPos, percent);
//			percent = (Time.time - startTime) / duration;
//			yield return null;
//		}

		float startPos = controller.navMeshAgent.baseOffset;
		float endPos = startPos + offset;

		float startTime = Time.time;
		float percent = 0;

		while(percent < 1)
		{
			controller.navMeshAgent.baseOffset = Mathf.Lerp(startPos, endPos, percent);
			percent = (Time.time - startTime) / duration;
			yield return null;
		}
	}



}


