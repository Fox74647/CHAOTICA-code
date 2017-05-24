using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour {

	XPLevels LevelSystem;
	PlayerLevel PlayerXP;
	Text XPCounter;

	// Use this for initialization
	void Start () {
		LevelSystem = GameObject.Find("LevelManager").GetComponent<XPLevels>();
		PlayerXP = GameObject.Find("Player").GetComponent<PlayerLevel>();
		XPCounter = GetComponent<Text> () as Text;
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerXP.CurrentLevel == 64)
		{
			XPCounter.text = LevelSystem.Levels[63].ToString() + "/" + LevelSystem.Levels[63].ToString();
		}
		else
		{
			int temp = 0;
			for (int i = 0; i<PlayerXP.CurrentLevel; i++)
			{
				temp +=LevelSystem.Levels[i];
			}
			XPCounter.text = (PlayerXP.TotalExperience-temp).ToString() + "/" + LevelSystem.Levels[PlayerXP.CurrentLevel].ToString();
		}
			
	}
}
