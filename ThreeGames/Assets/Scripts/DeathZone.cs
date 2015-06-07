using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
	GameManager gameManager;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.gameObject.GetComponent<StayonHead> () != null)
		{
			if (collision.collider.gameObject.GetComponent<StayonHead> ().transform.parent.GetComponent<MP_Control>() != null)
			{
				gameManager.partyManager.RemoveMPFromParty (collision.collider.gameObject, collision.collider.gameObject.GetComponent<StayonHead> ().transform.parent.GetComponent<MP_Control>().party);
				Debug.Log (collision.collider.gameObject.name + " Entered Death Zone and Died.");
			}
		}
	}
}
