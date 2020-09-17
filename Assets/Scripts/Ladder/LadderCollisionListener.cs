using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderCollisionListener : MonoBehaviour
{
    public GameObject playerObject;
    public PlayerController2D player;

    bool touchingPlayer;

    private void Awake()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        }
    }

    private void Update()
    {
        if(touchingPlayer && player.currentState == PlayerController2D.State.Climbing)
        {
            playerObject.transform.position = new Vector3(transform.position.x, playerObject.transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // Debug.Log("Whee");
        if (collision.tag == "Player")
        {
            touchingPlayer = true;

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            touchingPlayer =  false;
    }
}
