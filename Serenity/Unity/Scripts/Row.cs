using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class controlling information displayed in the 
/// rows of the player including the name of the 
/// respective song and the background color based
/// off of the current intensity setting
/// </summary>
public class Row : MonoBehaviour {

	public Text text;
	public Image background;
	private SongInfo songInfo;
	public Slider slider;
	
	public void Init (SongInfo _songInfo) {

		songInfo = _songInfo;
		text.text = songInfo.song.name;
		slider.value = songInfo.instensity / 100f;
		background.color = RowSpawner.instance.intensityColor.Evaluate (songInfo.instensity / 100);
	}

	/// <summary>
	/// Updates the song's intensity based on the slider value
	/// as well as the background color. Array of songs held in MusicManager
	/// are updated right away to show immediate results
	/// </summary>
	public void UpdateIntensity () {

		float intensity = slider.value * 100;
		songInfo.instensity = intensity;
		background.color = RowSpawner.instance.intensityColor.Evaluate (intensity / 100);
		MusicManager.instance.sortArray (MusicManager.instance.songs);

	}
}
