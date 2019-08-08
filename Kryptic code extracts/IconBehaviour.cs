using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconBehaviour : MonoBehaviour{

	private Button IconButton;
	public GameObject ShortcutItem;


	// Use this for initialization
	void Start ()
	{
		IconButton = transform.GetComponent<Button>();
		IconButton.onClick.AddListener(IconButtonClick);
	}
	
	// Update is called once per frame
	private void IconButtonClick()
	{
		ShortcutItem.SetActive(true);
		ShortcutItem.transform.SetAsLastSibling();
	}
}
