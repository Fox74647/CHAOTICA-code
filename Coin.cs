using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	private MoneyManager Money;
	private GameObject Player;
	public int CoinValue;

	void Start ()
	{
		Money = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();	
		Player = GameObject.Find("Player");
	}

	void Update ()
	{
		gameObject.transform.Rotate(0, 1, 0);

		if (Vector3.Distance(Player.transform.position, gameObject.transform.position) <= 2)
		{
			Money.MoneyDifference += CoinValue;
			Destroy(gameObject);
		}
	}

}
