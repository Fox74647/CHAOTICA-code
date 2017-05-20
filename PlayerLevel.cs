using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour {

	public int CurrentLevel, TotalExperience, MaxExperience;
	XPLevels LevelSystem;


	// Use this for initialization
	void Start () {
		LevelSystem = GameObject.Find("LevelManager").GetComponent<XPLevels>();
	}
	
	// Update is called once per frame
	void Update () {
		int temp = LevelSystem.Levels[0];
		if (MaxExperience == 0)
		{
			for (int i = 0; i < 64; i++)
			{
				MaxExperience += LevelSystem.Levels[i];
			}
		}
		if (TotalExperience >= MaxExperience)
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
