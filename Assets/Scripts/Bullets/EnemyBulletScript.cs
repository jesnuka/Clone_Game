using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    Rigidbody2D rb2d;

    public float destroyTimer;
    public float speed;
    public int damage;

    public bool destroyOnWalls = false;
    public bool destroyOnEnemy = false;

    Vector3 targetDirection;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        UpdatePosition();
        destroyTimer -= Time.deltaTime;
        
        if (destroyTimer <= 0.0f)
            Destroy(gameObject);
    }

  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the bullet hits a groundLayer layer
        if (collision.gameObject.layer == 8)
        {
            if (destroyOnWalls)
                Destroy(gameObject);
        }
    }

    void UpdatePosition()
    {
        transform.position += (targetDirection * speed ) * Time.deltaTime;
    }

    public void Shoot(Vector3 direction)
    {
        //Add these back in if you want to implement left and right shooting
        /*if(dir == 1)
        {
            rb2d.velocity = transform.right * speed * 1;
        }
        else
        {
            rb2d.velocity = transform.right * speed * -1;
        }
        */

        // this.transform.position = direction;
        targetDirection = (direction - transform.position).normalized;
      //  targetDirection.z = 0;
      //  targetDirection.Normalize();
       // Debug.Log("TargetPos is " + targetDirection);
    }
}
