using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{
    public int screen = 0, item = 0;
    bool axisXused = false, axisYused = false;
    public Vector3[] CameraPositions;
    public Quaternion[] CameraRotations;

    public AudioClip Highlight, Select, Denied;
    private AudioSource MenuAudioSource;

    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        MenuAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();

        if (Input.GetButtonDown("Action"))
        {
            switch (screen)
            {
                case 0: //start screen
                    {
                        MenuAudioSource.clip = Select;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 0;
                        break;
                    }
                case 1: //main menu
                    {
                        switch (item)
                        {
                            case 0: //start
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 2;
                                    item = 0;
                                    break;
                                }
                            case 1: //options
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 3;
                                    item = 0;
                                    break;
                                }
                            case 2: //credits
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 4;
                                    item = 0;
                                    break;
                                }
                            case 3: //quit
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 5;
                                    item = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case 2: //start game
                    {
                        switch (item)
                        {
                            case 0: //knight
                                {
                                    
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    SceneManager.LoadScene("0_knight");
                                    break;
                                }
                            case 1: //duke
                                {
                                    if (CubicCrushData.Data.UnlockedLevels[0] == true)
                                    {
                                        MenuAudioSource.clip = Select;
                                        MenuAudioSource.Play();
                                        SceneManager.LoadScene("1_duke");
                                        break;
                                    }
                                    else
                                    {
                                        MenuAudioSource.clip = Denied;
                                        MenuAudioSource.Play();
                                        break;
                                    }
                                }
                            case 2: //prince
                                {
                                    if (CubicCrushData.Data.UnlockedLevels[1] == true)
                                    {
                                        MenuAudioSource.clip = Select;
                                        MenuAudioSource.Play();
                                        SceneManager.LoadScene("2_prince");
                                        break;
                                    }
                                    else
                                    {
                                        MenuAudioSource.clip = Denied;
                                        MenuAudioSource.Play();
                                        break;
                                    }
                                }
                            case 3: //king
                                {
                                    if (CubicCrushData.Data.UnlockedLevels[2] == true)
                                    {
                                        MenuAudioSource.clip = Select;
                                        MenuAudioSource.Play();
                                        SceneManager.LoadScene("3_king");
                                        break;
                                    }
                                    else
                                    {
                                        MenuAudioSource.clip = Denied;
                                        MenuAudioSource.Play();
                                        break;
                                    }
                                }
                            case 4: //emperor
                                {
                                    if (CubicCrushData.Data.UnlockedLevels[3] == true)
                                    {
                                        MenuAudioSource.clip = Select;
                                        MenuAudioSource.Play();
                                        SceneManager.LoadScene("4_emperor");
                                        break;
                                    }
                                    else
                                    {
                                        MenuAudioSource.clip = Denied;
                                        MenuAudioSource.Play();
                                        break;
                                    }
                                }
                            case 5: //back
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 1;
                                    item = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case 3: //options
                    {
                        switch (item)
                        {
                            case 0: //music volume
                                {
                                    //do nothing
                                    break;
                                }
                            case 1: //sfx volume
                                {
                                    //do nothing
                                    break;
                                }
                            case 2: //reset scores
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 6;
                                    item = 0;
                                    break;
                                }
                            case 3: //invertY
                                {
                                    CubicCrushData.Data.Yinvert = !CubicCrushData.Data.Yinvert;
                                    CubicCrushData.Data.Save();
                                    break;
                                }
                            case 4: //back
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 1;
                                    item = 1;
                                    break;
                                }
                        }
                        break;
                    }
                case 4: //credits
                    {
                        MenuAudioSource.clip = Select;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 2;
                        break;
                    }
                case 5: //quit game
                    {
                        switch (item)
                        {
                            case 0: //no
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 1;
                                    item = 3;
                                    break;
                                }
                            case 1: //yes
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    CubicCrushData.Data.Save();
                                    Application.Quit();
                                    break;
                                }
                        }
                        break;
                    }
                case 6: //reset scores
                    {
                        switch (item)
                        {
                            case 0: //no
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    screen = 3;
                                    item = 2;
                                    break;
                                }
                            case 1: //yes
                                {
                                    MenuAudioSource.clip = Select;
                                    MenuAudioSource.Play();
                                    CubicCrushData.Data.DataReset();
                                    screen = 3;
                                    item = 2;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            switch (screen)
            {
                case 0: //start screen
                    {
                        //do nothing
                        break;
                    }
                case 1: //main menu
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 0;
                        item = 0;
                        break;
                    }
                case 2: //start game
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 0;
                        break;
                    }
                case 3: //options
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 1;
                        break;
                    }
                case 4: //credits
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 2;
                        break;
                    }
                case 5: //quit game
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 1;
                        item = 3;
                        break;
                    }
                case 6: //reset scores
                    {
                        MenuAudioSource.clip = Denied;
                        MenuAudioSource.Play();
                        screen = 3;
                        item = 2;
                        break;
                    }
            }
        }

        if (Input.GetAxis("Vertical") > 0 && axisXused == false)
        {
            axisXused = true;
            switch (screen)
            {
                case 0: //start screen
                    {
                        //do nothing
                        break;
                    }
                case 1: //main menu
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 0)
                        {
                            item = 3;
                        }
                        else
                        {
                            item--;
                        }
                        break;
                    }
                case 2: //start game
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 0)
                        {
                            item = 5;
                        }
                        else
                        {
                            item--;
                        }
                        break;
                    }
                case 3: //options
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 0)
                        {
                            item = 3;
                        }
                        else
                        {
                            item--;
                        }
                        break;
                    }
                case 4: //credits
                    {
                        //do nothing
                        break;
                    }
                case 5: //quit game
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 0)
                        {
                            item = 1;
                        }
                        else
                        {
                            item--;
                        }
                        break;
                    }
                case 6: //reset scores
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 0)
                        {
                            item = 1;
                        }
                        else
                        {
                            item--;
                        }
                        break;
                    }
            }
        }
        else if (Input.GetAxis("Vertical") < 0 && axisXused == false)
        {
            axisXused = true;
            switch (screen)
            {
                case 0: //start screen
                    {
                        //do nothing
                        break;
                    }
                case 1: //main menu
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 3)
                        {
                            item = 0;
                        }
                        else
                        {
                            item++;
                        }
                        break;
                    }
                case 2: //start game
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 5)
                        {
                            item = 0;
                        }
                        else
                        {
                            item++;
                        }
                        break;
                    }
                case 3: //options
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 4)
                        {
                            item = 0;
                        }
                        else
                        {
                            item++;
                        }
                        break;
                    }
                case 4: //credits
                    {
                        //do nothing
                        break;
                    }
                case 5: //quit game
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 1)
                        {
                            item = 0;
                        }
                        else
                        {
                            item++;
                        }
                        break;
                    }
                case 6: //reset scores
                    {
                        MenuAudioSource.clip = Highlight;
                        MenuAudioSource.Play();
                        if (item == 1)
                        {
                            item = 0;
                        }
                        else
                        {
                            item++;
                        }
                        break;
                    }
            }
        }
        else if (Input.GetAxis("Vertical") == 0 && axisXused == true)
        {
            axisXused = false;
        }

        if (Input.GetAxis("Horizontal") > 0 && axisYused == false)
        {
            axisYused = true;
            if (screen == 3 && item == 0)
            {
                if (CubicCrushData.Data.MusicVol < 10)
                {
                    MenuAudioSource.clip = Highlight;
                    MenuAudioSource.Play();
                    CubicCrushData.Data.MusicVol++;
                }
            }
            else if (screen == 3 && item == 1)
            {
                if (CubicCrushData.Data.SFXVol < 10)
                {
                    MenuAudioSource.clip = Highlight;
                    MenuAudioSource.Play();
                    CubicCrushData.Data.SFXVol++;
                }
            }
        }
        else if(Input.GetAxis("Horizontal") < 0 && axisYused == false)
        {
            axisYused = true;
            if (screen == 3 && item == 0)
            {
                if (CubicCrushData.Data.MusicVol > 0)
                {
                    MenuAudioSource.clip = Highlight;
                    MenuAudioSource.Play();
                    CubicCrushData.Data.MusicVol--;
                }
            }
            else if (screen == 3 && item == 1)
            {
                if (CubicCrushData.Data.SFXVol > 0)
                {
                    MenuAudioSource.clip = Highlight;
                    MenuAudioSource.Play();
                    CubicCrushData.Data.SFXVol--;
                }
            }
        }
        else if (Input.GetAxis("Horizontal") == 0 && axisYused == true)
        {
            axisYused = false;
        }
    }

    private void CameraMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, CameraPositions[screen], ref velocity, 0.3f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, CameraRotations[screen], 3f);
    }
}
