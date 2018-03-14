using UnityEngine;
using UnityEngine.AI;

public class BasicPlayer : MonoBehaviour
{
	private Vector3 moveInput = Vector3.zero;
	private Vector3 moveVelocity = Vector3.zero;
	public float moveSpeed;
	private NavMeshAgent agent;
	public AbilityRS abilites;
	public GameEvent abilityEvent;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		abilites.refreshAbilities ();
	}

	void Update ()
	{
		moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		moveVelocity = moveInput.normalized * moveSpeed;
		CheckAbilities ();
	}

	void FixedUpdate ()
	{
		agent.velocity = moveVelocity;
	}

	private void CheckAbilities ()
	{
		foreach (Ability ability in abilites.Items) {
			if (Input.GetButtonDown (ability.abilityButtonAxisName)) {
				if (!ability.OnCooldown ()) {
					StartCoroutine (ability.Cast (transform.position, GetMousePos (), agent));
					abilityEvent.Raise ();
				}
			}
		}
	}

	private Vector3 GetMousePos ()
	{
		Plane plane = new Plane (Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float point = 0f;
		if (plane.Raycast (ray, out point)) {
			return ray.GetPoint (point);
		} else {
			return Vector3.zero;
		}
	}
}
