using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpacing : MonoBehaviour {

	public float MinDistanceDegrees, MaxDistanceDegrees, CircleRadius;
	public GameObject[] Objects;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float distance = MaxDistanceDegrees - MinDistanceDegrees;
		float gap = distance / (Objects.Length-1);
		for (int i = 0; i < Objects.Length; i++)
		{
			float temp = (gap * i) + MinDistanceDegrees;
			if (Objects[i] != null)
			{
				Objects[i].transform.localPosition = Vector3.Lerp(Objects[i].transform.localPosition, new Vector3(
					Mathf.Sin(temp * Mathf.Deg2Rad)*CircleRadius, 
					Mathf.Cos(temp * Mathf.Deg2Rad)*CircleRadius,
					0), 0.1f);
			}
		}
	}
}
