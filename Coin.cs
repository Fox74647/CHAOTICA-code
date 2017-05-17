using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	private CoinManager Money;
	private GameObject Player;
	public int CoinValue;

	// Use this for initialization
	void Start () {
		Money = GameObject.Find("CoinManager").GetComponent<CoinManager>();	
		Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate(0, 1, 0);

		if (Vector3.Distance(Player.transform.position, gameObject.transform.position) <= 2)
		{
			Money.MoneyDifference += CoinValue;
			Destroy(gameObject);
		}
	}

}
