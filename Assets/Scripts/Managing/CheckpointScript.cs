using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject player;
    bool isActive;
    public Camera mainCam;

    public int checkpointId; //For what this checkpoint sets back active when player spawns here.
    public GameObject[] activateableObstacles;
    public GameObject[] activateableTransitions;
    public EnemyController[] activateableEnemies; //For pipes, as they spawn only once

    public GameObject activateSong; //Switch music back
    public float activateVolume;
    public GameObject disableSong; //Turn off


    private void Awake()
    {
        player = GameObject.Find("Player");
        mainCam = Camera.main;
    }

    public void ResetObst()
    {
        foreach(GameObject obstacle in activateableObstacles)
        {
            obstacle.SetActive(false);
        }
    }
    public void ResetPipes()
    {
        foreach(EnemyController enemy in activateableEnemies)
        {
            enemy.hotDogSpawnAmount = 1;
        }
    }

    public void ResetSongs()
    {
        activateSong.GetComponent<AudioSource>().volume = activateVolume;
        activateSong.GetComponent<AudioSource>().Play();
        disableSong.GetComponent<AudioSource>().Stop();

    }

    public void ResetTransitions()
    {
        foreach (GameObject transition in activateableTransitions)
        {
            transition.SetActive(true);
            transition.GetComponent<TransitionPoint>().ResetValue();
        }
    }

    public void ResetCamera()
    {
       mainCam.gameObject.GetComponent<CameraManager>().ResetCamera(checkpointId);

    }

    public void ResetThings()
    {
        
        if(checkpointId == 0)
        {
            ResetObst();
            ResetPipes();
            ResetTransitions();
            ResetCamera();
            ResetSongs();
        }
        else if (checkpointId == 1)
        {
            ResetObst();
            ResetTransitions();
            ResetCamera();
            ResetSongs();
        }
        else if (checkpointId == 2)
        {
            ResetObst();
            ResetTransitions();
            ResetCamera();
            ResetSongs();
        }
    }

    public void ActivateCheckpoint()
    {
        player.gameObject.GetComponent<PlayerController2D>().currentCheckpoint.GetComponent<CheckpointScript>().DisableCheckpoint();
        player.gameObject.GetComponent<PlayerController2D>().currentCheckpoint = this.gameObject;
        isActive = true;
    }

    public void DisableCheckpoint()
    {
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
          if (other.attachedRigidbody != null && other.gameObject.GetComponent<PlayerController2D>())
          {
             //PlayerController2D player = other.gameObject.GetComponent<PlayerController2D>();
              ActivateCheckpoint();
          }
    }
}
