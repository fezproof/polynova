using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;


public class GameManager : NetworkBehaviour
{

	[HideInInspector]
	public static GameManager instance;

	//	[HideInInspector]
	public PlayerRS players;
	public PlayerRS alivePlayers;

	[SyncVar]
	public bool readyToSpawnPlayers = false;

	private LivingStats[] playerBaseStats;
	private PlayerStats[] playerPlayerStats;

	public ScriptableInt maxEnemies;
	public ScriptableInt enemyCount;

	public LobbyManager lobbyManager;
	public LevelManager levelManager;
	public MapGenerator mapGenerator;

	public bool useRandomSeed;

	public bool mute;
	public AudioClip menuMusicClip;
	private SoundController soundController;

	private bool objectiveFound = false;

	public Level[] levels;
	[SyncVar]
	private int currentLevel;
	[SyncVar]
	private int seed = -1;

	[SyncVar]
	private int numConnectionsReady = 0;


	public void Awake () //change to Awake when Unity fixes a known issue 
	{//https://issuetracker.unity3d.com/issues/awake-of-networkbehaviour-is-not-called-at-launch-in-standalone-build-but-is-in-the-editor
		instance = this;

		soundController = GetComponent<SoundController> ();
		soundController.Initialise ();
		soundController.TransitionSong (menuMusicClip, 0f);
		players.Items.Clear ();
		alivePlayers.Items.Clear ();

		if (mute)
			soundController.Mute ();
	}

	void Start ()
	{
		NetworkIdentity ni = GetComponent<NetworkIdentity> ();
	}

	//called by LobbyManager
	public void EnterGameLoop ()
	{
		if (!isServer)
			return;
		StartCoroutine (GameLoop ());
	}

	private IEnumerator GameLoop ()
	{
		if (isServer) {
			for (int i = 0; i < levels.Length; i++) {
				yield return LevelStarting (i);

				yield return LevelPlaying ();

				yield return LevelEnding ();
			}
		}
	}

	private IEnumerator LevelStarting (int level)
	{
		if (isServer) {
			Debug.Log ("Level is starting");

//			NetworkServer.SetAllClientsNotReady ();
//			ClientScene.Ready (NetworkManager.singleton.client.connection);

			currentLevel = level;
			GenerateSeed ();

			lobbyManager.ServerChangeScene (lobbyManager.playScene);

//			NetworkServer.SpawnObjects ();


			enemyCount.Value = 0;
			objectiveFound = false;
		
			Debug.Log ("connections: " + NetworkServer.connections.Count);
			Debug.Log ("numPlayers: " + players.Items.Count);

			while (players.Items.Count < NetworkServer.connections.Count) {
				yield return null;
			}

			readyToSpawnPlayers = true;


			yield return null;
		}
	}

	public void GenerateSeed ()
	{
		System.Random prng = new System.Random ();

		if (useRandomSeed) {
			seed = prng.Next ();
		}
	}


	private IEnumerator DelaySong (AudioClip audioClip, float seconds, float transitionTime)
	{
		yield return new WaitForSeconds (seconds - transitionTime);
		soundController.TransitionSong (audioClip, transitionTime);
	}

	[ContextMenu ("Play Action")]
	private void PlayAction ()
	{
		soundController.TransitionSong (levels [currentLevel].levelMusic.actionMusic, 1f);
		StartCoroutine (DelaySong (levels [currentLevel].levelMusic.bodyMusic, levels [currentLevel].levelMusic.actionMusic.length, 4f));
	}

	private IEnumerator LevelPlaying ()
	{
		Debug.Log ("Level is playing");
		while (!objectiveFound && alivePlayers.Items.Count > 0) {
			yield return null;
		}
	}

	private IEnumerator LevelEnding ()
	{
		Debug.Log ("Level is ending");

		seed = -1; //resets the seed - see LevelManager
		readyToSpawnPlayers = false;

		if (objectiveFound) {


			ForgetManagers ();

//			SavePlayers ();

		} else if (alivePlayers.Items.Count == 0) {
			//end game
		}

		yield return null;
	}

	private void ForgetManagers ()
	{
		mapGenerator = null;
		levelManager = null;
	}

	//	[ClientRpc]
	//	private void RpcEnablePlayer(NetworkInstanceId netId)
	//	{
	//		ClientScene.FindLocalObject (netId).gameObject.SetActive (true);
	//	}
	//
	//	[ClientRpc]
	//	private void RpcDisablePlayer(NetworkInstanceId netId)
	//	{
	//		ClientScene.FindLocalObject (netId).gameObject.SetActive (false);
	//	}


	public IEnumerator OnPlayerDeath (NetworkPlayer player)
	{
		if (isServer) {
			alivePlayers.Remove (player);
			player.gameObject.SetActive (false);
//			RpcDisablePlayer (player.netId);
			yield return new WaitForSeconds (5f);
		}
	}



	[Server]
	public void CmdObjectiveFound ()
	{
		objectiveFound = true;
	}

	public int getNumPlayers ()
	{
		return players.Items.Count;
	}

	public int getCurrentLevel ()
	{
		return currentLevel;
	}

	public int getSeed ()
	{
		return seed;
	}
}
