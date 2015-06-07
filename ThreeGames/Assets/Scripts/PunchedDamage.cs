using UnityEngine;
using System.Collections;

public class PunchedDamage : MonoBehaviour
{
	public GameObject Parent;
	public string Party;

	public Party party;

	// Use this for initialization
	void Start ()
	{
		party = GetComponentInParent<MP_Control> ().party;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.transform.tag == "Fist" && other.gameObject.GetComponent<PunchForce>().Party != Party)
		{
			//Debug.Log (Party+other.gameObject.GetComponent<PunchForce>().Party);
			int damage = Random.Range (1, 4);
			Parent.GetComponent<MP_Control> ().DealDamage (damage);
			Parent.GetComponent<MP_Control> ().HitsPerSecond++;

			//Spawn a hit marker.
			if (damage >= 3)
				FindObjectOfType<PartyManager>().SpawnHitMarker (party, transform.position);
		}
	}
}
