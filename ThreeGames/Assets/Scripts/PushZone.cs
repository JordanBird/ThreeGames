using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PushZone : MonoBehaviour
{
	public Vector3 pushDirection = Vector3.zero;

	List<GameObject> colliding = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (Push ());
	}
	
	// Update is called once per frame
	void Update ()
	{		
	}
	
	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.GetComponentInParent<MP_Control>() != null)
		{
			if (!colliding.Contains (collision.gameObject.GetComponentInParent<MP_Control>().gameObject))
				colliding.Add (collision.gameObject.GetComponentInParent<MP_Control>().gameObject);
		}
	}

	void OnTriggerExit(Collider collision)
	{
		if (collision.gameObject.GetComponentInParent<MP_Control>() != null)
		{
			colliding.Remove (collision.gameObject.GetComponentInParent<MP_Control>().gameObject);
		}
	}

	IEnumerator Push()
	{
		while (true)
		{
			for (int i = 0; i < colliding.Count; i++)
			{
				if (colliding[i] != null)
					colliding[i].GetComponent<MP_Control> ().AddForce (pushDirection);
				else
				{
					colliding.RemoveAt (i);
					i--;
				}
			}
			
			yield return new WaitForSeconds(1);
		}
	}
}