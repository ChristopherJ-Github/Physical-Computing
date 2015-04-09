using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Class communicating information between the current song playing
/// and the seeker
/// </summary>
public class Seeker : MonoBehaviour {

	private float minPos, maxPos;
	private RectTransform rectTransform;
	private bool clicking;	
	private Slider slider;

	void Start () {

		rectTransform = GetComponent<RectTransform> ();
		slider = GetComponent<Slider> ();
	}

	void Update () {

		UpdateSlider (null);
	}

	public void UpdateSong (BaseEventData baseEventData) {

		AudioSource currentSong = MusicManager.instance.currentSong;
		currentSong.time = slider.value * currentSong.clip.length;
	}

	public void UpdateSlider (BaseEventData baseEventData) {

		AudioSource currentSong = MusicManager.instance.currentSong;
		slider.value = currentSong.time / currentSong.clip.length;
	}
}
