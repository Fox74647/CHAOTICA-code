using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantumState : MonoBehaviour
{
    public Pause PauseScript;
    public bool QuantumActive = false;
    public int State = 0;
    public GameObject[] PlayerModels;
    private PlayerMovement[] PlayerMovementScripts;
    public Camera[] PlayerCamera;
    public RectTransform ActiveStateGUI;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovementScripts = new PlayerMovement[4];
        for (int i = 0; i < 4; i++) //there's always a maximum of 4 player models
        {
            PlayerMovementScripts[i] = PlayerModels[i].GetComponent<PlayerMovement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseScript.pause)
        {
            if (Input.GetButtonDown("ShiftState"))
            {
                QuantumStateShift();
            }
            if (QuantumActive)
            {
                ActiveStateGUI.sizeDelta = new Vector2(Screen.width/2, Screen.height/2); //fits the camera sizes
                /*
                    during superposition, TL, TR, DL, DR are clones 0, 1, 2 and 3 respectively
                    screen.height/2 * (state/2) makes the bottom left screen act as clone 0
                    using -(state/2) inverts this, but places everything below the screen view
                    adding screen.height fixes this issue
                */
                ActiveStateGUI.position = new Vector3(Screen.width/2 * (State%2), Screen.height + (Screen.height/2 * -(State/2)), 0);
                if (Input.GetButtonDown("PrevState"))
                {
                    if (State == 0)
                    {
                        State = 3;
                    }
                    else
                    {
                        State--;
                    }
                    StateChange();
                }
                if (Input.GetButtonDown("NextState"))
                {
                    if (State == 3)
                    {
                        State = 0;
                    }
                    else
                    {
                        State++;
                    }
                    StateChange();
                }
            }
            else
            {
                ActiveStateGUI.sizeDelta = new Vector2(Screen.width, Screen.height); //fits the camera sizes
                ActiveStateGUI.position = new Vector3(0, Screen.height, 0);
            }
        }
    }

    private void QuantumStateShift()
    {
        if (!QuantumActive)
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerModels[i+1].transform.position = PlayerModels[0].transform.position;
                PlayerModels[i+1].transform.rotation = PlayerModels[0].transform.rotation;
                PlayerCamera[i+1].transform.rotation = PlayerCamera[0].transform.rotation;
                PlayerModels[i+1].SetActive(true);
            }
            
            PlayerCamera[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (PlayerMovementScripts[i].Holding != null)
                {
                    if (i == 0 && State == 0)
                    {
                        /*
                        While player 0 was the active state and holding an item, collapsing caused them to drop the item,
                        even though they shouldn't. This if clause checks to see if the active state and i are both 0, and
                        if so, breaks out of this if statement, fixing the issue.
                         */
                        break;
                    }
                    else if (i == State)
                    {
                        PlayerMovementScripts[0].ForceGrab(PlayerMovementScripts[i].Holding);
                    }
                    else
                    {
                        PlayerMovementScripts[i].ForceDrop();
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                PlayerModels[i+1].SetActive(false);
            }
            if (State != 0)
            {
                PlayerModels[0].transform.position = PlayerModels[State].transform.position;
                PlayerModels[0].transform.rotation = PlayerModels[State].transform.rotation;
                PlayerCamera[0].transform.rotation = PlayerCamera[State].transform.rotation;
            }
            PlayerCamera[0].rect = new Rect(0, 0, 1, 1);
            State = 0;
            StateChange();
        }
        QuantumActive = !QuantumActive;
    }

    private void StateChange()
    {
        for (int i = 0; i < 4; i++)
            {
                if (i != State)
                {
                    PlayerMovementScripts[i].IsActive = false;
                }
                else
                {
                    PlayerMovementScripts[i].IsActive = true;
                }
            }
    }

    public void Collapse(int ObservedState)
    {
        if (QuantumActive)
        {
            State = ObservedState;
            QuantumStateShift();
        }
    }
}
