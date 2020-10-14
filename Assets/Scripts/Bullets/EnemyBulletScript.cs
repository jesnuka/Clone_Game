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

    public BulletType bulletType;

    float fireCurveCounter;
    //float fireCurveCounterMax;

    public enum BulletType
    {
        bullet,
        fire
    }

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
        if (bulletType == BulletType.fire)
        {
            fireCurveCounter += Time.deltaTime * 2;
            //transform.position += (targetDirection * speed) * Time.deltaTime;
            //rb2d.AddForce(-transform.right * 2f, ForceMode2D.Force);
            //rb2d.AddForce(Vector3.up * 20f * 10);
            //rb2d.velocity = new Vector2(0, 2000f);

            // rb2d.velocity = new Vector3(speed * -1, rb2d.velocity.y, 0);
            
            transform.position += new Vector3(0f, (fireCurveCounter * 4) * Time.deltaTime, 0f);
            transform.position += new Vector3((-1 * speed) * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.position += (targetDirection * speed) * Time.deltaTime;
        }
       
    }

    void FireArc()
    {

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
       

        if(bulletType == BulletType.fire)
        {
            targetDirection = (direction - transform.position).normalized;
            fireCurveCounter = -1.2f;
        }
        else
        {
            targetDirection = (direction - transform.position).normalized;
        }
      //  targetDirection.z = 0;
      //  targetDirection.Normalize();
       // Debug.Log("TargetPos is " + targetDirection);
    }
}
