using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb2d;

    public float destroyTimer;
    public float speed;
    public int damage;

    public bool destroyOnWalls = false;
    public bool destroyOnEnemy = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0.0f)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the bullet hits an enemy
        //if (collision.otherRigidbody != null && collision.otherRigidbody.GetComponent<EnemyController>())
      //  {
            /*EnemyController enemy = collision.otherRigidbody.GetComponent<EnemyController>();
            enemy.RemoveHealth(damage);
            Destroy(gameObject);*/
     //   }

        //If the bullet hits a groundLayer layer
        if (collision.gameObject.layer == 8)
        {
            if (destroyOnWalls)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the bullet hits an enemy
      /*  if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<EnemyController>())
        {
            EnemyController enemy = other.attachedRigidbody.GetComponent<EnemyController>();
            enemy.RemoveHealth(damage);
            Destroy(gameObject);
        }

        //If the bullet hits a groundLayer layer
        if (other.gameObject.layer == 8)
        {
            if (destroyOnWalls)
                Destroy(gameObject);
        }*/
    }
    public void Shoot(int dir)
    {
        if(dir == 1)
        {
            rb2d.velocity = transform.right * speed * 1;
        }
        else
        {
            rb2d.velocity = transform.right * speed * -1;
        }
        
    }
}
