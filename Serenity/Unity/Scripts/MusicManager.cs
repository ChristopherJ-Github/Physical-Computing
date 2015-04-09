using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///Class that contains properties that can be adjusted
///for each song. 
/// </summary>
[Serializable]
public class SongInfo {

	public AudioClip song;
	public float volume;
	[HideInInspector] public bool entrancePlayed;
	public float instensity;
}

/// <summary>
///Class in charge of storing and playing back music
///based on sensor data collected from the Sensor class.
/// </summary>
public class MusicManager : Singleton<MusicManager> {
	
	public AudioSource source1, source2;
	[HideInInspector]
	public AudioSource currentSong, nextSong;
	public Text currentSongText;
	SongInfo currentSongInfo;
	public float speed;
	public float quitSpeed;
	public float transitionTime;
	public float leeway;
	public SongInfo[] songs;
	float currentIntensity;
	bool transitioning;
	static MusicManager singleton;
	float maxVolume;
	string state;
	bool quitting;
	
	void Awake () {
		
		sortArray (songs);
		RowSpawner.instance.SpawnRows ();

		currentSong = source1;
		nextSong = source2;
		currentSongInfo = songs [0];
		currentSong.clip = currentSongInfo.song;
		maxVolume = currentSong.volume;
		Application.runInBackground = true;
	}
	
	void Update () {

		if (Input.GetKey(KeyCode.Escape))
			StartCoroutine(FadeQuit());


		float remainingTime = currentSong.clip.length - currentSong.time;
		if (remainingTime > transitionTime)
			return;


		if (transitioning || !Sensor.instance.ready)
			return;

		float startPoint = 0;
		float instensity = Sensor.instance.intensity;
		
		currentSongInfo = GetSongInfo(instensity);
		AudioClip song = currentSongInfo.song;
		StartCoroutine(TransitionMusic(song,  instensity, speed, currentSongInfo.volume));
	}

	/// <summary>
	/// Switches the new song to a new one
	/// </summary>
	public void UpdateSong () {

		float intensity = Sensor.instance.intensity;
		currentSongInfo = GetSongInfo(intensity);
		AudioClip song = currentSongInfo.song;

		StopAllCoroutines ();
		StartCoroutine(TransitionMusic(song, intensity, speed, currentSongInfo.volume));
	}

	/// <summary>
	/// Crossfades into audioClip based on _speed
	/// </summary>
	/// <returns>The music.</returns>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="instensityWhenCalled">Instensity when called.</param>
	/// <param name="_speed">_speed.</param>
	/// <param name="volume">Volume.</param>
	IEnumerator TransitionMusic (AudioClip audioClip, float instensityWhenCalled, float _speed, float volume) {

		if (quitting) //don't mess with the volume while FadeQuit is active
			yield break;

		transitioning = true;
		float currentSongMaxVolume = currentSong.volume;
		nextSong.audio.clip = audioClip;
		nextSong.Play ();
		
		float transition = 0;
		while (transition != 1) {
			
			transition = Mathf.MoveTowards (transition, 1, _speed * Time.unscaledDeltaTime);
			currentSong.volume = Mathf.Lerp(currentSongMaxVolume, 0, transition);
			nextSong.volume = Mathf.Lerp(0, volume, transition);
			yield return null;
		}
		
		currentSong.Stop ();
		SwitchSongSources ();
		currentIntensity = instensityWhenCalled;
		transitioning = false;
		currentSongText.text = currentSong.clip.name;
	}

	IEnumerator FadeQuit () {

		quitting = true;

		float currentSongMaxVolume = currentSong.volume;
		float transition = 0;
		while (transition != 1) {
			
			transition = Mathf.MoveTowards (transition, 1, quitSpeed * Time.unscaledDeltaTime);
			currentSong.volume = Mathf.Lerp(currentSongMaxVolume, 0, transition);
			yield return null;
		}
		Application.Quit ();
	}
	
	void SwitchSongSources () {
		
		AudioSource _currentSong = currentSong;
		currentSong = nextSong;
		nextSong = _currentSong;
	}
	
	/// <summary>
	/// Based on the intensity of the music get the closest match
	/// </summary>
	/// <returns>The song info.</returns>
	/// <param name="intensity">Intensity.</param>
	SongInfo GetSongInfo (float intensity) {

		List<int> indexes = new List<int> ();
		
		bool matchFound = false;
		for (int i = songs.Length-1 ; i >= 0 ; i--) {
			if (intensity >= songs[i].instensity - leeway) {

				if (matchFound) { //only add songs with the same level
					if (intensity - leeway > songs[i].instensity || intensity + leeway < songs[i].instensity) {
						break;
					}
				}
				indexes.Add(i);
				matchFound = true;
			}
		}
		
		if (!matchFound) {
			float lowestLvl = songs[0].instensity;
			for (int i = 0; i < songs.Length; i++) {

				if (songs[i].instensity - leeway > lowestLvl) {
					break;
				}
				indexes.Add(i);
			}
		}
		
		int index = UnityEngine.Random.Range(0, indexes.Count);
		int songIndex = indexes[index];
		SongInfo songInfo = songs [songIndex];
		if (songInfo.song == currentSong.clip) { //if it's the same song just get the next one
			index = (index + 1) % indexes.Count;
			songIndex = indexes[index];
			songInfo = songs[songIndex];
		}

		return songInfo;
	}

	/// <summary>
	/// Sorts arrays specifically for SongInfo using the intensity value as a base
	/// </summary>
	/// <param name="array">Array.</param>
	public void sortArray(SongInfo[] array) {
		
		SongInfo[] arrayCopy = (SongInfo[])array.Clone ();
		float[] sortedArray = new float[array.Length];
		int[] index = new int[array.Length];
		
		
		for (int i = 0; i < array.Length; i++) 
			sortedArray[i] = array[i].instensity;
		Array.Sort(sortedArray);
		
		for (int i = 0; i < array.Length; i++) {
			index[i] = Array.IndexOf(sortedArray, array[i].instensity);
			sortedArray[index[i]] = -1; //remove value to allow detection of duplicate
		}
		
		for (int i = 0; i < array.Length; i++) 
			array[index[i]] = arrayCopy[i];
	}
	
}
