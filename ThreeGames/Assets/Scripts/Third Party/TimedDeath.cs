using UnityEngine;
using System.Collections;

public class TimedDeath : MonoBehaviour
{
	public float timeToLive = 5;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (TimeDeath());
	}
	
	IEnumerator TimeDeath()
	{
		yield return new WaitForSeconds(timeToLive);

		Destroy (gameObject);
	}
}
