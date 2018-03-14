using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

//NOTE: Spawning functions are within the NetworkManager script

public class EnemyController : LivingEntity, IRanged
{
	public ScriptableInt enemyCount;
	[HideInInspector]
	public State currentState;

	public EnemyStats enemyStats;
	public Transform eyes;

	[HideInInspector]
	public NavMeshAgent navMeshAgent;
	[HideInInspector]
	protected float stateTimeElapsed;
	[HideInInspector]
	protected bool hasTarget;
	[HideInInspector]
	public Transform chaseTarget;

	protected Renderer meshRenderer;

	public float refreshRate = 0.2f;

	private bool aiActive;

	public NetworkHash128 assetId { get; set; }

	[SyncVar]
	Vector3 realPosition = Vector3.zero;
	[SyncVar]
	Quaternion realRotation;
	private float updatePositionRotationInterval;

	public float waitTime;

	public ParticleSystem sitEffect;

	public GameObject itemPrefab;

	protected override void Awake ()
	{
		base.Awake ();
		meshRenderer = GetComponentInChildren<Renderer> ();
	}

	protected override void Start ()
	{
		if (!isServer)
			return;
		base.Start ();
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.speed = moveSpeed;
		if (isServer) {
			aiActive = true;
			StartCoroutine (AI ());
		}

	}

	protected override void OnEnable ()
	{
		base.OnEnable ();

		realPosition = transform.position;
		realRotation = transform.rotation;
	}

	public override void SetStats ()
	{
		base.SetStats ();
		sitEffect = enemyStats.sitEffect;
		currentState = enemyStats.initialState;
	}

	protected IEnumerator AI ()
	{
		while (aiActive) {
			currentState.CheckAggro (this);
			yield return currentState.DoActions (this);
			yield return currentState.CheckTransitions (this);
			yield return new WaitForSeconds (waitTime);
		}
	}

	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
		if (isServer) {
			// update the server with position/rotation
			updatePositionRotationInterval += Time.deltaTime;
			if (updatePositionRotationInterval > 0.11f) { // 9 times per second
				updatePositionRotationInterval = 0;
				RpcSetPositionRotation (transform.position, transform.rotation);
			}
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
		}
	}

	void OnDrawGizmos ()
	{
		if (!isServer)
			return; //client doesn't have navMesh
		if (currentState != null && eyes != null) {
			Gizmos.color = currentState.sceneGizmoColor;
			Gizmos.DrawWireSphere (eyes.position, enemyStats.lookSphereCastRadius);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (navMeshAgent.destination, 0.5f);
		}
	}

	public IEnumerator TransitionToState (State nextState)
	{
		if (nextState != currentState) {
			yield return OnExitState (currentState);
			currentState = nextState;
			yield return OnStartState (currentState);
		}
	}

	private IEnumerator OnStartState (State state)
	{
		yield return currentState.DoFirstAction (this);
	}

	private IEnumerator OnExitState (State state)
	{
		yield return currentState.DoLastAction (this);
		stateTimeElapsed = 0;
		yield return null;
	}

	public bool CheckIfCountDownElapsed (float duration)
	{
		stateTimeElapsed += Time.deltaTime;
		return (stateTimeElapsed >= duration);
	}

	private void OnExitState ()
	{
		stateTimeElapsed = 0;
	}

	[ClientRpc]
	private void RpcSetPositionRotation (Vector3 position, Quaternion rotation)
	{
		realPosition = position;
		realRotation = rotation;
	}

	[ClientRpc]
	public void RpcSetColour (Color colour)
	{
		meshRenderer.material.color = colour;
	}

	public virtual void Shoot (Vector3 position, Quaternion direction)
	{
	}

	public virtual void StopShooting ()
	{
	}

	protected override void Destroy ()
	{
		enemyCount.Value--;
		Item item = enemyStats.itemTierTable.getItem ();
		if (item != null) {
			Debug.Log ("Drop!");
			GameObject obj = Instantiate (itemPrefab, new Vector3 (transform.position.x, 0, transform.position.z), Quaternion.identity);
			obj.GetComponent<Renderer> ().material.color = item.colour;
			Debug.Log (item.name);
		}
		base.Destroy ();
	}
}

