using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D aliveRb2d;
    Animator aliveAnimator;
    Collider2D aliveCollider2d;
    SoundManager soundManager;

    public GameObject[] dropItemsList;

    public GameObject particleParent;
    public ParticleSystem hurtParticles;
    public ParticleSystem attackParticles;
    public ParticleSystem moveParticles;

    public int maxHealth;
    public int currentHealth;

    public float maxSpeed;
    public float currentSpeed;

    private Vector2 movement;

    public int damage;

    public State currentState;
    public EnemyType enemyType;

    private int facingDirection;
    public GameObject alive;

    [SerializeField]
    private bool inPlayerView;


    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck;

    [SerializeField]
    private LayerMask groundLayer;
    private bool groundDetected;
    private bool wallDetected;

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



    private void LateUpdate()
    {
        //Move particleParent to the alive enemy
        if(alive != null)
        {
            particleParent.transform.position = alive.transform.position;
            particleParent.transform.rotation = alive.transform.rotation;
        }
        

    }

    private void Awake()
    {
        currentState = State.Moving;
        currentHealth = maxHealth;

        if(spriteRenderer == null)
        {
            spriteRenderer = alive.GetComponent<SpriteRenderer>();
        }
        if(aliveRb2d == null)
        {
            aliveRb2d = alive.GetComponent<Rigidbody2D>();
        }

        if (aliveCollider2d == null)
        {
            aliveCollider2d = alive.GetComponent<Collider2D>();
        }

        if(soundManager == null)
        {
            soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }

       if(aliveAnimator == null)
        {
            aliveAnimator = alive.GetComponent<Animator>();
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
    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        facingDirection = 1;
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
                DropItem();
                // Destroy(alive.gameObject);
                //UpdateDeadState();
                break;
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckDetections()
    {

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

                groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

               // Debug.Log("Wall? " + wallDetected + " ground? " + groundDetected);
                if (!groundDetected || wallDetected)
                {
                    //Flip enemy around
                   // Debug.Log("IFWall? " + wallDetected + " ground? " + groundDetected);
                    Flip();
                }
                else
                {
                  //  Debug.Log("ELSEWall? " + wallDetected + " ground? " + groundDetected);
                    //Move the enemy
                    movement.Set(currentSpeed * facingDirection, aliveRb2d.velocity.y);
                    aliveRb2d.velocity = movement;
                }

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
        //Spawn particles etc, effects
        
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
                soundManager.PlaySound(SoundManager.Sound.enemyTakeDamage, 1f);
                currentHealth = tempValue;
            }
        }
        else
        {
            //Game over!
            currentState = State.Dead;
        }
    }

    void DestroyEnemy()
    {
        Destroy(alive.gameObject);
    }

    public void DropItem()
    {
        if(alive != null)
        {
            //RandomNum can be removed, if items are later chosen depending on dropChance
          //  int randomNum = Random.Range(0, dropItemsList.Length);
            float dropChance = Random.Range(0.0f, 100f);


            //With more items, add more to list. If specific items should have different drop chance, add if- here, and choose from list [n]


            
            if ((dropChance > 50f) && (dropChance <= 75f)) //Large health pickup
            {
                GameObject itemDrop = dropItemsList[1];
                Instantiate(itemDrop, alive.transform.position, alive.transform.rotation);
                Destroy(alive.gameObject);

            }
            else if ((dropChance > 0f) && (dropChance <= 40f)) //Small health pickup
            {
                GameObject itemDrop = dropItemsList[0];
                Instantiate(itemDrop, alive.transform.position, alive.transform.rotation);
                Destroy(alive.gameObject);

            }
            else
            {
                Destroy(alive.gameObject);
            }
            //Kill enemy afterwards
            //Invoke("DestroyEnemy", 1f);
        }

    }

}
