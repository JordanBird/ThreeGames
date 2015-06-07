using UnityEngine;
using System.Collections;

public class PunchForce : MonoBehaviour
{
	public GameManager gameManager;

	[SerializeField]GameObject Target;
	Vector3 Dir;
	public GameObject Home;
	float Dis;
	Vector3 Head;
	Rigidbody Rb;
	MP_Control mp;
	float PunchDelay = 0;
	public string Party = "";
	public string TarParty;

	private GameObject target;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();

		Rb = GetComponent<Rigidbody> ();
		mp = GetComponentInParent<MP_Control> ();

		Home = GameObject.Find (mp.Party + "Home");
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Enable this if MPs stick to the floor too much.
		//Rb.AddForce (Vector3.up * 50);
		PunchDelay += 1 * Time.deltaTime;
	
		if (PunchDelay >= Random.Range(1.0f,10.0f))
		{
			if (target == null)
			{
				Party targetParty = gameManager.partyManager.parties[Random.Range (0, gameManager.partyManager.parties.Length)];

				if (targetParty.name == Party) //Allow it to still happen but lessen the chance.
					targetParty = gameManager.partyManager.parties[Random.Range (0, gameManager.partyManager.parties.Length)];

				TarParty = targetParty.name;

				GameObject[] MPList = targetParty.mps.ToArray ();
				
				if (MPList.Length > 0)
				{
					Target = MPList[Random.Range(0,MPList.Length)];
					foreach (Transform child in Target.GetComponentInChildren<Transform>()) {
						if (child.tag == "Target"){
							Target = child.gameObject;}
					} 
					//Head = Target.transform.position - transform.position;
					//Dis = Target.transform.position.magnitude;
					//Dir = Head / Dis;
					//Rb.AddForce (Dir * 50500);
				}
				else
					PunchDelay = 10;

				try
				{
					Rb.AddForce((Target.transform.position - transform.position).normalized * 20000);
					PunchDelay = 0;
				}
				catch
				{
					PunchDelay = 10;
				}
			}

		}

//		if (Input.GetKeyDown (KeyCode.Space)) {
//
//			//Head = Home.transform.position - transform.position;
//			//Dis = Home.transform.position.magnitude;
//			//Dir = Head / Dis;
//			//Rb.AddForce (Dir * 50500);
//			Rb.AddForce((Home.transform.position - transform.position).normalized * 30000);
//		}
	}

	public void Order()
	{
		Rb.AddForce((Home.transform.position - transform.position).normalized * 30000);
	}

	public void AddForce(Vector3 force)
	{
		Rb.AddForce (force);
	}

	public Vector3 GetVelcoity()
	{
		return Rb.velocity;
	}
}
