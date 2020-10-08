using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    public Type itemType;
    public int healthIncrease;
    GameObject player;

    public LayerMask ignoreLayers;

    public enum Type
    {
        healthSmall,
        healthLarge,
        ammoSmall,
        ammoLarge,
        lifePickup
    }
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    public void CheckPlayerDistance()
    {
        //18 Is current half of screen size width, change if screensize changes

        //Player far enough, go far
        float distanceToPlayer = player.transform.position.x - this.transform.position.x;
        float halfScreenWidth = player.GetComponent<PlayerController2D>().halfScreenWidth;

        if ((distanceToPlayer < -halfScreenWidth) || (distanceToPlayer > halfScreenWidth))
        {
            Destroy(transform.gameObject);
        }

        if(player.GetComponent<PlayerController2D>().currentState == PlayerController2D.State.Respawning)
        {
            Destroy(transform.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(itemType == Type.healthLarge || itemType == Type.healthSmall)
        {
            if (other.otherRigidbody != null && other.gameObject.GetComponent<PlayerController2D>())
            {
                PlayerController2D playerController = other.gameObject.GetComponent<PlayerController2D>();
                playerController.IncreaseHealth(healthIncrease);
                Destroy(transform.gameObject);
            }
        }
        else if(itemType == Type.lifePickup)
        {
            if (other.otherRigidbody != null && other.gameObject.GetComponent<PlayerController2D>())
            {
                PlayerController2D playerController = other.gameObject.GetComponent<PlayerController2D>();
                playerController.IncreaseLives(1);
                Destroy(transform.gameObject);
            }
        }
        

        if (ignoreLayers == (ignoreLayers | (1 << other.gameObject.layer)))
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), other.collider);
        }

    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerDistance();
    }
}
