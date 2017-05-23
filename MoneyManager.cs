using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

	/*
	 * This script manages the overall money the player has.
	 * coin pickups should send a message to add to MoneyDifference.
	 * shop items should check against money to see if they can be bought when selected.
	 * if they can, they should send a message to subtract from MoneyDifference.
	 * 
	 * THIS SCRIPT IS DESIGNED TO WORK WITH 7 IMAGES AND 16 SPRITES
	 */

	public int Money, MoneyDifference;
	public Image[] CoinCounterSegment;
	public Sprite[] CoinCounterUI;
    private AudioSource CoinCounterBeep;

    private void Start()
    {
        CoinCounterBeep = gameObject.GetComponent<AudioSource>();
    }

    void Update ()
	{
		MoneyChanger();

		//limits the money to between 0 and the maximum value
		if (Money < 0)
		{
			Money = 0;
		}
		else if (Money > 268435455)
		{
			Money = 268435455;
		}

		//updates each segment
		for (int i = 0; i < 7; i++)
		{
			SegmentUpdate(i);
		}
	}

	void SegmentUpdate(int segment)
	{
		/*
		 * the money system works in a hexadecimal format.
		 * this function converts the decimal number to a hexadecimal display.
		 */

		int TempMultiplier = 1;
		for (int i = 0; i < segment; i++)
		{
			TempMultiplier *=16;
		}
		CoinCounterSegment[segment].sprite = CoinCounterUI[(Money/TempMultiplier)%16];
	}

	public void MoneyChanger()
	{
		/*
		 * instead of instantly changing the money amount to the required value,
		 * this system animates the counter. 
		 */
		int TempIncreaser = MoneyDifference/5;
		if(TempIncreaser == 0)
		{
			if (MoneyDifference < 0)
			{
				CoinCounterBeep.Play();
				Money--;
				MoneyDifference++;
			}
			else if (MoneyDifference > 0)
			{
				CoinCounterBeep.Play();
				Money++;
				MoneyDifference--;
			}
		}
		if (MoneyDifference != 0)
		{
            CoinCounterBeep.Play();
            Money += TempIncreaser;
			MoneyDifference -= TempIncreaser;
        }
	}
}
