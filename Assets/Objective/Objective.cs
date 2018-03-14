using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Objective : NetworkBehaviour {

	private static int playerLayer = 8;
	private static int playerMask = 1 << playerLayer;

	[Server]
	void OnTriggerEnter(Collider c)
	{
		if (playerMask == (playerMask | (1 << c.gameObject.layer)))
		{
			GameManager.instance.CmdObjectiveFound ();
			gameObject.SetActive (false);
		}
	}
}
