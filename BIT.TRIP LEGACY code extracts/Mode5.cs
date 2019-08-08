using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode5 : MonoBehaviour
{
    public Sprite [] Mode5Sprites;
    public int AnimationDistance;
    private int CurrentSprite = 0;
    public float progress = 0;
    private Vector3 StartLocation;

    public MusicHandler Speed;

    // Start is called before the first frame update
    void Awake()
    {
        StartLocation = transform.GetChild(4).GetComponent<SpriteRenderer>().transform.position;
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
        progress = (((Time.time-Speed.TimeUpdated)/Speed.SecondsPerBeat)%5f);
        for (int i = 0; i < 5; i++)
        {
            if (progress > i && progress <= i+1)
            {
                CurrentSprite = i;
                break;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild((i+CurrentSprite)%5).GetComponent<SpriteRenderer>().transform.position = new Vector3(
                StartLocation.x + Mathf.Abs(Mathf.Sin(progress * 180 * Mathf.Deg2Rad) * (4 - (i+CurrentSprite)%5) * AnimationDistance),
                StartLocation.y + Mathf.Abs(Mathf.Sin(progress * 180 * Mathf.Deg2Rad) * (4 - (i+CurrentSprite)%5) * AnimationDistance),
                0);
            transform.GetChild((i+CurrentSprite)%5).GetComponent<SpriteRenderer>().sprite = Mode5Sprites[i];
        }
    }
}