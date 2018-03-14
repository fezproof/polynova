using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{

	public List<T> Items = new List<T> ();

	public virtual bool Add (T t)
	{
		if (!Items.Contains (t)) {
			Items.Add (t);
			return true;
		} else {
			return false;
		}
	}

	public virtual bool Remove (T t)
	{
		if (Items.Contains (t))
			return Items.Remove (t);
		return false;
	}
}
