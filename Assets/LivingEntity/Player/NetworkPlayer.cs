using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent (typeof(Gun))]

public class NetworkPlayer : LivingEntity
{

	public PlayerStats playerStats;

	public bool infiniteHealth = false;
	private Vector3 moveInput = Vector3.zero;
	private Vector3 moveVelocity = Vector3.zero;
	public Vector3 normal;

	private Gun gun;
	public PlayerController controller;
	private Transform eyes;

	public Crosshairs crosshairsPrefab;
	private Crosshairs crosshairs;

	private Camera mainCamera;

	private FollowCamera followCam;
	private NavMeshAgent agent;

	public PlayerRS playerList;
	public PlayerRS alivePlayerList;

	public AbilityRS abilites;
	public GameEvent abilityEvent;

	[Space (5)]

	[SyncVar]
	public Color colour;

	protected override void Awake ()
	{
		if (NetworkServer.active) {
			Debug.Log (this.GetType ());
			playerList.Add (this);
			alivePlayerList.Add (this);
		}
		eyes = transform.Find ("Eyes");
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
//		GameManager.instance.numPlayersReadyToSpawn++;
	}

	public override void SetStats ()
	{
		base.SetStats ();
		gun.gunObject = playerStats.gun;
	}

	protected override void Start ()
	{
		base.Start ();
		InitialisePlayerColour ();

		if (!isLocalPlayer) {
			return;
		}
	
		agent = GetComponent<NavMeshAgent> ();
		controller = GetComponent<PlayerController> ();
		gun = GetComponent<Gun> ();
		gun.gunObject = playerStats.gun;
		SetStats ();
		abilites.refreshAbilities ();
//		GameManager.instance.numPlayersReadyToSpawn++;
	}

	public void OnLevelStart ()
	{
		if (isLocalPlayer) {
			followCam = GetComponent<FollowCamera> ();
			mainCamera = Camera.main;
			followCam.Intialise (mainCamera);
			crosshairs = Instantiate (crosshairsPrefab, Vector3.zero, Quaternion.Euler (-90, 0, 0));
		}
		if (healthBar != null) {
			healthBar.Initialise ();
		}
	}

	public void OnLevelEnd ()
	{
		Destroy (crosshairs);
	}

	void Update ()
	{
		if (!isLocalPlayer)
			return;
		
		//Movement
		moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		moveVelocity = moveInput.normalized * base.moveSpeed;
		controller.Move (moveVelocity);

		//Look input
		Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, transform.position);
		float rayDistance;
		Vector3 point = Vector3.zero;

		if (groundPlane.Raycast (ray, out rayDistance)) { //check crosshairs are over the groundplane
			point = ray.GetPoint (rayDistance);
			controller.LookAt (point);
			crosshairs.transform.position = point;
		}

		if (gun != null) {
			if (Input.GetButton ("Fire1")) {
				gun.OnTriggerDown ();
				gun.Shoot (eyes.position, eyes.rotation, attackDamage);
			} else {
				gun.OnTriggerUp ();
			}
		}

		CheckAbilities (point);
	}

	protected override void FixedUpdate ()
	{
		if (!isLocalPlayer)
			return;
		base.FixedUpdate ();
		agent.velocity = moveVelocity;
	}

	private void CheckAbilities (Vector3 mousePos)
	{
		foreach (Ability ability in abilites.Items) {
			if (Input.GetButtonDown (ability.abilityButtonAxisName)) {
				if (!ability.OnCooldown ()) {
					StartCoroutine (ability.Cast (transform.position, mousePos, agent));
					abilityEvent.Raise ();
				}
			}
		}
	}

	private void InitialisePlayerColour ()
	{
		GetComponentInChildren<MeshRenderer> ().material.SetColor ("_EmissionColor", colour);
		GetComponentInChildren<MeshRenderer> ().material.SetColor ("_Color", colour);
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		if (!infiniteHealth)
			base.TakeHit (damage, hitPoint, hitDirection);
	}

	public void OnDeath ()
	{
		crosshairs.OnPlayerDeath ();
	}

	protected override void Destroy ()
	{
		GameManager.instance.OnPlayerDeath (this);
		base.Destroy ();
	}


	public void Initialise ()
	{
		Awake ();
		OnEnable ();
		Start ();
	}
}

