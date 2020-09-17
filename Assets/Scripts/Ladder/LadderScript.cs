using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    Collider2D ladderCollider;
    public GameObject playerObject;
    public PlayerController2D player;
    public Rigidbody2D playerRigidbody;
    public Collider2D playerCollider;

    public GameObject ladderParent;

    public bool canCollide;

    private void Awake()
    {
        if(playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        if(ladderCollider == null)
        {
            ladderCollider = this.GetComponent<Collider2D>();
        }
        if (ladderParent == null)
        {
            ladderParent = this.transform.parent.gameObject;
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        }
        if(playerRigidbody == null)
        {
            playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        }
        if(playerCollider == null)
        {
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        }
    }

    private void Update()
    {
        if(!(player.currentState == PlayerController2D.State.Climbing))
        {
            canCollide = true;
        }
        if ((player.currentState == PlayerController2D.State.Climbing))
        {
            Physics2D.IgnoreCollision(playerCollider, ladderCollider);
            canCollide = false;
        }
        if (canCollide)
        {
            Physics2D.IgnoreCollision(playerCollider, ladderCollider, false);
        }
    }

}
