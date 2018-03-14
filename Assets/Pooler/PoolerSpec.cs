using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolerSpec : ScriptableObject
{
	[SerializeField]
	public GameObject 	objectToPool;
	[SerializeField]
	public int 			amountToPool;
	[SerializeField]
	public bool 		willGrow;
	[SerializeField]
	public int 			growAmount;
}
