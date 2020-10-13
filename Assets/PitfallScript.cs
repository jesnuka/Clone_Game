using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallScript : MonoBehaviour
{
    public GameObject player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null && collision.attachedRigidbody.GetComponent<PlayerController2D>())
        {
            PlayerController2D player = collision.attachedRigidbody.GetComponent<PlayerController2D>();
            player.RemoveHealth(30);
            //Insta kill when falling in pit;
        }
    }
}
