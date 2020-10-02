using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionChecker : MonoBehaviour
{
    //Enemy controller for this specific enemy
    public EnemyController enemyController;

    public LayerMask ignoreLayers;

    private void Awake()
    {
        enemyController = this.transform.parent.gameObject.GetComponent<EnemyController>();
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
        //if the bullet hits an enemy
        if (collision.otherRigidbody != null && collision.otherRigidbody.GetComponent<PlayerController2D>())
        {
            PlayerController2D player = collision.otherRigidbody.GetComponent<PlayerController2D>();
            player.RemoveHealth(1);
           // Destroy(gameObject);
        }

    }
}
