using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public TwitterManager twitterManager;
	public PartyManager partyManager;
	public WaveManager waveManager;

	public GUIM_MainGame guimMainGame;

	public GameObject dynamicObjectHolder;

	// Use this for initialization
	void Start ()
	{
		//Find the Twitter Manager in the scene.
		twitterManager = FindObjectOfType<TwitterManager> ();
		partyManager = FindObjectOfType<PartyManager> ();
		waveManager = FindObjectOfType<WaveManager> ();

		guimMainGame = FindObjectOfType<GUIM_MainGame> ();

		dynamicObjectHolder = GameObject.Find("Dynamic Objects");

		if (dynamicObjectHolder == null)
			dynamicObjectHolder = new GameObject ("Dynamic Objects");

		//Jordan Says: Scraping here might be good for more data and key words: http://www.bbc.co.uk/news/election/2015/manifesto-guide

		//DEBUG: To leave game running.
		Application.runInBackground = true;

		Cursor.visible = false;

		//Launch the Game
		StartGame ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private void StartGame()
	{
		waveManager.StartWave ();
	}
}
