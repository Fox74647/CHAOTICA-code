using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearScript : MonoBehaviour {

	public int teeth;
	public float speed;
	public GameObject PrevGear;
	public bool ConnectedByShaft;
	private float angle = 0f;
	public float ratio = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (PrevGear != null)
		{
			speed = PrevGear.transform.GetComponent<GearScript>().speed;
			if (!ConnectedByShaft)
			{
				ratio = -1.0f * ((float)PrevGear.transform.GetComponent<GearScript>().teeth/(float)teeth);
				speed *= ratio;
			}
		}
		angle += speed * Time.deltaTime;
		gameObject.transform.eulerAngles = new Vector3 (transform.rotation.x, angle, transform.rotation.z);
	}
}
