using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListener> listeners = new List<GameEventListener> ();

	public void Raise ()
	{
		for (int i = listeners.Count - 1; i >= 0; i--) {
			listeners [i].OnEventRaised ();
		}
	}

	public void RegisterListener (GameEventListener listener)
	{
		if (!listeners.Contains (listener))
			listeners.Add (listener);
	}

	public bool UnregisterListener (GameEventListener listener)
	{
		if (listeners.Contains (listener))
			return listeners.Remove (listener);
		return false;
	}
}
