using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public float BeatsPerMinute;
    private float BeatsPerMinuteOld;
    public float SecondsPerBeat;
    public float TimeUpdated;

    [System.Serializable]
    public struct LevelTrack{
		public AudioClip AudioClip;
		public float BPM;
	}
    private AudioSource MusicPlayer;
    public bool Play;
    public LevelTrack[] LevelAudio;

    void Awake()
    {
        BeatsPerMinuteOld = BeatsPerMinute;
        TimeUpdated = Time.time;
        SecondsPerBeat = (60f/BeatsPerMinute)*2;
        MusicPlayer = transform.GetComponent<AudioSource>();
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!Play)
        {
            MusicPlayer.Stop();
        }
        else
        {
            if (!MusicPlayer.isPlaying && MusicPlayer.time >= MusicPlayer.clip.length)
            {
                MusicPlayer.time = MusicPlayer.clip.length/2f;
                MusicPlayer.Play();
            }
        }

        if (BeatsPerMinuteOld != BeatsPerMinute)
        {
           Resync(); 
        }

        if (Input.GetKeyDown("1"))
        {
            SwapTrack(0);
            Resync();
        }
        else if (Input.GetKeyDown("2"))
        {
            SwapTrack(1);
            Resync();
        }
        else if (Input.GetKeyDown("3"))
        {
            SwapTrack(2);
            Resync();
        }
        else if (Input.GetKeyDown("4"))
        {
            SwapTrack(3);
            Resync();
        }
    }

    public void SwapTrack(int TrackID)
    {
        MusicPlayer.clip = LevelAudio[TrackID].AudioClip;
        BeatsPerMinute = LevelAudio[TrackID].BPM;
        MusicPlayer.time = 0;
        MusicPlayer.Play();
        Play = true;
    }

    public void StopAudio()
    {
        Play = false;
    }
    
    public void Resync()
    {
        BeatsPerMinuteOld = BeatsPerMinute;
        SecondsPerBeat = (60f/BeatsPerMinute)*2;
        TimeUpdated = Time.time;
    }
}
