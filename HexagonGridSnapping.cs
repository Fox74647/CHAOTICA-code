using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexagonGridSnapping : MonoBehaviour
{
	void Update ()
	{
		AxisAlignmentX();
	}
	
	private void AxisAlignmentX()
	{
		foreach (Transform child in transform)
		{
			float TargetX = Mathf.Round(child.transform.position.x * 2)/2;
			float TargetZ = 0;
			float Zoffset = Mathf.Sin(Mathf.Deg2Rad*60.0f);
			float DoubleZoffset = Zoffset*2;
			if (child.transform.position.x % 1.0f == 0)
			{

				TargetZ = (Mathf.Round(child.transform.position.z / DoubleZoffset))*DoubleZoffset;
			}
			else
			{
				TargetZ = (Mathf.Round((child.transform.position.z-Zoffset) / DoubleZoffset))*DoubleZoffset+Zoffset;
			}
			child.transform.position = new Vector3(TargetX, 0, TargetZ);
		}
	}
}
