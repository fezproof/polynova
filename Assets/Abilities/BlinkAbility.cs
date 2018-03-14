using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Abilities/Point Target/Blink")]
public class BlinkAbility : Ability
{
	public float range;

	public override IEnumerator Cast (Vector3 playerPosition, Vector3 mousePosition, NavMeshAgent playerAgent)
	{
		float distance = Vector3.Distance (mousePosition, playerPosition);
		if (distance > range) {
			mousePosition = mousePosition * (range / distance) + playerPosition * ((distance - range) / distance);
		}

		NavMeshHit hit;
		if (NavMesh.SamplePosition (mousePosition, out hit, 0.1f, NavMesh.AllAreas)) {
			playerAgent.Warp (hit.position);
		} else {
			NavMesh.Raycast (playerPosition, mousePosition, out hit, NavMesh.AllAreas);
			playerAgent.Warp (hit.position);
		}
		playerAgent.Warp (mousePosition);
		yield return base.Cast ();
	}
}