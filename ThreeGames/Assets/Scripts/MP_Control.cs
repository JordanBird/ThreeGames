using UnityEngine;
using System.Collections;

public class MP_Control : MonoBehaviour
{
	public GameManager gameManager;

	public Party party;

	public string Party;
	private float Health = 100;
	public float HitsPerSecond { get; set; }

	private PunchForce[] punchForces;

	private Vector3 oldScale = Vector3.one;

	public GameObject target = null;

	AudioSource audioSource;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
		punchForces = GetComponentsInChildren<PunchForce> ();
		audioSource = GetComponent<AudioSource> ();

		StartCoroutine (ScaleSpawn ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		HitsPerSecond -= Time.deltaTime;
		if (HitsPerSecond < 0)
			HitsPerSecond = 0;

//		if (target == null)
//		{
//			Party targetParty = gameManager.partyManager.parties[Random.Range (0, gameManager.partyManager.parties.Length)];
//			
//			if (targetParty.name == Party) //Allow it to still happen but lessen the chance.
//				targetParty = gameManager.partyManager.parties[Random.Range (0, gameManager.partyManager.parties.Length)];
//			
//			string tarParty = targetParty.name;
//			
//			GameObject[] MPList = targetParty.mps.ToArray ();
//			
//			if (MPList.Length > 0)
//			{
//				target = MPList[Random.Range(0,MPList.Length)];
//				foreach (Transform child in target.GetComponentInChildren<Transform>()) {
//					if (child.tag == "Target"){
//						target = child.gameObject;}
//				} 
//				//Head = Target.transform.position - transform.position;
//				//Dis = Target.transform.position.magnitude;
//				//Dir = Head / Dis;
//				//Rb.AddForce (Dir * 50500);
//			}
//			else
//				PunchDelay = 6;
//		}
	}

	public void DealDamage(float amount)
	{
		if (amount >= 3)
		{
			if (gameManager.partyManager.punchSounds.Length > 0)
			{
				audioSource.clip = gameManager.partyManager.punchSounds[Random.Range (0, gameManager.partyManager.punchSounds.Length)];
				audioSource.Play ();
			}
		}

		Health -= amount;

		if (Health <= 0)
			Dead ();
	}

	public float GetHealth()
	{
		return Health;
	}

	public void SetHealth(float value)
	{
		Health = value;
	}

	public void Dead()
	{
		FindObjectOfType<GameManager> ().partyManager.RemoveMPFromParty (gameObject, party); //Search rather than cache to save space.

		Destroy (this.gameObject);
		// Maybe put increment a total of what ever party just got killed.
	}

	public void Order()
	{
		try
		{
			foreach (PunchForce pf in punchForces)
			{
				pf.Order();
			}
		}
		catch {}
	}

	public void AddForce(Vector3 force)
	{
		foreach (PunchForce pf in punchForces)
		{
			pf.AddForce (force);
		}
	}

	IEnumerator ScaleSpawn()
	{
		oldScale = transform.localScale;
		transform.localScale = Vector3.one * 0.01f;

		Vector3 jump = oldScale * 0.1f;

		float progress = 0;

		while (transform.localScale != oldScale + jump)
		{
			progress += 0.1f;
			transform.localScale = Vector3.Lerp(transform.localScale, oldScale + jump, progress);

			yield return new WaitForSeconds(0.01f);
		}

		progress = 0;
		while (transform.localScale != oldScale)
		{
			progress += 0.1f;
			transform.localScale = Vector3.Lerp(transform.localScale, oldScale, progress);
			
			yield return new WaitForSeconds(0.01f);
		}

		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
		{
			rb.isKinematic = false;
		}
	}
}
