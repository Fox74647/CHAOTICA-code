using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode4 : MonoBehaviour
{
    private float progress = 0;
    public MusicHandler Speed;

    void Awake()
    {
        Speed = GameObject.Find("Main Camera").transform.GetComponent<MusicHandler>();
    }


    void OnEnable()
    {
        ModeAnimation();
    }

    void Update()
    {
        ModeAnimation();
    }

    private void ModeAnimation()
    {
        progress = (((Time.time-Speed.TimeUpdated)/Speed.SecondsPerBeat)%2);
        if (progress >= 2)
        {
            progress -= 2;
        }
        this.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(progress/2, 1, 1);
    }
}
