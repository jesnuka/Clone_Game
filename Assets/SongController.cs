﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongController : MonoBehaviour
{
    public bool hasStarted;
    public bool stopped;

    public float currentVolume1;
    public float maxVolume1;

    public float volume1Divider;
   

    public float currentVolume2;
    public float volume2Divider;

    public GameObject songToStart;
    public GameObject songToStop;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null && collision.attachedRigidbody.GetComponent<PlayerController2D>())
        {
            PlayerController2D player = collision.attachedRigidbody.GetComponent<PlayerController2D>();

            if(!hasStarted)
            {
                hasStarted = true;
                songToStart.GetComponent<AudioSource>().Play();
                
            }

        }
    }
    private void Awake()
    {
        currentVolume1 = 0f;
        songToStart.GetComponent<AudioSource>().volume = currentVolume1;

       // currentVolume2 = songToStop.GetComponent<AudioSource>().volume;
    }
    // Update is called once per frame
    void Update()
    {
        if(hasStarted && !stopped)
        {
            currentVolume1 += Time.deltaTime/volume1Divider;
            songToStart.GetComponent<AudioSource>().volume = currentVolume1;
            currentVolume2 -= Time.deltaTime/volume2Divider;
            songToStop.GetComponent<AudioSource>().volume = currentVolume2;
        }
        if(currentVolume1 >= maxVolume1)
        {
            stopped = true;
            currentVolume2 = 0f;
            currentVolume1 = maxVolume1;

            songToStart.GetComponent<AudioSource>().volume = currentVolume1;
            songToStop.GetComponent<AudioSource>().volume = currentVolume2;
            songToStop.GetComponent<AudioSource>().Stop();
        }
    }
}
