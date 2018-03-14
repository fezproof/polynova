using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Ability : ScriptableObject
{
	new public string name = "New Ability";
	public AudioClip sound;
	public float baseCoolDown = 1f;
	public Sprite icon;

	public string abilityButtonAxisName = "Fire2";

	public float currentCooldown = 0f;

	protected static WaitForFixedUpdate wait = new WaitForFixedUpdate ();

	public virtual IEnumerator Cast (Vector3 playerPosition = default (Vector3), Vector3 mousePosition = default (Vector3), NavMeshAgent playerAgent = default(NavMeshAgent))
	{
		currentCooldown = baseCoolDown;
		while (OnCooldown ()) {
			currentCooldown -= Time.deltaTime;
			yield return wait;
		}
	}

	public virtual IEnumerator PassiveCast (Vector3 playerPosition = default (Vector3), Vector3 mousePosition = default (Vector3), NavMeshAgent playerAgent = default(NavMeshAgent))
	{
		yield return null;
	}

	public bool OnCooldown ()
	{
		return currentCooldown > 0f;
	}

	public void Refresh ()
	{
		currentCooldown = 0f;
	}
}
