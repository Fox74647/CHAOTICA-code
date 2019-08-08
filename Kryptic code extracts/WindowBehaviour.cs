using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WindowBehaviour : MonoBehaviour
{
	public RectTransform m_transform = null;
	private Button MinimiseButton, ExitButton;
	public RectTransform Panelsize;
	private Vector2 MinSize, MaxSize;
	private bool IsMinimised = false;
	private float InterpolationRate = 0.3f;
	private Color DefaultColor;
	public AudioClip[] Solution, Minimise;

	void OnEnable()
	{
		transform.SetAsLastSibling();
		PlayMinimiseSound(1);
	}

	// Use this for initialization
	void Start ()
	{
		m_transform = GetComponent<RectTransform>();
		Panelsize = gameObject.GetComponent<RectTransform>() as RectTransform;
		MaxSize = Panelsize.sizeDelta;
		MinSize = new Vector2(Panelsize.sizeDelta.x, 35); //top of window is 27, bottom of window is 8
		ExitButton = gameObject.transform.Find("ExitButton").GetComponent<Button>();
		MinimiseButton = gameObject.transform.Find("MinimiseButton").GetComponent<Button>();
		ExitButton.onClick.AddListener(ExitButtonClick);
		MinimiseButton.onClick.AddListener(MinimiseButtonClick);
		DefaultColor = gameObject.GetComponent<Image>().color;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (IsMinimised)
		{
			Panelsize.sizeDelta = Vector2.Lerp(Panelsize.sizeDelta, MinSize, InterpolationRate);
		}
		else
		{
			Panelsize.sizeDelta = Vector2.Lerp(Panelsize.sizeDelta, MaxSize, InterpolationRate);
		}
		if(gameObject.GetComponent<Image>().color != DefaultColor)
		{
			gameObject.GetComponent<Image>().color = Color.Lerp(gameObject.GetComponent<Image>().color, DefaultColor, 0.01f);
		}
	}
	
	public void PlaySolutionSound(int AudioClipID)
	{
		gameObject.GetComponent<AudioSource>().clip = Solution[AudioClipID];
		gameObject.GetComponent<AudioSource>().Play();
	}
	
	public void PlayMinimiseSound(int AudioClipID)
	{
		gameObject.GetComponent<AudioSource>().clip = Minimise[AudioClipID];
		gameObject.GetComponent<AudioSource>().Play();
	}

	private void MinimiseButtonClick()
	{
		IsMinimised = !IsMinimised;
		if (IsMinimised)
		{
			PlayMinimiseSound(0);
		}
		else
		{
			PlayMinimiseSound(1);
		}
	}

	private void ExitButtonClick()
	{
		gameObject.SetActive(false);
	}

	void OnDisable()
	{
		Panelsize.sizeDelta = MaxSize;
		IsMinimised = false;
	}
}
