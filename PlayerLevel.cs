using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour {

	/*
	 * This script keeps track of the player's experience level and total experience 
	 */

	public int CurrentLevel, TotalExperience;
	XPLevels LevelSystem;
	private int MaxExperience;

	void Start ()
	{
		LevelSystem = GameObject.Find("LevelManager").GetComponent<XPLevels>();
	}

	void Update () {
		int temp = LevelSystem.Levels[0];

		if (MaxExperience == 0)
		{
			for (int i = 0; i < 64; i++)
			{
				MaxExperience += LevelSystem.Levels[i]; //calculate the maximum experience possible
			}
		}

		if (TotalExperience > MaxExperience)
		{
			TotalExperience = MaxExperience;
		}

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
}
