using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public float speed;

    public int enemyType;

    private enum State
    {
        Dead,
        Walking,
        Attacking
    }

}
