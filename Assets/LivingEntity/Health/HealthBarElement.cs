using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarElement : MonoBehaviour {

	private TextMeshProUGUI playerName;
	private GameObject canvas;

	[HideInInspector]
	public Slider slider;
	[HideInInspector]
	public Vector3 offset;

	void Awake()
	{
		canvas = GameObject.FindGameObjectWithTag ("HealthBars");
		transform.SetParent(canvas.transform, false);
			
		playerName = GetComponentInChildren<TextMeshProUGUI> ();
		slider = GetComponentInChildren<Slider> ();
		gameObject.SetActive (false);
	}

	public void SetPlayerName(string name)
	{
		playerName.text = name;
	}

	public void SetHealth(float health)
	{
		slider.value = health;
	}

	public void Destroy ()
	{
		if (playerName != null)
		{
			playerName.text = null;
		}
		if (gameObject != null)
		{
			gameObject.SetActive (false);
		}
	}
}
