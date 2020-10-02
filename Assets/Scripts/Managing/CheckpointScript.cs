using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject player;
    bool isActive;

    private void Awake()
    {
        player = GameObject.Find("Player");
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
