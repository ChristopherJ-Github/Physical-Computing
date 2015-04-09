using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RowSpawner : Singleton<RowSpawner> {

	public GameObject row;
	private List<GameObject> rows;
	public RectTransform startPos;
	public RectTransform contentBackground;
	public Scrollbar scrollBar;
	public Gradient intensityColor;
	private float height;

	void Awake () {

		height = startPos.rect.height;
		startPos.gameObject.SetActive (false);

		Vector2 offsetMax = contentBackground.offsetMax;
		offsetMax.y = height * MusicManager.instance.songs.Length;
		contentBackground.offsetMax = offsetMax;

		StartCoroutine (SetDefaultPosition ());
	}

	IEnumerator SetDefaultPosition () {//no idea what's moving the bar in the first 2 frames so this was made to override it

		int frames = 2;
		while (frames > 0) {
			scrollBar.value = 1;
			frames --;
			yield return null;
		}
	}

	public void SpawnRows () {

		Vector3 startPosVec = startPos.position;
		SongInfo[] songs = MusicManager.instance.songs;
		rows = new List<GameObject> ();

		foreach (SongInfo songInfo in songs) {

			GameObject _row = Instantiate (row) as GameObject;
			RectTransform rectTransform = _row.GetComponent<RectTransform>();
			_row.transform.parent = contentBackground.transform;

			rectTransform.offsetMin = startPos.offsetMin;
			rectTransform.offsetMax = startPos.offsetMax;
			rectTransform.position = startPosVec;

			Row rowComp = _row.GetComponent<Row>();
			rowComp.Init(songInfo);
			startPosVec.y -= height;
			rows.Add(_row);
		}
	}

	public void DeleteRows () {

		foreach (GameObject _row in rows) 
			Destroy(_row);
		rows = null;
	}

	public void ResortRows () {

		DeleteRows ();
		SpawnRows ();
	}
}
