using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionChecker : MonoBehaviour
{
    //Enemy controller for this specific enemy
    public EnemyController enemyController;

    public LayerMask ignoreLayers;

    public EnemyController.EnemyType enemyType;

    private void Awake()
    {
        enemyController = this.transform.parent.gameObject.GetComponent<EnemyController>();
        enemyType = enemyController.enemyType;
    }

    public int GetDamage()
    {
        return enemyController.damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<BulletScript>())
        {
            BulletScript bullet = other.attachedRigidbody.GetComponent<BulletScript>();
            enemyController.RemoveHealth(bullet.damage);
            Destroy(other.transform.gameObject);
        }

        if (ignoreLayers == (ignoreLayers | (1 << other.gameObject.layer)))
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), other);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 14)
        {
            enemyController.CeilingHit();
            // Destroy(gameObject);
        }
        if(enemyType == EnemyController.EnemyType.bat) // Bats can fly through floor / blocks
        {
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 10) //If collider is ground collider
            {
                Debug.Log("Ground, ignored");
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.collider);
            }
        }
        
    }
        //Not used currently, right now player has this instead
        /*private void OnCollisionEnter2D(Collision2D collision)
        {
            //If enemy touches player
            if (collision.otherRigidbody != null && collision.otherRigidbody.GetComponent<PlayerController2D>())
            {
                PlayerController2D player = collision.otherRigidbody.GetComponent<PlayerController2D>();
                player.RemoveHealth(1);
                enemyController.TouchedPlayer();
               // Destroy(gameObject);
            }

        }*/
    }
