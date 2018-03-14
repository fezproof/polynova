using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempWorkAround : MonoBehaviour {

	public GameManager gameManager;

	//this is a workaround for the following issue: 
	//https://issuetracker.unity3d.com/issues/awake-of-networkbehaviour-is-not-called-at-launch-in-standalone-build-but-is-in-the-editor
	void Awake()
	{
//		gameManager.Initialise ();
	}

	void Start()
	{
		Debug.Log ("Mono start");
	}
}
