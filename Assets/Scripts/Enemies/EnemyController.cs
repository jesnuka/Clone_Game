using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    Rigidbody2D aliveRb2d;
    Animator aliveAnimator;
    Collider2D aliveCollider2d;
    SoundManager soundManager;
    public GameObject player;

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

    //This is different for each type of enemy, it is set beforehand!
    //it determines what type of an enemy this spawns
    public GameObject enemyTypeGameObject;
    public GameObject alive;

    [SerializeField]
    private bool inPlayerView;


    public GameObject[] bulletList;

    // -- Bat related starts
    public float batSleepTimer;
    public float batSleepTimerMax;
    bool batIsSleeping;
    bool batIsFleeing;
    Vector3 batFleePos;
    // -- Bat related ends

    // -- Bunny related starts
    public float bunnyWalkTimer;
    public float bunnyWalkTimerMax;

    public float bunnyWaitTimer;
    public float bunnyWaitTimerMax;

    public float bunnyShootTimer;
    public float bunnyShootTimerMax;

    public int bunnyShootCounter;
    public int bunnyShootCounterMax;

    //Vector3 bunnyMovePos;
    bool bunnyIsWaiting;
    // -- Bunny related ends


    // -- Hotdog related starts
    public float hotdogWaitTimer;
    public float hotdogWaitTimerMax;

    public float hotdogShootTimer;
    public float hotdogShootTimerMax;
    public int hotdogShootCounter;
    public int hotdogShootCounterMax;

    bool hotdogIsWaiting;

    // -- Hotdog related ends


    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck;

    //If spawned once, makes sure respawn does not happen if enemy dies close to the respawn point!
    bool spawnedOnce;
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
        Attacking,
        ReadyToSpawn
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
        player = GameObject.Find("Player");
        

        //Despawn, once player is in area, spawn them again
        DespawnEnemy();

        SetVariableReferences();

    }

    public void CheckPlayerDistance()
    {
        //halfScreenWidth Is current half of screen size width, change if screensize changes from player

        float spawnerDistanceToPlayer = player.transform.position.x - this.transform.position.x;
        float aliveDistanceToPlayer = player.transform.position.x - alive.transform.position.x;

        float halfScreenWidth = player.GetComponent<PlayerController2D>().halfScreenWidth;

        if (alive.activeSelf == false) // Is alive currently despawned?
        {
            if ((spawnerDistanceToPlayer >= -halfScreenWidth) && (spawnerDistanceToPlayer <= halfScreenWidth) && !spawnedOnce)
            {
                //Player is close enough, spawn enemy!
                RespawnEnemy();
                spawnedOnce = true;
            }
            else if ((spawnerDistanceToPlayer < -halfScreenWidth) || (spawnerDistanceToPlayer > halfScreenWidth))
            {
                spawnedOnce = false;
            }
            else
            {
                //Nothing needed here
            }
        }
        else //Alive is active, do not respawn when close enough, but instead if far enough from ALIVE, despawn it.
        {

            if ((aliveDistanceToPlayer < -halfScreenWidth) || (aliveDistanceToPlayer > halfScreenWidth))
            {
                DespawnEnemy();
            }
        }
            
        
    }

    void SetVariableReferences()
    {
        spriteRenderer = alive.GetComponent<SpriteRenderer>();
        aliveRb2d = alive.GetComponent<Rigidbody2D>();
        aliveCollider2d = alive.GetComponent<Collider2D>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        aliveAnimator = alive.GetComponent<Animator>();
        alive.GetComponent<EnemyCollisionChecker>().enemyController = this.GetComponent<EnemyController>();
        alive.GetComponent<EnemyCollisionChecker>().SetVariables();
    }

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        spriteRenderer = alive.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();
        facingDirection = 1;
    }
    private void Update()
    {
        CheckPlayerDistance();
        if(alive.activeSelf == true)
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
                    // RespawnEnemy();
                    // Destroy(alive.gameObject);
                    //UpdateDeadState();
                    break;
            }
        }
        
    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }


    private void RespawnEnemy()
    {
        /*GameObject aliveNew = Instantiate(enemyTypeGameObject);
        aliveNew.transform.parent = this.transform;
        aliveNew.transform.position = this.transform.position;
        alive = aliveNew;*/

        alive.transform.position = this.transform.position;
        currentHealth = maxHealth;
        alive.gameObject.SetActive(true);
        this.currentState = State.Moving;
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

                //groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                //wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

                //Shooting happens seperately

                /* else
                 {
                    bunnyShootTimer -= Time.deltaTime
                 }*/
                if (player.transform.position.x > alive.transform.position.x) //Player is on the right side
                {
                    facingDirection = 1;
                  //  alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180.0f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = true;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                 //   alive.transform.rotation = new Quaternion(alive.transform.rotation.x, -180.0f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = false;
                }

                if (bunnyIsWaiting) //Pause between movements
                {
                    aliveRb2d.velocity = new Vector3(0f, aliveRb2d.velocity.y, 0f);//Vector3.zero;

                   // Debug.Log("Velocity is is " + movement);
                    bunnyWaitTimer -= Time.deltaTime;
                    bunnyShootTimer -= Time.deltaTime;

                    if ((bunnyShootTimer < 0) && bunnyShootCounter > 0) //Shoot, reset timer
                    {
                        bunnyShootTimer = bunnyShootTimerMax;
                        bunnyShootCounter -= 1;

                        GameObject bullet = Instantiate(bulletList[0]);
                        bullet.transform.parent = this.transform;
                        bullet.transform.position = alive.transform.position;
                        bullet.GetComponent<EnemyBulletScript>().Shoot(player.transform.position);

                        //bullet.transform.position = new Vector3(transform.position.x + 2f * shootDir, transform.position.y + 0.25f, 0f);
                    }
                    //  Debug.Log("Waitin");
                    if (bunnyWaitTimer < 0)
                    {
                    //    Debug.Log("Wait STOP");
                        bunnyIsWaiting = false;
                        bunnyWalkTimer = bunnyWalkTimerMax;
                        //bunnyMovePos =
                        //Set movement
                    }
                }
                else
                {
                    bunnyWalkTimer -= Time.deltaTime;

                    if(bunnyWalkTimer > 0) // Walk until timer runs out
                    {
                       // Debug.Log("Walkin!");
                       

                        movement.Set(currentSpeed * facingDirection, aliveRb2d.velocity.y);
                        aliveRb2d.velocity = movement;
                       // Debug.Log("Movement is "+movement);
                    }
                    else //Time ran out, now wait 
                    {
                      //  Debug.Log("Walk STOP");
                        bunnyIsWaiting = true;
                        bunnyWaitTimer = bunnyWaitTimerMax;
                        bunnyShootTimer = bunnyShootTimerMax;
                        bunnyShootCounter = bunnyShootCounterMax;
                        aliveRb2d.velocity = Vector3.zero;
                    }
                    
                }

                break;
            case EnemyType.bat:
                // groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                // wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

                float distanceToPlayer = Vector3.Distance(alive.transform.position, player.transform.position);
                //Debug.Log("Bat distance to player is: " + distanceToPlayer);

                if (player.transform.position.x > alive.transform.position.x) //Player is on the right side
                {
                    facingDirection = 1;
                    spriteRenderer.flipX = false;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                    spriteRenderer.flipX = true;
                }

                if (batIsFleeing) //Hit player, flee,
                {
                    alive.transform.position = Vector3.MoveTowards(alive.transform.position, batFleePos, currentSpeed*4);
                    if(alive.transform.position == batFleePos)
                    {
                        aliveRb2d.velocity = Vector3.zero;
                        batIsFleeing = false;
                        batIsSleeping = true;
                        batSleepTimer = batSleepTimerMax;
                        //Set sleep animation here
                    }
                }
                else
                {
                    if (distanceToPlayer < 20) //Change distance to more accurate number
                    {
                        if (!batIsSleeping) //If awake, allow movement
                        {
                            alive.transform.position = Vector3.MoveTowards(alive.transform.position, player.transform.position, currentSpeed);
                        }
                        else //If the bat is sleeping, wake up once countdown reaches 0
                        {
                            batSleepTimer -= Time.deltaTime;
                            if (batSleepTimer < 0)
                            {
                                batIsSleeping = false;
                                //Wakeup here, change animation to moving
                            }
                        }
                        //Sees player
                    }

                   /* if (distanceToPlayer < 1) // This was moved to TouchedPlayer() function, which is called from EnemyCollisionChecker
                    {
                       
                    }*/
                }

                break;
            case EnemyType.gorilla:
                break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:

                if(hotdogIsWaiting) //Wait, then after time runs out, start shooting
                {
                    hotdogWaitTimer -= Time.deltaTime;
                    if(hotdogWaitTimer < 0 )
                    {
                        hotdogIsWaiting = false;
                        hotdogShootTimer = hotdogShootTimerMax;
                        hotdogShootCounter = hotdogShootCounterMax;
                    }
                }
                else
                {
                    hotdogShootTimer -= Time.deltaTime;
                    if ((hotdogShootTimer < 0) && hotdogShootCounter > 0) //Shoot, reset timer
                    {
                        
                        hotdogShootTimer = hotdogShootTimerMax;
                        hotdogShootCounter -= 1;

                        GameObject bullet = Instantiate(bulletList[0]); //This is fire
                        bullet.transform.parent = this.transform;
                        bullet.transform.position = alive.transform.position;
                        bullet.GetComponent<EnemyBulletScript>().Shoot(player.transform.position);

                        //bullet.transform.position = new Vector3(transform.position.x + 2f * shootDir, transform.position.y + 0.25f, 0f);
                    }
                    else if(hotdogShootCounter <= 0)
                    {
                        hotdogWaitTimer = hotdogWaitTimerMax;
                        hotdogIsWaiting = true;
                    }
                }
                

                break;
            case EnemyType.rooster:
                break;
            default:
                //Default back and forth movement, use for simple enemies if needed
               /* groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
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
                }*/
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
    
    public void CeilingHit()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:

                //TODO add sleep animation here!
                aliveRb2d.velocity = Vector3.zero;
                batIsFleeing = false;
                batIsSleeping = true;
                batSleepTimer = batSleepTimerMax;

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

    public void TouchedPlayer()
    {
        switch (enemyType)
        {
            case EnemyType.bunny:
                break;
            case EnemyType.bat:

                    batIsFleeing = true;
                    float fleeDist = Random.Range(10f, 15f);
                    batFleePos = new Vector3(alive.transform.position.x, alive.transform.position.y + fleeDist, alive.transform.position.z);

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


    public void RemoveHealth(int damage)
    {
        if (currentHealth > 0)
        {
            int tempValue = currentHealth;
            tempValue -= damage;
            if (tempValue <= 0)
            {
                //Die
                hurtParticles.Play();
                soundManager.PlaySound(SoundManager.Sound.enemyTakeDamage, 1f);
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

    void DespawnEnemy()
    {
        // Destroy(alive.gameObject);
        alive.gameObject.SetActive(false);
    }

    public void DropItem()
    {
        if(alive != null)
        {
            //RandomNum can be removed, if items are later chosen depending on dropChance
          //  int randomNum = Random.Range(0, dropItemsList.Length);
            float dropChance = Random.Range(0.0f, 100f);
            Debug.Log("Dropchance: " + dropChance);

            //With more items, add more to list. If specific items should have different drop chance, add if- here, and choose from list [n]



            if ((dropChance > 87.5f) && (dropChance <= 90f)) //Large health pickup
            {
                
                GameObject itemDrop = dropItemsList[1];
                Instantiate(itemDrop, alive.transform.position, alive.transform.rotation);
                // Destroy(alive.gameObject);

            }
            else if ((dropChance > 90f) && (dropChance <= 99f)) //Small health pickup
            {
                GameObject itemDrop = dropItemsList[0];
                Instantiate(itemDrop, alive.transform.position, alive.transform.rotation);
                //  Destroy(alive.gameObject);

            }
            else if ((dropChance > 99f) && (dropChance < 100f)) //Life pickup
            {
                GameObject itemDrop = dropItemsList[2];
                Instantiate(itemDrop, alive.transform.position, alive.transform.rotation);
                // Destroy(alive.gameObject);

            }

            else
            {
                //  Destroy(alive.gameObject);
                alive.gameObject.SetActive(false);

            }

            //Then despawn enemy
            DespawnEnemy();
        }

    }

}
