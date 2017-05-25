using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour {
	/*
	 * This script displays the current experience level,
	 * total experience needed to gain a level,
	 * and the amount of experience within the current level.
	 */

	XPLevels LevelSystem;
	PlayerLevel PlayerXP;
	Text XPCounter, LevelCounter;
	Slider XPBar;

	// Use this for initialization
	void Start () {
		LevelSystem = GameObject.Find("LevelManager").GetComponent<XPLevels>();
		PlayerXP = GameObject.Find("Player").GetComponent<PlayerLevel>();
		XPCounter = GameObject.Find("XPCounter").GetComponent<Text> () as Text;
		LevelCounter = GameObject.Find("LevelCounter").GetComponent<Text> () as Text;
		XPBar = GameObject.Find("XPBar").GetComponent<Slider> () as Slider;
	}
	
	// Update is called once per frame
	void Update () {
		LevelCounter.text = "Level " + PlayerXP.CurrentLevel.ToString();
		if(PlayerXP.CurrentLevel == 64)
		{
			XPCounter.text = LevelSystem.Levels[63].ToString() + "/" + LevelSystem.Levels[63].ToString();
			XPBar.maxValue = LevelSystem.Levels[63];
			XPBar.value = LevelSystem.Levels[63];
		}
		else
		{
			int temp = 0;
			for (int i = 0; i<PlayerXP.CurrentLevel; i++)
			{
				temp +=LevelSystem.Levels[i];
			}
			XPCounter.text = (PlayerXP.TotalExperience-temp).ToString() + "/" + LevelSystem.Levels[PlayerXP.CurrentLevel].ToString();
			XPBar.maxValue = LevelSystem.Levels[PlayerXP.CurrentLevel];
			XPBar.value = PlayerXP.TotalExperience-temp;
		}
			
	}
}
