using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleIconBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	private Button PuzzleIconButton;
	private GameObject LockIcon;
	public string PuzzleName;
	public GameObject ShortcutItem;
	public bool UnlockedFromStart;
	public bool Locked = true;
	public int PreviousPuzzle, PreviousPuzzleSolutionID;
	private int PuzzleOffset = 0;
	public GameObject Tooltip;
	public Vector2 TooltipPosition;

	// Use this for initialization
	void Start ()
	{		
		PuzzleIconButton = transform.GetComponent<Button>();
		PuzzleIconButton.onClick.AddListener(PuzzleIconButtonClick);
		if (!UnlockedFromStart)
		{
			LockIcon = gameObject.transform.GetChild(0).gameObject;
			for (int i = 0; i < PreviousPuzzle; i++)
			{
				PuzzleOffset += KrypticSaveData.Data.PuzzleSolutions[i];
			}
			if (KrypticSaveData.Data.PuzzlesSolved[PuzzleOffset + PreviousPuzzleSolutionID]  == true)
			{
				LockIcon.SetActive(false);
				Locked = false;
			}
		}
		else
		{
			Locked = false;
		}
	}

	void Update () {
		if (!UnlockedFromStart)
		{
			if (KrypticSaveData.Data.PuzzlesSolved[PuzzleOffset + PreviousPuzzleSolutionID]  == true)
			{
				LockIcon.SetActive(false);
				Locked = false;
			}
			else
			{
				LockIcon.SetActive(true);
				Locked = true;
			}
		}

	}

	// Update is called once per frame
	private void PuzzleIconButtonClick()
	{
		if (!Locked)
		{
			ShortcutItem.SetActive(true);
			ShortcutItem.transform.SetAsLastSibling();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Tooltip != null)
		{
			Tooltip.SetActive(true);
			if (Locked)
			{
				Tooltip.transform.GetChild(0).GetComponent<Text>().text = "Locked";
			}
			else
			{
				Tooltip.transform.GetChild(0).GetComponent<Text>().text = PuzzleName;
			}
			Tooltip.GetComponent<RectTransform>().pivot = TooltipPosition;
			Tooltip.GetComponent<RectTransform>().position = gameObject.transform.position;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (Tooltip != null)
		{
			Tooltip.SetActive(false);
		}
	}
	
	public bool GetLockStatus()
	{
		return Locked;
	}
}