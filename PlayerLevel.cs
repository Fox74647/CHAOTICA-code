using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour {

	/*
	 * This script keeps track of the player's experience level and total experience 
	 */

	public int CurrentLevel, TotalExperience, XPDifference;
	private XPLevels LevelSystem;
	private int MaxExperience;
    private AudioSource XPCounterBeep;

    void Start ()
	{
		LevelSystem = GameObject.Find("LevelManager").GetComponent<XPLevels>();
        XPCounterBeep = gameObject.GetComponent<AudioSource>();
    }

	void Update () {

        XPChanger();

        //calculate the maximum experience possible
        int temp = LevelSystem.Levels[0];
        if (MaxExperience == 0)
        {
            for (int i = 0; i < 64; i++)
            {
                MaxExperience += LevelSystem.Levels[i]; 
            }
        }
        //limits the XP to between 0 and the maximum value
        if (TotalExperience < 0) 
        {
            TotalExperience = 0;
            CurrentLevel = 0;
            XPDifference = 0;
        }
        else if (TotalExperience > MaxExperience)
		{
			TotalExperience = MaxExperience;
            XPDifference = 0;
		}

        //sets the correct experience level
		if (TotalExperience == MaxExperience)
		{
			CurrentLevel = 64;
		}
		else
		{
			for (int i = 0; i < 64; i++)
			{
				if (TotalExperience >= temp)
				{
					temp += LevelSystem.Levels[i+1];
				}
				else
				{
					CurrentLevel = i;
					break;
				}
			}
		}
	}
    void XPChanger()
    {
        /*
		 * instead of instantly changing the money amount to the required value,
		 * this system animates the counter. 
		 */
        int TempIncreaser = XPDifference / 10;
        if (TempIncreaser == 0)
        {
            if (XPDifference < 0)
            {
                XPCounterBeep.Play();
                TotalExperience--;
                XPDifference++;
            }
            else if (XPDifference > 0)
            {
                XPCounterBeep.Play();
                TotalExperience++;
                XPDifference--;
            }
        }
        if (XPDifference != 0)
        {
            XPCounterBeep.Play();
            TotalExperience += TempIncreaser;
            XPDifference -= TempIncreaser;
        }
    }
}
