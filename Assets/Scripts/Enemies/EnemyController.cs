using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb2d;
    Collider2D collider2d;
    SoundManager soundManager;
    ParticleSystem hurtParticles;
    //ParticleSystem attackParticles;
    //ParticleSystem moveParticles;

    public int maxHealth;
    public int currentHealth;

    public float maxSpeed;
    public float currentSpeed;

    public int damage;

    public State currentState;
    public EnemyType enemyType;

    public enum EnemyType
    {
        bunny,
        bat,
        gorilla,
        bird,
        hotdog,
        rooster
    }

    public enum State
    {
        Dead,
        Moving,
        Attacking
    }
    private void Awake()
    {
        currentState = State.Moving;
        currentHealth = maxHealth;

        if(spriteRenderer == null)
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
        }
        if(rb2d == null)
        {
            rb2d = this.GetComponent<Rigidbody2D>();
        }

        if (collider2d == null)
        {
            collider2d = this.GetComponent<Collider2D>();
        }

        if(soundManager == null)
        {
            soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }

        if(hurtParticles == null)
        {
            hurtParticles = this.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        }

       /* if (moveParticles == null)
        {
            moveParticles = this.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();
        }

        if (attackParticles == null)
        {
            attackParticles = this.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>();
        }*/
    }
    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Attacking:
                UpdateAttackingState();
                break;
            case State.Dead:
                //TODO: CHANGE THIS BACK
                Destroy(this.gameObject);
                //UpdateDeadState();
                break;
        }
    }

    //Moving State
    void EnterMovingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }

    }
    void UpdateMovingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }
    void ExitMovingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }



    //Attacking State
    void EnterAttackingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }
    void UpdateAttackingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }
    void ExitAttackingState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }


    //Dead State
    void EnterDeadState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }
    void UpdateDeadState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }
    void ExitDeadState()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:
                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                break;
            case EnemyType.rooster:
                break;
            default:
                break;
        }
    }


    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Attacking:
                ExitAttackingState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Attacking:
                EnterAttackingState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }
    


    public void RemoveHealth(int damage)
    {
        if (currentHealth > 0)
        {
            int tempValue = currentHealth;
            tempValue -= damage;
            if (tempValue <= 0)
            {
                //Die
                currentHealth = 0;
                currentState = State.Dead;
            }
            else
            {
                //TODO: Add "flash" effect
                hurtParticles.Play();
                currentHealth = tempValue;
            }
        }
        else
        {
            //Game over!
            currentState = State.Dead;
        }
    }

}
