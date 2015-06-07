using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
	public GameManager gameManager;

	public GameObject mpPrefab;

	public GameObject deathEffect;
	public GameObject[] hitEffects;

	public Party[] parties;

	public List<PartyTweetData> partyRelatedTweets = new List<PartyTweetData>();

	public Vector3 boundsCentre = Vector3.zero;
	public float maxBoundsDistace = 20;

	private float lastOrder = 0;

	public AudioClip[] punchSounds;

	public bool limited = false;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();

		string [] conKey = new string[] { "David Cameron" };
		string [] greKey = new string[] { "Natalie Bennett" };
		string [] labKey = new string[] { "Ed Miliband" };
		string [] libKey = new string[] { "Nick Clegg" };
		string [] ukipKey = new string[] { "Nigel Farage" };
		string [] snpKey = new string[] { "Alex Salmond", "Nicola Sturgeon" };
		string [] plaidKey = new string[] { "Leanne Wood" };

		string [] conNam = new string[] { "Conservatives", "Tories" };
		string [] greNam = new string[] { "Green Party", "Green" };
		string [] labNam = null;
		string [] libNam = new string[] { "Lib Dems", "Lib Dem", "LibDems", "LibDem" };
		string [] ukipNam = new string[] { "UKIP" };
		string [] snpNam = new string[] { "SNP" };
		string [] plaidNam = new string[] { "Plaid", "Cymru" };

		Party conservative = new Party ("Conservative", new Color(0, 0.5294117647f, 0.86274509803f), conNam, conKey, null);
		Party green = new Party ("Green", new Color(0.41568627451f, 0.69019607843f, 0.13725490196f), greNam, greKey, null);
		Party labour = new Party ("Labour", new Color(0.86274509803f, 0.14117647058f, 0.12156862745f), labNam, labKey, null);
		Party libDems = new Party ("Liberal Democrats", new Color(1, 0.49803921568f, 0), libNam, libKey, null);
		Party ukip = new Party ("UK Independance Party", new Color(0.50196078431f, 0, 0.50196078431f), ukipNam, ukipKey, null);
		Party snp = new Party ("Scottish National Party", new Color(1, 1, 0), snpNam, snpKey, null);
		Party plaid = new Party ("Plaid Cymru", new Color(0, 0.50588235294f, 0.25882352941f), plaidNam, plaidKey, null);

		//Populate parties array with created parties.
		parties = new Party[] { conservative, green, labour, libDems, ukip, snp, plaid };

		//Set old scores in case of crash.
		foreach (Party p in parties)
		{
			if (PlayerPrefs.HasKey (p.name + " Wins"))
			{
				p.winCount = PlayerPrefs.GetInt (p.name + " Wins");
			}
		}

		//Instantiate the GUI
		gameManager.guimMainGame.SetPartyScoringObjects (parties);

		Seat[] seats = GameObject.FindObjectsOfType<Seat> ();

		int seatsForParties = Mathf.FloorToInt(seats.Length / parties.Length);

		int currentParty = 0;
		int currentSeats = 0;

		for (int i = 0; currentParty < parties.Length; i++)
		{
			parties[currentParty].seats.Add (seats[i]);

			currentSeats++;

			if (currentSeats >= seatsForParties)
			{
				currentSeats = 0;
				currentParty++;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void SpawnMPs()
	{
		int limit = 1;

		if (limited)
			limit = 2;

		if (partyRelatedTweets.Count == 0)
		{
			Debug.Log ("Rate Limited. Spawning Polls");

			for (int i = 0; i < 43 / limit; i++)
			{
				SpawnMP(parties[0]);
			}

			for (int i = 0; i < 42 / limit; i++)
			{
				SpawnMP(parties[2]);
			}

			for (int i = 0; i < 4 / limit; i++)
			{
				SpawnMP(parties[3]);
			}

			for (int i = 0; i < 4 / limit; i++)
			{
				SpawnMP(parties[5]);
			}

			SpawnMP(parties[1]);
			SpawnMP(parties[4]);
		}
		else
		{
			for (int i = 0; i < partyRelatedTweets.Count / limit; i++)
			{
				SpawnMP (partyRelatedTweets[i].party);
			}
		}
	}

	public void PopulatePartiesWithCount(Tweet[] tweets)
	{
		partyRelatedTweets.Clear ();

		for (int i = 0; i < tweets.Length; i++)
		{
			foreach (Party party in parties)
			{
				if (tweets[i].text.ToUpper ().Contains(party.name.ToUpper ()))
					IncrementPartyCount(party, tweets[i]);
				
				if (party.nameVariations != null)
				{
					foreach (string s in party.nameVariations)
					{
						if (tweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							IncrementPartyCount(party, tweets[i]);
					}
				}
				
				if (party.keywords != null)
				{
					foreach (string s in party.keywords)
					{
						if (tweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							IncrementPartyCount(party, tweets[i]);
					}
				}
			}
		}
	}

	private void IncrementPartyCount(Party party, Tweet tweet)
	{
		party.count++;
		partyRelatedTweets.Add (new PartyTweetData (tweet, party));
	}

	public GameObject SpawnMP(Party party)
	{
		GameObject home = party.seats [Random.Range (0, party.seats.Count)].gameObject;
		
		Vector3 position = Vector3.zero;
		
		if (home != null)
		{
			float x = home.transform.position.x + Random.Range (-(home.transform.localScale.x / 2), (home.transform.localScale.x / 2));
			float y = home.transform.position.y + Random.Range (-(home.transform.localScale.y / 2), (home.transform.localScale.y / 2));
			float z = home.transform.position.z + Random.Range (-(home.transform.localScale.z / 2), (home.transform.localScale.z / 2));

			position = new Vector3(x, y, z);
		}
		
		GameObject mp = Instantiate (mpPrefab, position, mpPrefab.transform.rotation) as GameObject;
		mp.GetComponent<MP_Control> ().Party = party.name;
		mp.GetComponent<MP_Control> ().party = party;
		mp.GetComponentInChildren<PunchedDamage>().Party = party.name;

		foreach (PunchForce pf in mp.GetComponentsInChildren<PunchForce>())
		{
			pf.Party = party.name;
		}
		
		foreach (Renderer r in mp.GetComponentsInChildren<Renderer>())
		{
			if (r.tag == "Colourable Item")
				r.material.color = party.colour;
		}

		mp.transform.parent = gameManager.dynamicObjectHolder.transform;

		party.mps.Add (mp);

		//Update GUI
		gameManager.guimMainGame.UpdateScoreCard (party);

		return mp;
	}

	public void RemoveMPFromParty(GameObject mp, Party party)
	{
		int partiesWithRemainingMPs = 0;

		party.mps.Remove (mp);

		try
		{
			GameObject death = Instantiate (deathEffect, mp.transform.FindChild ("HeadTarget").transform.position, Quaternion.identity) as GameObject;
			death.transform.SetParent (gameManager.dynamicObjectHolder.transform);
			death.GetComponent<ParticleSystem>().startColor = GetParty (mp.GetComponent<MP_Control>().Party).colour;
		}
		catch {}

		foreach (Party p in parties)
		{
			if (p.mps.Count > 0)
				partiesWithRemainingMPs++;

			//Update GUI
			gameManager.guimMainGame.UpdateScoreCard (p);
		}

		if (partiesWithRemainingMPs < 2 && gameManager.waveManager.waveInProgress)
		{
			gameManager.waveManager.EndWave ();
			gameManager.waveManager.StartWave ();
		}
	}

	public void SpawnHitMarker(Party party, Vector3 position)
	{
		if (hitEffects.Length > 0)
		{
			GameObject g = Instantiate(hitEffects[Random.Range (0, hitEffects.Length)], position, Quaternion.identity) as GameObject;
			g.GetComponent<Renderer>().material.color = party.colour;
		}
	}

	public Party GetParty(string party)
	{
		foreach (Party p in parties)
		{
			if (p.name.ToUpper() == party.ToUpper ())
				return p;
		}

		return null;
	}

	public void ACTION_Order()
	{
		StartCoroutine (FindObjectOfType<Order> ().ShowSign ());

		if (lastOrder < Time.time - 2.5f)
		{
			lastOrder = Time.time;

			foreach (Party p in parties)
			{
				for (int i = 0; i < p.mps.Count; i++)
				{
					if (p.mps[i] != null)
					{
						//Home.transform.position - transform.position).normalized * 30000
						p.mps[i].GetComponent<MP_Control>().AddForce ((p.seats[Random.Range (0, p.seats.Count)].transform.position - transform.position).normalized * 30000);
					}
				}
			}
		}
	}

	IEnumerator OutOfBoundsCheck()
	{
		while (true)
		{
			foreach (Party p in parties)
			{
				for (int i = 0; i < p.mps.Count; i++)
				{
					if (Vector3.Distance (p.mps[i].transform.position, boundsCentre) > maxBoundsDistace)
					{
						Debug.Log (p.mps[i] + " is out of bounds. Destroyed.");
						RemoveMPFromParty (p.mps[i], p);
						i--;
					}
				}
			}

			yield return new WaitForSeconds(5);
		}
	}
}
