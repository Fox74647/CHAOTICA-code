using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearStepper : MonoBehaviour {

    public float MaxSpd, MinSpd, MaxPeriod, MinPeriod;
    public bool startMax;
    private GearScript gear;
    private float time;

	// Use this for initialization
	void Start () {
        gear = gameObject.GetComponent<GearScript>();
        if (startMax)
        {
            gear.speed = MaxSpd;
        }
        else
        {
            gear.speed = MinSpd;
        }
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        if (startMax && time >= MaxPeriod)
        {
            time -= MaxPeriod;
            gear.speed = MinSpd;
            startMax = !startMax;
        }
        else if (!startMax && time >= MinPeriod)
        {
            time -= MinPeriod;
            gear.speed = MaxSpd;
            startMax = !startMax;
        }
    }
}
