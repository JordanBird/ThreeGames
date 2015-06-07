using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmartCamera : MonoBehaviour
{
	public GameManager gameManager;
	
	private Camera camera;
	
	#region Orbiting
	public Vector3 OrbitPosition;
	public float OrbitDistance;
	public float OrbitSpeed;
	#endregion
	
	#region Focusing
	private bool focused = false;
	private Transform focus = null;
	private Vector3 lastFocusPosition;
	private Vector3 currentFocusPosition;
	public Vector3 FocusDefaultPosition;
	
	private float focusFollowTimer;
	private float focusFollowDuration;
	public  float FocusFollowDurationMin, FocusFollowDurationMax;
	public float FocusFollowSmoothness;
	
	private float focusChangeTimer;
	private float focusChangeDuration;
	public float FocusChangeSmoothness;
	
	private float lastFocusFOV;
	private float currentFocusFOV;
	public float FocusFOVMin, FocusFOVMax;
	
	public bool AutoFocus;
	public float AutoFocusInterestThreshold;
	public int FocusChance;
	#endregion
	
	#region Shaking
	public Vector3 ShakeAmplitude;
	public Vector3 ShakeSpeed;
	
	// Scales both the above variables uniformly.
	public float ShakeAmplitudeScale;
	public float ShakeSpeedScale;
	#endregion
	
	public Vector3 percentageCentre = Vector3.zero;
	
	void Awake()
	{
		camera = GetComponent<Camera>();
		currentFocusFOV = lastFocusFOV = FocusFOVMax;
		focusFollowTimer = focusFollowDuration = FocusFollowDurationMax;
	}
	
	void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		//StartCoroutine (PercentageCentre ()); //Disabled for performance reasons.
	}
	
	void Update()
	{
		Orbit ();
		Focus ();
		Shake ();
	}
	
	void Orbit()
	{
		float ox = OrbitDistance * Mathf.Sin (Time.time * OrbitSpeed);
		float oz = OrbitDistance * Mathf.Cos (Time.time * OrbitSpeed);
		transform.position = OrbitPosition + new Vector3(ox, 0, oz);
	}
	
	void Focus()
	{
		if (AutoFocus)
		{
			// Change focus if the follow timer has finished.
			focusFollowTimer += Time.deltaTime;
			if (focusFollowTimer >= focusFollowDuration || focused && focus == null)
			{
				// Automatically find the next thing to focus on.
				if (Random.Range (0, 100) < FocusChance)
					DetermineFocus();
				else
					SetFocus(null);
				
				Debug.Log (focus);
			}
		}
		
		// Follow the focus smoothly.
		float interpolation = Mathf.SmoothStep (0.0f, 1.0f, Mathf.Min (focusChangeTimer / focusChangeDuration, 1.0f));
		
		// Look at the intermediate position between the last focus position and the new focus.
		Vector3 nextFocusPosition = currentFocusPosition;
		if (focus != null)
		{
			nextFocusPosition = Vector3.Lerp (lastFocusPosition, focused ? focus.position : FocusDefaultPosition, interpolation);
		}
		else
		{
			nextFocusPosition = Vector3.Lerp (lastFocusPosition, FocusDefaultPosition, interpolation);
		}
		// Do the cheaty lerp to make it even smoother.
		currentFocusPosition = Vector3.Lerp (currentFocusPosition, nextFocusPosition, 0.3f);
		
		transform.LookAt(currentFocusPosition);
		
		float nextFocusFOV = currentFocusFOV;
		if (focused)
		{
			// Zoom out then in.  The maximum zoomed out is scaled by the angle between focal points.
			nextFocusFOV = Mathf.Lerp (Mathf.Lerp (lastFocusFOV, FocusFOVMin, interpolation), Mathf.Lerp (FocusFOVMin, FocusFOVMax, focusChangeDuration / FocusChangeSmoothness), Mathf.Sin(interpolation * Mathf.PI));
		}
		else
		{
			// Zoom out.
			nextFocusFOV = Mathf.Lerp (lastFocusFOV, FocusFOVMax, interpolation);
		}
		// Do the cheaty lerp to make it even smoother.
		currentFocusFOV = Mathf.Lerp (currentFocusFOV, nextFocusFOV, 0.3f);
		
		camera.fieldOfView = currentFocusFOV; 
		
		focusChangeTimer += Time.deltaTime;
		if (focusChangeTimer > focusChangeDuration)
			focusChangeTimer = focusChangeDuration;
	}
	
	void Shake()
	{
		// Get random pitch and yaw based on smooth Perlin noise.
		float dp = ShakeAmplitudeScale * ShakeAmplitude.x * (Mathf.PerlinNoise (0, Time.time * ShakeSpeedScale * ShakeSpeed.x) - 0.5f);
		float dy = ShakeAmplitudeScale * ShakeAmplitude.y * (Mathf.PerlinNoise (Time.time  * ShakeSpeedScale * ShakeSpeed.y, 0) - 0.5f);
		float dr = ShakeAmplitudeScale * ShakeAmplitude.z * (Mathf.PerlinNoise (Time.time  * ShakeSpeedScale * ShakeSpeed.z, Time.time * ShakeSpeed.z) - 0.5f);
		
		// Apply the changes to the direction of the camera.
		transform.Rotate (dp, dy, dr);
	}
	
	public void SetFocus(Transform focus)
	{ 
		focused = focus != null;
		
		lastFocusPosition = currentFocusPosition;
		lastFocusFOV = currentFocusFOV;
		
		// Get the time it should take to change aim.
		float angle = Vector3.Angle (transform.forward, (focused ? focus.position : FocusDefaultPosition) - transform.position);
		focusChangeTimer = 0.0f;
		focusChangeDuration = Mathf.Clamp(focused ? angle / 90.0f * FocusChangeSmoothness : FocusChangeSmoothness * 0.5f, 1.0f, FocusChangeSmoothness * 0.1f);
		
		// Reset the follow timer.
		focusFollowTimer = 0.0f;
		focusFollowDuration = Random.Range (FocusFollowDurationMin, FocusFollowDurationMax);
		
		this.focus = focus;
		
	}
	
	public void SetFocus(Vector3 focus)
	{
		lastFocusPosition = currentFocusPosition;
		lastFocusFOV = currentFocusFOV;
		
		// Get the time it should take to change aim.
		float angle = Vector3.Angle (transform.forward, (focused ? focus : FocusDefaultPosition) - transform.position);
		focusChangeTimer = 0.0f;
		focusChangeDuration = focused ? angle / 90.0f * FocusChangeSmoothness : FocusChangeSmoothness * 0.5f;
		
		// Reset the follow timer.
		focusFollowTimer = 0.0f;
		focusFollowDuration = Random.Range (FocusFollowDurationMin, FocusFollowDurationMax);
	}
	
	void DetermineFocus()
	{
		Debug.Log ("Finding focus");
		GameManager gm = FindObjectOfType<GameManager>();
		
		// Copy the list of mps to a new array so that any changes to the list after yielding do not cause errors.
		// Get the most interesting position by getting the average position of hit mps but multiplying the positions by their HitsPerSecond cubed to shift the average position towards them.
		Vector3 mostInterestingPosition = Vector3.zero;
		Vector3 averagePosition = Vector3.zero;
		float totalInterest = 0.0f;
		int numMPs = 0;
		foreach (Party p in gm.partyManager.parties)
		{
			numMPs += p.mps.Count;
			
			// Access the mps through a normal for loop since the number of mps could change after yielding.
			for (int i = 0; i < p.mps.Count; ++i)
			{
				if (p.mps[i] != null)
				{
					averagePosition += p.mps[i].transform.position;
					
					MP_Control mpControl = p.mps[i].GetComponent<MP_Control>();
					
					// If the mp has not been hit recently, ignore it.
					if (mpControl.HitsPerSecond != 0)
					{
						float interest = Mathf.Pow (1 + mpControl.HitsPerSecond, 3);
						mostInterestingPosition += p.mps[i].transform.Find ("HeadTarget").position * interest;
						totalInterest += interest;
					}
				}
			}
		}
		
		// Rescale the average positions.
		mostInterestingPosition /= totalInterest;
		averagePosition /= numMPs;
		FocusDefaultPosition = percentageCentre;
		
		//Debug.Log ("Num MPS: " + numMPs + " | Interest: " + totalInterest);
		
		// If nothing specifically interesting is going on, just focus at the average position.
		if (totalInterest / numMPs < AutoFocusInterestThreshold)
		{
			Debug.Log ("Percentage Centre 1");
			SetFocus (percentageCentre);
		}
		else
		{
			// Get the most beat up MP nearest to the position.
			float bestDistance = float.MaxValue;
			Transform bestMP = null;
			foreach (Party p in gm.partyManager.parties)
			{
				for (int i = 0; i < p.mps.Count; ++i)
				{
					if (p.mps[i] != null)
					{
						MP_Control mpControl = p.mps[i].GetComponent<MP_Control>();
						if (mpControl.HitsPerSecond != 0)
						{
							float distance = Vector3.Distance(p.mps[i].transform.Find ("HeadTarget").position, mostInterestingPosition) / mpControl.HitsPerSecond;
							if (distance < bestDistance)
							{
								bestDistance = distance;
								bestMP = p.mps[i].transform;
							}
						}
					}
				}
			}
			
			SetFocus (bestMP == null ? null : bestMP.Find ("HeadTarget"));
		}
	}
	
	IEnumerator PercentageCentre()
	{
		while (true)
		{
			List<PositionCount> xPositionCounts = new List<PositionCount>();
			List<PositionCount> yPositionCounts = new List<PositionCount>();
			List<PositionCount> zPositionCounts = new List<PositionCount>();
			
			if (gameManager.partyManager == null)
			{
				yield return null;
				continue;
			}
			
			foreach (Party p in gameManager.partyManager.parties)
			{
				foreach (GameObject g in p.mps)
				{
					int x = Mathf.RoundToInt (g.transform.FindChild ("HeadTarget").transform.position.x);
					int y = Mathf.RoundToInt (g.transform.FindChild ("HeadTarget").transform.position.y);
					int z = Mathf.RoundToInt (g.transform.FindChild ("HeadTarget").transform.position.z);
					
					if (xPositionCounts.Count == 0)
						xPositionCounts.Add (new PositionCount(x, 1));
					
					for (int i = 0; i < xPositionCounts.Count; i++)
					{
						if (xPositionCounts[i].coordinate == x)
							xPositionCounts[i].count++;
						else if (i == xPositionCounts.Count - 1)
							xPositionCounts.Add (new PositionCount(x, 1));
					}
					
					if (yPositionCounts.Count == 0)
						yPositionCounts.Add (new PositionCount(y, 1));
					
					for (int i = 0; i < yPositionCounts.Count; i++)
					{
						if (yPositionCounts[i].coordinate == y)
							yPositionCounts[i].count++;
						else if (i == yPositionCounts.Count - 1)
							yPositionCounts.Add (new PositionCount(y, 1));
					}
					
					if (zPositionCounts.Count == 0)
						zPositionCounts.Add (new PositionCount(z, 1));
					
					for (int i = 0; i < zPositionCounts.Count; i++)
					{
						if (zPositionCounts[i].coordinate == z)
							zPositionCounts[i].count++;
						else if (i == zPositionCounts.Count - 1)
							zPositionCounts.Add (new PositionCount(z, 1));
					}
				}
			}
			
			if (xPositionCounts.Count == 0)
			{
				yield return null;
				continue;
			}
			
			Vector3 centre = Vector3.zero;
			
			int count = 0;
			int index = 0;
			
			for (int i = 0; i < xPositionCounts.Count; i++)
			{
				if (xPositionCounts[i].count > count)
				{
					count = xPositionCounts[i].count;
					index = i;
				}
			}
			
			centre.x = xPositionCounts[index].coordinate;
			count = 0;
			index = 0;
			
			for (int i = 0; i < yPositionCounts.Count; i++)
			{
				if (yPositionCounts[i].count > count)
				{
					count = yPositionCounts[i].count;
					index = i;
				}
			}
			
			centre.y = yPositionCounts[index].coordinate;
			count = 0;
			index = 0;
			
			for (int i = 0; i < zPositionCounts.Count; i++)
			{
				if (zPositionCounts[i].count > count)
				{
					count = zPositionCounts[i].count;
					index = i;
				}
			}
			
			centre.z = zPositionCounts[index].coordinate;
			count = 0;
			index = 0;
			
			percentageCentre = centre;
			
			yield return new WaitForSeconds(1);
		}
	}
}

public class PositionCount
{
	public int coordinate = 0;
	public int count = 0;
	
	public PositionCount(int coordinate, int count)
	{
		this.coordinate = coordinate;
		this.count = count;
	}
}
