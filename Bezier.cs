using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour {

    public GameObject[] PointArray; //for the control points of the bezier
    public GameObject PathPrefab; //prefab that marks the curve
    public int RenderPoints; //number of points along the bezier curve

	// Use this for initialization
	void Start () {
		//duplicate the original array for manipulation purposes
        Vector3[] DuplicateArray = new Vector3[PointArray.Length];
        for (int i = 0; i < PointArray.Length; i++)
        {
            DuplicateArray[i] = PointArray[i].transform.position;
        }
		
		//instantiate prefab at the first and last points of the bezier curve
        Instantiate(PathPrefab, PointArray[0].transform.position, Quaternion.identity); 
        Instantiate(PathPrefab, PointArray[PointArray.Length-1].transform.position, Quaternion.identity);
		 
		if (RenderPoints != 0)
        {
            float spacing = 1.0f / (RenderPoints + 1); //distance between the prefab instances
           
            for (int i = 0; i < RenderPoints; i++)
            {
                DuplicateArray = new Vector3[PointArray.Length];
                for (int j = 0; j < PointArray.Length; j++)
                {
                    DuplicateArray[j] = PointArray[j].transform.position;
                }

                while (DuplicateArray.Length > 1)
                {
					/*
					This while loop computes the bezier curve at a given point, using the lerp function,
					and the spacing value computed further up.
					
					Here's an example of how it works
					4 points: p0, p1, p2, p3
					and a spacing value: 0.5, which is essentially the midpoint.
					
					first, the lerp function returns the midpoint between all pairs of consecutive points
					p0 and p1, which gives us a point q0
					p1 and p2, which gives q1
					p2 and p3, which gives q2
					
					we now have 3 new points to work with. rather than make a brand new array, the lerp
					overwrites the first value it was given, and when all lerps are done in one itteration,
					the while loop simply removes the final element in the array.
					
					the function then repeats, q0 q1 q2 becomes r0 r1, which becomes s
					s is the point that sits on the path of the bezier curve, and sits alone in the array.
					the while loop breaks out, because the array is no longer greater than 1, and we proceed
					to instantiate a prefab at the given point.
					*/
					
                    for (int k = 0; k < DuplicateArray.Length - 1; k++)
                    {
                        DuplicateArray[k] = Vector3.Lerp(DuplicateArray[k], DuplicateArray[k + 1], spacing*(i+1));
                    }
                    System.Collections.Generic.List<Vector3> list = new System.Collections.Generic.List<Vector3>(DuplicateArray);
                    list.Remove(DuplicateArray[DuplicateArray.Length - 1]);
                    DuplicateArray = list.ToArray();
                }
                Instantiate(PathPrefab, DuplicateArray[0], Quaternion.identity);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
