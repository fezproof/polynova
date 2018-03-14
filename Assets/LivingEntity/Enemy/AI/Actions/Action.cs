using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actions : ScriptableObject
{
	public abstract IEnumerator Act (EnemyController controller);

}