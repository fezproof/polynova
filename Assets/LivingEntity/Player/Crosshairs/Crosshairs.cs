using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Cursor.visible = false;
//		LivingEntity player = GameObject.FindGameObjectWithTag ("Player").GetComponent<LivingEntity>();
	}

	public void OnPlayerDeath ()
	{
		Cursor.visible = true;
	}
}
