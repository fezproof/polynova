using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Spawner : MonoBehaviour
{
	//	[SerializeField]
	//	private List<EnemyController> enemiesToSpawn;
	//	public List<NetworkPooler> enemyPools;
	[Header ("Prefabs of enemy types")]
	public GameObject rangedEnemy;

	[Header ("Prefab of SpawnPoint")]
	public GameObject spawnPoint;

	[Space (10f)]
	public ScriptableInt maxEnemies;
	public ScriptableInt enemyCount;
	public PlayerRS players;

	[Tooltip ("Amount of times per second")]
	[Range (0f, 1f)]public float spawnRate = 0.5f;
	private WaitForSeconds spawnDelay;

	//	public Transform player;
	private Map map;
	private Level.Squad[] spawnableSquads;
	private GameObject targetObjective;

	public void Initialise (Map map, Level.Squad[] squads, GameObject targetObjective)
	{
		if (!NetworkServer.active)
			return;
			

		this.spawnableSquads = squads;
		this.targetObjective = targetObjective;
		this.map = map;
		SpawnPlayerSpawns ();
		spawnDelay = new WaitForSeconds (1 / spawnRate);
		FindEnemyPools ();
		StartCoroutine (Spawning ());
	}

	private IEnumerator Spawning ()
	{
		Vector3 spawnLocation;

		SpawnTargetObjective ();

		while (true) {
			if (enemyCount.Value < maxEnemies.Value) {
				spawnLocation = FindSpawnPoint ();
				SpawnEnemies (spawnLocation);
				yield return spawnDelay;
			}
			yield return new WaitForSeconds (5f);
		}
	}

	public Vector3 FindSpawnPoint ()
	{	
		Vector3 point = new Vector3 (-map.levelWidth / 2 + Random.Range (0f, map.levelWidth), 0f, -map.levelHeight / 2 + Random.Range (0f, map.levelHeight));
//		Debug.Log (point);
		return point * map.tileSize;
	}

	private void SpawnEnemies (Vector3 location)
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition (location, out hit, 10f, NavMesh.AllAreas)) {
			float chance;
			GameObject obj;
			int squadSize = 0;
			for (int i = 0; i < spawnableSquads.Length; i++) {
				chance = Random.Range (0f, 1f);
				if (spawnableSquads [i].chanceProgression.Evaluate (0.5f) >= chance) {
					Debug.Log ("Spawn " + i);
					RangedEnemy ranged;
					for (int a = 0; a < spawnableSquads [i].rangedEnemies.Length; a++) {
						for (int ai = 0; ai < spawnableSquads [i].rangedEnemies [a].amount; ai++) {
							if (NavMesh.SamplePosition (RandomPointInCircle (hit.position, 5f), out hit, 3f, NavMesh.AllAreas)) {
								obj = spawnableSquads [i].rangedPool.ReuseObject (hit.position, Quaternion.identity);
								ranged = obj.GetComponent<RangedEnemy> ();
								ranged.baseStats = spawnableSquads [i].rangedEnemies [a].baseStats;
								ranged.enemyStats = spawnableSquads [i].rangedEnemies [a].enemyStats;
								ranged.rangedStats = spawnableSquads [i].rangedEnemies [a].rangedStats;
								ranged.SetStats ();
								NetworkServer.Spawn (obj);
								squadSize++;
							}
						}
						//TODO melee spawning
					}
					break;
				}
			}
			enemyCount.Value += squadSize;
		}
	}

	private void FindEnemyPools ()
	{
		for (int i = 0; i < spawnableSquads.Length; i++) {
			spawnableSquads [i].rangedPool = rangedEnemy.GetComponent<PoolObject> ().pool;
		}
	}

	private void SpawnTargetObjective ()
	{
		float x = Random.Range (-5, 5);
		float z = Random.Range (-5, 5);
		GameObject obj = GameObject.Instantiate (targetObjective, new Vector3 (x, 0, z), Quaternion.identity);				
		NetworkServer.Spawn (obj);
	}

	private void SpawnPlayerSpawns ()
	{
		Vector3 spawnLocation = FindPointInSquares (map.levelWidth * map.tileSize * 0.9f, map.levelWidth * map.tileSize * 0.7f, Vector3.zero);
		NavMeshHit hit;
		while (!NavMesh.SamplePosition (spawnLocation, out hit, 5f, NavMesh.AllAreas)) {
			spawnLocation = FindPointInSquares (map.levelWidth * map.tileSize * 0.9f, map.levelWidth * 0.7f * map.tileSize, Vector3.zero);
		}
		spawnLocation = hit.position;
//		Vector3 currentSpawn;
		for (int i = 0; i < players.Items.Count; i++) {
//			currentSpawn = RandomPointOnCircumference (spawnLocation, 3f);
			while (!NavMesh.SamplePosition (spawnLocation, out hit, 1f, NavMesh.AllAreas)) {
//				currentSpawn = RandomPointOnCircumference (spawnLocation, 3f);
			}
			Instantiate (spawnPoint, hit.position, Quaternion.identity, this.transform);
		}
	}

	private Vector3 FindPointInSquares (float maxSquare, float minSquare, Vector3 middlePoint)
	{
		float x = Random.Range (0f, maxSquare);
		float y;
		if (x < maxSquare / 2f + minSquare / 2f && x > maxSquare / 2f - minSquare / 2f) {
			int multiplyer = Random.Range (0, 2);
			y = Random.Range (0f, maxSquare / 2f - minSquare / 2f) * multiplyer +
			Random.Range (maxSquare / 2f + minSquare / 2f, maxSquare) * (1 - multiplyer);
		} else {
			y = Random.Range (0f, minSquare);
		}

		return (middlePoint + (new Vector3 (-maxSquare / 2f + x, 0f, -maxSquare / 2f + y)));
	}

	private Vector3 RandomPointOnCircumference (Vector3 centre, float radius)
	{
		float angle = Random.Range (0f, 1f) * 2f * Mathf.PI;
		return new Vector3 (centre.x + Mathf.Cos (angle), 0f, centre.z + Mathf.Sin (angle));
	}

	private Vector3 RandomPointInCircle (Vector3 centre, float radius)
	{
		float a = Random.Range (0f, 1f);
		float b = Random.Range (0f, 1f);

		if (b < a) {
			float c = a;
			a = b;
			b = c;
		}

		return centre += new Vector3 (b * radius * Mathf.Cos (2 * Mathf.PI * a / b), 0f, b * radius * Mathf.Sin (2 * Mathf.PI * a / b));
	}

}
