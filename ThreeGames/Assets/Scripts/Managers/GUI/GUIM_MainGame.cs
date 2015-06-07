using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIM_MainGame : MonoBehaviour
{
	public GameManager gameManager;

	public Canvas mainGameCanvas;
	public GameObject partyScoringPrefab;
	public GameObject tickerItemPrefab;

	private GameObject[] partyScores;

	private string[] tickerItems;
	private List<GameObject> activeTickerItems = new List<GameObject>();

	private float tickerSpeed = 35f;
	private float newsTickerSpacing = 200;

	private int lastTickerItem = 0;

	private int currentParty = 0;
	private int nextParty = 0;
	private float progress = 1;
	private float holdColour = 1;

	// Use this for initialization
	void Start ()
	{
		string file = Resources.Load<TextAsset> ("Ticker Items").text;
		string[] lines = file.Split (new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);

		List<string> items = new List<string> ();

		for (int i = 0; i < lines.Length; i++)
		{
			if (!lines[i].StartsWith ("<!") && lines[i] != null && lines[i] != "")
				items.Add(lines[i]);
		}

		tickerItems = items.ToArray ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate()
	{
		while (activeTickerItems.Count < 5)
		{
			RectTransform tickerPanel = mainGameCanvas.transform.FindChild ("Panel - Ticker").GetComponent<RectTransform>();

			GameObject item = Instantiate (tickerItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			item.transform.SetParent (tickerPanel.transform, false);

			int index = Random.Range (0, tickerItems.Length);

			while (index == lastTickerItem && tickerItems.Length > 1)
			{
				index = Random.Range (0, tickerItems.Length);
			}

			lastTickerItem = index;

			item.GetComponent<Text>().text = tickerItems[index];

			float itemWidth  = item.GetComponent<RectTransform>().rect.width;

			if (activeTickerItems.Count == 0)
			{
				item.GetComponent<RectTransform>().anchoredPosition = new Vector2((mainGameCanvas.pixelRect.width / 2) + itemWidth / 2, 0);
			}
			else
			{
				RectTransform previousItem = activeTickerItems[activeTickerItems.Count - 1].GetComponent<RectTransform>();
				item.GetComponent<RectTransform>().anchoredPosition = new Vector2(previousItem.anchoredPosition.x + itemWidth + (newsTickerSpacing * activeTickerItems.Count), 0);
			}

			activeTickerItems.Add (item);
		}

		for (int i = 0; i < activeTickerItems.Count; i++)
		{
			if (i == 0)
				activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.x - (tickerSpeed * Time.deltaTime), activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.y);
			else
			{
				float xPosPrevious = activeTickerItems[i - 1].GetComponent<RectTransform>().anchoredPosition.x;
				float widthPrevious = activeTickerItems[i - 1].GetComponent<RectTransform>().rect.width / 2;
				float widthSelf = activeTickerItems[i].GetComponent<RectTransform>().rect.width / 2;
				float speedIncrease = (tickerSpeed * Time.deltaTime);

				activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosPrevious + widthPrevious + widthSelf + speedIncrease + newsTickerSpacing, activeTickerItems[i - 1].GetComponent<RectTransform>().anchoredPosition.y);
			}

			if (activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.x < -(mainGameCanvas.pixelRect.width + activeTickerItems[i].GetComponent<RectTransform>().rect.width / 2))
			{
				Destroy (activeTickerItems[i]);
				activeTickerItems.RemoveAt (i);
				i--;
			}
		}

		//Colour the ticker.
		if (progress >= 1)
		{
			if (holdColour >= 1)
			{
				currentParty = nextParty;
				nextParty = Random.Range (0, FindObjectOfType<PartyManager>().parties.Length);
				
				progress = 0;
				holdColour = 0;
			}
			else
				holdColour += Time.deltaTime * (0.1f);
		}
		else
		{
			Color newColour = Color.Lerp (FindObjectOfType<PartyManager>().parties[currentParty].colour, FindObjectOfType<PartyManager>().parties[nextParty].colour, progress);
			
			mainGameCanvas.transform.FindChild ("Panel - Ticker").GetComponent<Image>().color = newColour;
			mainGameCanvas.transform.FindChild ("Image - Ticker Bar").GetComponent<Image>().color = newColour;
			mainGameCanvas.transform.FindChild ("Text - Wave Timer").GetComponent<Outline>().effectColor = newColour;
			mainGameCanvas.transform.FindChild ("Text - Wave #").GetComponent<Outline>().effectColor = newColour;
		}
		
		progress += Time.deltaTime * (0.05f);
	}

	public void SetPartyScoringObjects(Party[] parties)
	{
		partyScores = new GameObject[parties.Length];

		float height = mainGameCanvas.pixelRect.height;

		for (int i = 0; i < parties.Length; i++)
		{
			//Spawn
			GameObject panel = Instantiate (partyScoringPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			panel.transform.SetParent (mainGameCanvas.transform.FindChild ("Party Scores").transform, false);

			//Position
			panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 + (i * -(height / (parties.Length + 1))));

			//Customise
			panel.transform.FindChild ("Image - Party Colour").GetComponent<Image>().color = parties[i].colour;
			panel.transform.FindChild ("Text - MPs").GetComponent<Text>().text = "0";
			panel.transform.FindChild ("Text - Wave Wins").GetComponent<Text>().text = "0";
			panel.transform.FindChild ("Text - Party").GetComponent<Text>().text = parties[i].name;
			panel.name = "Party Score: " + parties[i].name;

			RectTransform t = panel.GetComponent<RectTransform>();
			RectTransform pt = panel.transform.parent.GetComponent<RectTransform>();
			
			if(t == null || pt == null) return;
			
			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);
			
			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);

			partyScores[i] = panel;
		}
	}

	public void UpdateScoreCard(Party party)
	{
		foreach (GameObject g in partyScores)
		{
			if (g.name == "Party Score: " + party.name)
			{
				g.transform.FindChild ("Text - MPs").GetComponent<Text>().text = party.mps.Count.ToString ();
				g.transform.FindChild ("Text - Wave Wins").GetComponent<Text>().text = party.winCount.ToString ();

				PlayerPrefs.SetInt (party.name + " Wins", party.winCount);
				PlayerPrefs.Save ();

				return;
			}
		}
	}

	public void UpdateTimeLeft(string timeLeft)
	{
		mainGameCanvas.transform.FindChild ("Text - Wave Timer").GetComponent<Text> ().text = timeLeft + " Left";
	}

	public void UpdateWaveNumber(string wave)
	{
		mainGameCanvas.transform.FindChild ("Text - Wave #").GetComponent<Text> ().text = "Wave: " + wave;

		PlayerPrefs.SetInt ("Waves", int.Parse (wave));
		PlayerPrefs.Save ();
	}

	public IEnumerator FlashPartyColour(Party party)
	{
		Text text = null;
		Text winText = null;

		foreach (GameObject g in partyScores)
		{
			if (g.name == "Party Score: " + party.name)
			{
				text = g.transform.FindChild ("Text - Party").GetComponent<Text> ();
				text.text = text.text + "++";
				text.color = party.colour;
				text.fontStyle = FontStyle.Bold;

				winText = g.transform.FindChild ("Text - Wave Wins").GetComponent<Text> ();
				winText.color = party.colour;
				winText.fontStyle = FontStyle.Bold;
				break;
			}
		}

		if (text != null)
		{
			bool changing = true;
			float progress = 0;
			
			while (changing)
			{
				progress += 0.005f;
				
				text.color = Color.Lerp (text.color, Color.white, progress);
				winText.color = Color.Lerp (text.color, Color.white, progress);

				if (text.color == Color.white)
					changing = false;

				yield return new WaitForSeconds(0.05f);
			}
		}

		text.text = text.text.Replace ("++", "");
		text.fontStyle = FontStyle.Normal;

		winText.fontStyle = FontStyle.Normal;
	}
}
