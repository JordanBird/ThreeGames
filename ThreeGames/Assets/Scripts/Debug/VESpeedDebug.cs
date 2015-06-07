using UnityEngine;
using System.Collections;

public class VESpeedDebug : MonoBehaviour
{
    public float[] speeds = new float[3];

    void Start ()
    {
        if (speeds[0] == 0)
            speeds[0] = 1;

        for (int i = 1; i < speeds.Length; i++)
        {
            if (speeds[i] == 0)
                speeds[i] = speeds[i - 1] * 2f;
        }
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        if (Input.GetKey(KeyCode.LeftShift) &&  Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = speeds[0];

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = speeds[1];

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = speeds[2];
	}
}
