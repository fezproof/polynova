using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Game/Level")]
public class Level : ScriptableObject
{
	public string levelName;

	public Map map;

	public Squad[] possibleSquads;

	public GameObject targetObjective;

	public LevelMusic levelMusic;

	[System.Serializable]
	public class SpawnableRanged
	{
		public LivingStats baseStats;
		public EnemyStats enemyStats;
		public RangedEnemyStats rangedStats;
		public int amount = 1;
	}

	[System.Serializable]
	public class SpawnableMelee
	{
		public LivingStats baseStats;
		public EnemyStats enemyStats;
		public RangedEnemyStats rangedStats;
		public int amount = 1;
	}

	[System.Serializable]
	public class Squad
	{
		public SpawnableRanged[] rangedEnemies;
		public SpawnableMelee[] meleeEnemies;
		[HideInInspector]public NetworkPooler rangedPool;
		[HideInInspector]public NetworkPooler meleePool;

		public AnimationCurve chanceProgression = AnimationCurve.Linear (0f, 0f, 1f, 1f);

	}
}
