using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Abilities/Dash")]
public class DashAbility : Ability
{

	public float power = 10f;
	public float time = 0.2f;


	public override IEnumerator Cast (Vector3 playerPosition = default (Vector3), Vector3 mousePosition = default (Vector3), NavMeshAgent playerAgent = default(NavMeshAgent))
	{
		float temp = 0f;
		currentCooldown = baseCoolDown;
		while (temp < time) {
			playerAgent.velocity = playerAgent.velocity.normalized * power;
			currentCooldown -= Time.deltaTime;
			yield return wait;
			temp += Time.fixedDeltaTime;
		}
		while (OnCooldown ()) {
			currentCooldown -= Time.deltaTime;
			yield return wait;
		}
	}
}