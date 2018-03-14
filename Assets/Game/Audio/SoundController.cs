using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
	[SerializeField]private AudioMixerSnapshot[] musicSnapshots;

	private AudioSource[] musicSources;

	private AudioClip[] musicClips;

	private int currentMusicIndex = 1;

	[Header ("Music Clips")]
	public AudioClip normalMusic;
	public AudioClip actionMusic;

	[Space (10)]
	public float bpm = 120;

	public bool inAction;

	public float inTransitionNotes;
	public float outTransitionNotes;

	private float quarterNote;

	private float oldTime;

	private bool mute;

	public void Initialise ()
	{
		quarterNote = 60 / bpm;
		musicClips = new AudioClip[2];
		musicSources = GetComponentsInChildren<AudioSource> ();
	}

	public void TransitionSong (AudioClip newSong, float numberOfNotes)
	{
		if (newSong != musicClips [currentMusicIndex])
			gameObject.SetActive (true); //this is a workaround - when running in a standalone build, the gameobject gets disabled somehow
			StartCoroutine (TransSong (newSong, numberOfNotes));
	}

	private IEnumerator TransSong (AudioClip newSong, float numberOfNotes)
	{
		int oldIndex = currentMusicIndex;
		currentMusicIndex = 1 - currentMusicIndex;

		if (newSong != musicSources [currentMusicIndex].clip) {
			musicSources [currentMusicIndex].clip = newSong;
			musicSources [currentMusicIndex].Play ();
		} else {
			musicSources [currentMusicIndex].Play ();
			musicSources [currentMusicIndex].time = oldTime;
		}

		if (mute) {
			Mute ();
		} else {
			Unmute ();
		}

		musicClips [currentMusicIndex] = newSong;

		musicSnapshots [currentMusicIndex].TransitionTo (numberOfNotes * quarterNote);

		yield return new WaitForSeconds (numberOfNotes * quarterNote);
		oldTime = musicSources [oldIndex].time;

	}

	public void Mute()
	{
		mute = true;
		musicSources [currentMusicIndex].mute = true;
	}


	public void Unmute()
	{
		mute = false;
		musicSources [currentMusicIndex].mute = false;
	}
}
