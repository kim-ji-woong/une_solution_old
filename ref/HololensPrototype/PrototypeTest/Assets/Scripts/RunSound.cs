using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSound : MonoBehaviour {

    // Use this for initialization


    AudioSource audio;


    void Start ()
    {
        audio = GetComponent<AudioSource>();
        audio.spatialize = true;
        audio.volume = 0.3f;
        audio.spatialBlend = 1.0f;
        audio.dopplerLevel = 0.0f;
        audio.rolloffMode = AudioRolloffMode.Logarithmic;
        audio.maxDistance = 20f;
    }


    public void StartPlay()
    {        
        audio.Play();
    }

    public void StopPlay()
    {
        audio.Stop();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
