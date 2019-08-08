using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearScript : MonoBehaviour {

    public int teeth;
	public float speed;
    public GameObject[] LinkedGears;
	public bool ConnectedByShaft, firstgear;
	private float angle = 0f;
	private float ratio = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (firstgear == true)
        {
            GearCall(null);
        }
	}

    // Using a recursive call means all gears update on the same frame, instead of a delay between one gear updating, and the next 
    public void GearCall(GameObject previousgear)
    {
        if (previousgear != null)
        {
            speed = previousgear.transform.GetComponent<GearScript>().speed;
            if (!ConnectedByShaft)
            {
                ratio = -1.0f * ((float)previousgear.transform.GetComponent<GearScript>().teeth / (float)teeth);
                speed *= ratio;
            }
        }
        angle = speed * Time.deltaTime;
        gameObject.transform.localEulerAngles += new Vector3(0, angle, 0);
        for (int i = 0; i < LinkedGears.Length; i++)
        {
            LinkedGears[i].GetComponent<GearScript>().GearCall(gameObject);
        }
    }
}
