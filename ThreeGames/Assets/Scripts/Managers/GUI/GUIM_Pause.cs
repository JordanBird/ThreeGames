using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIM_Pause : MonoBehaviour
{
	public GameManager gameManager;
	public Canvas pauseCanvas;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
		pauseCanvas.transform.FindChild ("Image - Limit").GetComponent<Image> ().color = Color.green;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			pauseCanvas.enabled = !pauseCanvas.enabled;
		}

		Cursor.visible = pauseCanvas.enabled;
	}

	public void ToggleLimit()
	{
		if (pauseCanvas.transform.FindChild ("Image - Limit").GetComponent<Image> ().color == Color.green)
		{
			pauseCanvas.transform.FindChild ("Image - Limit").GetComponent<Image> ().color = Color.red;
			gameManager.partyManager.limited = true;
		}
		else
		{
			pauseCanvas.transform.FindChild ("Image - Limit").GetComponent<Image> ().color = Color.green;
			gameManager.partyManager.limited = false;
		}
	}

	public void ClearWinsAndWaves()
	{
		foreach (Party p in gameManager.partyManager.parties)
		{
			p.winCount = 0;
		}

		gameManager.waveManager.wave = 0;

		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
	}

	public void QuitGame()
	{
		Application.Quit ();
	}
}
