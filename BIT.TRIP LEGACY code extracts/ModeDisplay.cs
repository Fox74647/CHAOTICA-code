using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeDisplay : MonoBehaviour
{
    public GameObject[] Mode;

    void Awake()
	{
        Mode = new GameObject[8];
        for (int i = 0; i < Mode.Length; i++)
        {
            Mode[i] = transform.GetChild(i).gameObject;
            Mode[i].SetActive(false);
        }
    }

    public void UpdateModeDisplay(CharacterDetails.Character Character)
    {
        /*
        Party.CharacterList[Party.PartyDetails[ActiveChar]] grabs the current fighting character
        
        The alpha mask of the mode bars ranges from 128 alpha to 255 alpha
        The mode bars are thus represented as a float value between 0.5 and 1 inclusive
        The mode up bar takes 1 - 0.5f*(CurrentModeUp/(BaseModeUp*(Mode+1)))
        The mode dn bar takes 0.5f + 0.5f*(CurrentHealth/((BaseHealth/8)*(8-Mode)))
        */
        
        for (int i = 0; i < 8; i++)
        {
            if ((int)Character.Mode == i)
            {
                Mode[i].SetActive(true);
                foreach (Transform child in Mode[i].transform.GetComponentsInChildren<Transform>())
                {
                    if (child.tag == "ModeUpBar")
                    {
                        child.GetComponent<SpriteMask>().alphaCutoff = 1 - 0.5f *
                        (Character.CurrentModeUp/
                        (Character.BaseModeUp*
                        ((int)Character.Mode+1f)));
                    }
                    else if (child.tag == "ModeDnBar")
                    {
                        child.GetComponent<SpriteMask>().alphaCutoff = 0.5f + 0.5f * 
                        (Character.CurrentHealth/
                        ((Character.BaseHealth/8f)*
                        (8f-(int)Character.Mode)));
                    }
                }
            }
            else
            {
                Mode[i].SetActive(false);
            }
        }
    }

    public void NullModeDisplay()
    {
        for (int i = 0; i < Mode.Length; i++)
        {
            Mode[i].SetActive(false);
        }
    }
}
