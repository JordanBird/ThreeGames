using UnityEngine;
using System.Collections;

public class StayonHead : MonoBehaviour {
	public GameObject HeadBone;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = HeadBone.transform.position;
	}
}
