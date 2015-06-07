using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
	GameManager gameManager;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			//GetComponent<Text>().enabled = true;
			gameManager.partyManager.ACTION_Order ();
		}
//
//	if (Input.GetKeyUp (KeyCode.Space)) {
//			GetComponent<Text>().enabled = false;
//		}
	}

	public IEnumerator ShowSign()
	{
		GetComponent<Text>().enabled = true;

		yield return new WaitForSeconds(0.5f);

		GetComponent<Text>().enabled = false;
	}
}
