using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPLevels : MonoBehaviour {

	/*
	 * This script calculates the 64 experience levels in the game.
	 */

	private int Lvls = 64, MinLvlXP = 1024, MaxLvlXP = 1048576;
	public int[] Levels;

	void Start ()
	{
		Levels = new int[Lvls];
		double B = Mathf.Log((float)MaxLvlXP / MinLvlXP) / (Lvls - 1);
		double A = (float)MinLvlXP / (Mathf.Exp((float)B) - 1.0);
		for (int i = 1; i <= Lvls; i++)
		{
			int old_xp = (int)Mathf.Round((float)A * Mathf.Exp((float)B * (i - 1)));
			int new_xp = (int)Mathf.Round((float)A * Mathf.Exp((float)B * i));
			Levels[i-1] = (new_xp - old_xp);
		}
		Levels[63] = MaxLvlXP;
	}
}
