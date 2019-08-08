using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragBehaviour : MonoBehaviour, IDragHandler
{

	public RectTransform Parent;

	// Use this for initialization
	void Start () {
		Parent = transform.parent.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnDrag(PointerEventData eventData)
	{
		Parent.position += new Vector3(eventData.delta.x, eventData.delta.y);
		Parent.transform.SetAsLastSibling();
		Parent.position = new Vector3(
			Mathf.Clamp(Parent.position.x, Parent.sizeDelta.x/2, Screen.width-Parent.sizeDelta.x/2), 
			Mathf.Clamp(Parent.position.y, Parent.sizeDelta.y, Screen.height));
	}
}
