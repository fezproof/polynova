using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelManager : MonoBehaviour
{

	[HideInInspector]
	public static LevelManager instance;

	private Level[] levels;

	public MapGenerator mapGenerator;
	public NetworkPoolerManager poolManager;
	public Spawner spawner;

	private SoundController soundController;

	public void Awake ()
	{
		instance = this;
		soundController = FindObjectOfType<SoundController> ();
		levels = GameManager.instance.levels;
		GameManager.instance.levelManager = this;
	}

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable ()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		GameManager.instance.levelManager = null;
	}

	void Start ()
	{
		StartCoroutine (InitialisePlayers ());
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		StartCoroutine (InitialiseLevel ());
	}

	private IEnumerator InitialiseLevel ()
	{
		while (GameManager.instance.getSeed () == -1) {//wait for SyncVar to update
			yield return null;
		}


		int level = GameManager.instance.getCurrentLevel ();
		int seed = GameManager.instance.getSeed ();

		soundController.TransitionSong (levels [level].levelMusic.introMusic, 4f);
		StartCoroutine (DelaySong (levels [level].levelMusic.bodyMusic, levels [level].levelMusic.introMusic.length, 0.5f));

		mapGenerator.Initialise (levels [level].map, seed);
		poolManager.Initialise ();
		spawner.Initialise (levels [level].map, levels [level].possibleSquads, levels [level].targetObjective);
	}

	private IEnumerator DelaySong (AudioClip audioClip, float seconds, float transitionTime)
	{
		yield return new WaitForSeconds (seconds - transitionTime);
		soundController.TransitionSong (audioClip, transitionTime);
	}

	private IEnumerator InitialisePlayers ()
	{
		while (!GameManager.instance.readyToSpawnPlayers) {
			yield return null;
		}
			
		LoadPlayers ();
		MovePlayers ();
	}

	public void MovePlayers ()
	{
		List<NetworkPlayer> players = GameManager.instance.players.Items;
		foreach (NetworkPlayer player in players) {
			Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
			player.transform.position = spawnPoint.position;
		}
	}

	public void LoadPlayers ()
	{
		Scene playScene = SceneManager.GetSceneByName ("LevelTest");
		GameObject[] objs = playScene.GetRootGameObjects ();
		Transform rootObj = objs [0].gameObject.transform.parent;

		List<NetworkPlayer> players = GameManager.instance.players.Items;

		Debug.Log ("Number of players: " + players.Count);

		for (int i = 0; i < players.Count; i++) {
			Debug.Log ("Loading player: " + players [i].playerName);
			players [i].transform.parent = rootObj;
			players [i].gameObject.SetActive (true);

			players [i].OnLevelStart ();
		}
	}

	private void SavePlayers ()
	{
		List<NetworkPlayer> players = GameManager.instance.players.Items;

		for (int i = 0; i < players.Count; i++) {
			players [i].transform.parent = GameManager.instance.transform;
			players [i].gameObject.SetActive (false);
			players [i].OnLevelEnd ();
		}
	}

		
}
