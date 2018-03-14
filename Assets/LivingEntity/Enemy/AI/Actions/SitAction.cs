using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemies/Actions/Sit")]
public class SitAction : Actions {

	public float offset = 0.1f;
	public float duration = 1;

	public override IEnumerator Act (EnemyController controller)
	{
		controller.currentState.sceneGizmoColor = Color.magenta;

		controller.navMeshAgent.isStopped = true;
		yield return Sit (controller, offset, duration);
		CastParticle (controller);
		yield return null;
	}

	private IEnumerator Sit(EnemyController controller, float offset, float duration)
	{
//		Vector3 startPos = controller.transform.position;
//		Vector3 endPos = controller.transform.position - Vector3.up * offset;
//
//		float startTime = Time.time;
//		float percent = 0;
//
//		while(percent < 1)
//		{
//			controller.transform.position = Vector3.Lerp(startPos, endPos, percent);
//
//			percent = (Time.time - startTime) / duration;
//			//			Debug.Log ("Position: " + controller.transform.position);
//			yield return null;
//		}
//
		//		Debug.Log ("Position: " + controller.transform.position);



		float startPos = controller.navMeshAgent.baseOffset;
		float endPos = startPos - offset;

		float startTime = Time.time;
		float percent = 0;

		while(percent < 1)
		{
			controller.navMeshAgent.baseOffset = Mathf.Lerp(startPos, endPos, percent);
			percent = (Time.time - startTime) / duration;
			yield return null;
		}

	}

	private void CastParticle (EnemyController controller)
	{
		if(controller.sitEffect != null)
		{
			Destroy (Instantiate (controller.sitEffect.gameObject, controller.transform.position, Quaternion.identity) as GameObject, controller.sitEffect.main.startLifetime.constant);
		}
	}
}