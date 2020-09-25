﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionChecker : MonoBehaviour
{
    //Enemy controller for this specific enemy
    EnemyController enemyController;

    private void Awake()
    {
        enemyController = this.transform.parent.gameObject.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<BulletScript>())
        {
            BulletScript bullet = other.attachedRigidbody.GetComponent<BulletScript>();
            enemyController.RemoveHealth(bullet.damage);
            Destroy(other.transform.gameObject);
        }

    }
}