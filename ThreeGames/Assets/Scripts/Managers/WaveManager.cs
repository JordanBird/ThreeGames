using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
	public GameManager gameManager;

	public System.DateTime lastScannedTweet = new System.DateTime (1970, 1, 1);
	public System.DateTime highestThisRunthough = new System.DateTime(1970, 1, 1);

	public float timeLimit = 240;
	public float timeLeft = 0;
	public int wave = 0;

	public bool waveInProgress = false;

	public static string userTweets = "#NVAElection";
	public static string electionHashtag = "#GE2015";

	public List<UserAction> userActions = new List<UserAction>();

	public int DEBUG_twitterSearchCheck = 0;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();

		if (PlayerPrefs.HasKey ("Election Hashtag"))
			electionHashtag = PlayerPrefs.GetString ("Election Hashtag");
		else
			PlayerPrefs.SetString ("Election Hashtag", electionHashtag);

		//Add User Actions to List
		UserAction order = new UserAction ("Order", UserOrder);
		userActions.Add (order);

		//Setup Times
		lastScannedTweet = System.DateTime.Now;
		highestThisRunthough = System.DateTime.Now;

		if (PlayerPrefs.HasKey ("Waves"))
			wave = PlayerPrefs.GetInt ("Waves") - 1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (waveInProgress)
		{
			timeLeft -= Time.deltaTime;

			//UpdateGUI
			System.TimeSpan span = System.TimeSpan.FromSeconds (timeLeft);
			gameManager.guimMainGame.UpdateTimeLeft (span.Minutes.ToString ("D2") + ":" + span.Seconds.ToString ("D2"));

			if (timeLeft <= 0)
			{
				Timeout ();
				StartWave ();
			}
		}
	}

	public void StartWave()
	{
		//This is where you specify the search term for the tweets you want. Result type can be 'new', 'popular' or 'mixed'.
		StartCoroutine (GetTweets(gameManager.twitterManager.CreateSearchUserWWW (electionHashtag, "new")));
	}

	public void SpawnWave(string tweets)
	{
		wave++;
		timeLeft = timeLimit;
		waveInProgress = true;

		//UpdateGUI
		gameManager.guimMainGame.UpdateWaveNumber (wave.ToString ());

		gameManager.partyManager.PopulatePartiesWithCount(Twitter.ParseSearchResults (tweets));
		gameManager.partyManager.SpawnMPs ();

		StartCoroutine (SearchForUserTweets());
	}

	public void EndWave()
	{
		StopCoroutine (SearchForUserTweets());

		waveInProgress = false;

		foreach (Party p in gameManager.partyManager.parties)
		{
			if (p.mps.Count > 0)
			{
				p.winCount++;

				foreach (GameObject g in p.mps)
				{
					Destroy (g);
				}

				p.mps.Clear ();

				//Update GUI
				gameManager.guimMainGame.UpdateScoreCard (p);
			}
		}
	}

	/// <summary>
	/// Called when time runs out for a wave. Clears all MPs.
	/// </summary>
	public void Timeout()
	{
		waveInProgress = false;

		foreach (Party p in gameManager.partyManager.parties)
		{
			if (p.mps.Count > 0)
			{
				foreach (GameObject g in p.mps)
				{
					Destroy (g);
				}
				
				p.mps.Clear ();
			}
		}
	}

	/// <summary>
	/// Deal with the election tweets and spawn a wave based on those tweets.
	/// </summary>
	/// <returns>The tweets.</returns>
	/// <param name="www">Www.</param>
	private IEnumerator GetTweets(WWW www)
	{
		yield return www;
		
		string parseValue = "";
		
		// check for errors
		if (www.error == null)
		{
			parseValue = www.text;
		}
		else
		{
			Debug.Log("WWW Error: "+ www.error);
		}
		
		//Success
		SpawnWave (parseValue);
	}

	/// <summary>
	/// Deal with the tweets found from searching the user hashtag.
	/// </summary>
	/// <returns>The user tweets.</returns>
	/// <param name="www">Www.</param>
	private IEnumerator GetUserTweets(WWW www)
	{
		yield return www;
		
		string parseValue = "";
		
		// check for errors
		if (www.error == null)
		{
			parseValue = www.text;
		}
		else
		{
			Debug.Log("WWW Error: "+ www.error);
		}
		
		//Success
		Tweet[] foundTweets = Twitter.ParseSearchResults (parseValue);
		
		highestThisRunthough = lastScannedTweet;
		
		for (int i = 0; i < foundTweets.Length; i++)
		{
			if (foundTweets[i].createdAt <= lastScannedTweet)
				continue;

			if (foundTweets[i].createdAt > highestThisRunthough)
				highestThisRunthough = foundTweets[i].createdAt;
			
			foreach (Party party in gameManager.partyManager.parties)
			{
				if (foundTweets[i].text.ToUpper ().Contains(party.name.ToUpper ()))
				{
					SpawnUserMP (party);
					break;
				}

				if (party.nameVariations != null)
				{
					foreach (string s in party.nameVariations)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
						{
							SpawnUserMP (party);
							break;
						}
					}
				}
				
				if (party.keywords != null)
				{
					foreach (string s in party.keywords)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
						{
							SpawnUserMP (party);
							break;
						}
					}
				}
			}

			foreach (UserAction action in userActions)
			{
				if (foundTweets[i].text.ToUpper ().Contains(action.name.ToUpper ()))
				    action.runAction();
			}
		}
		
		lastScannedTweet = highestThisRunthough;
	}

	/// <summary>
	/// Spawns a stronger MP as this should be called as a result of someone using the hashtag to spawn an MP.
	/// </summary>
	/// <param name="party">Party.</param>
	private void SpawnUserMP(Party party)
	{
		GameObject mp = gameManager.partyManager.SpawnMP (party);

		mp.transform.localScale = new Vector3 (65f, 65f, 65f);
		mp.GetComponent<MP_Control> ().SetHealth (150);

		//Update GUI
		gameManager.guimMainGame.UpdateScoreCard (party);
		StartCoroutine (gameManager.guimMainGame.FlashPartyColour (party));
	}

	/// <summary>
	/// Searches for user tweets.
	/// </summary>
	/// <returns>The for user tweets.</returns>
	IEnumerator SearchForUserTweets()
	{
		while (waveInProgress)
		{
			//Debug.Log ("Starting Coroutine.");
			StartCoroutine (GetUserTweets(gameManager.twitterManager.CreateSearchUserWWW (userTweets, "new")));
			yield return new WaitForSeconds(25);//Every 25 seconds search for user tweets. This should follow Twitter's API limit of 180 searchs / 15 minutes. As that should give us 20 seconds.
		}
	}

	/// <summary>
	/// Simulates users tweeting to the user hashtag.
	/// </summary>
	/// <returns>The g_ simulate user tweets.</returns>
	public IEnumerator DEBUG_SimulateUserTweets()
	{
		while (waveInProgress)
		{
			if (gameManager.partyManager.parties.Length > 0)
			{
				for (int i = 0; i < Random.Range (0, 5); i++)
				{
					Party party = gameManager.partyManager.parties [Random.Range (0, gameManager.partyManager.parties.Length)];
					
					SpawnUserMP (party);
				}
			}

			yield return new WaitForSeconds(30);
		}
	}

	public void UserOrder()
	{
		gameManager.partyManager.ACTION_Order ();
	}
}
