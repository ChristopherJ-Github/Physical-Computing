using UnityEngine;
using System.Collections;
using System.IO.Ports;

/// <summary>
/// Class taking BPM information from an Arduino script 
/// that comes with the pulse sensor:
/// https://github.com/WorldFamousElectronics/PulseSensor_Amped_Arduino/blob/master/PulseSensorAmped_Arduino_1dot4/PulseSensorAmped_Arduino_1dot4.ino
/// Also in charge of calibration.
/// </summary>
public class Sensor : Singleton<Sensor> {
	
	private SerialPort stream = new SerialPort("COM6", 1200);
	[HideInInspector]
	public float BPM;
	[HideInInspector]
	public float minBPM = Mathf.Infinity, maxBPM = 0;
	[HideInInspector]
	public float lerp;
	[HideInInspector]
	public float intensity;
	[HideInInspector]
	public bool ready;
	public float error;

	void Start () {

		stream.Open(); //Open the Serial Stream.
		StartCoroutine (Calibrate ());
	}
	
	/// <summary>
	/// For a few seconds calibrate before the user can actually 
	/// play a song. Optimally this would be in the form of running
	/// in place to get the user's heart rate up, putting on the sensor,
	/// and then running the program. This well let Calibrate get a large
	/// range of value that the user can produce.
	/// </summary>
	IEnumerator Calibrate () {

		float timer = 10f;
		while (timer > 0) {
			if (BPM > maxBPM)
				maxBPM = BPM;
			if (BPM < minBPM && BPM != 0)
				minBPM = BPM;
			timer -= Time.deltaTime;

			MusicManager.instance.currentSongText.text = "Calibrating, " + Mathf.Ceil(timer) + " seconds left"; //Override the current song text to display calibration info
			yield return null;
		}
		minBPM -= error;
		maxBPM += error;
		MusicManager.instance.currentSongText.text = "Ready, calibration range of " + (maxBPM - minBPM) + " gotten";
	}
	
	void Update () {

		stream.BaseStream.Flush ();
		string value = stream.ReadLine(); //Read the information
		float parsedValue = float.Parse (value);
		BPM = parsedValue;

		ready = BPM != 0;
		if (!ready)
			return;

		lerp = Mathf.InverseLerp (minBPM, maxBPM, BPM); //convert into an easily readable value between 0 and 1
		intensity = lerp * 100;
		stream.BaseStream.Flush ();
	}
}
