using UnityEngine;
using System.Collections;

public class LookAtMainCamera : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		//StartCoroutine (Look ()); //Disabled for perfomance reasons.
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	/// <summary>
	/// Advised agaisnt using for performance issues.
	/// </summary>
	IEnumerator Look()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.001f);

			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}
	}
}
