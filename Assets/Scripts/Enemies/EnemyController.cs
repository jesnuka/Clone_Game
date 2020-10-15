using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Camera mainCamera;
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
    public bool batIsSleeping;
    public bool batIsFleeing;
    Vector3 batFleePos;

    public GameObject droneSpriteObject;
    public GameObject droneSpriteObject2;

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

    public bool spawnAreaActive;
    public GameObject hotdogBulletHole;
    public int hotDogSpawnAmount;

    // -- Hotdog related ends

    // -- Gorilla related starts

    public float gorillaWaitTimer;
    public float gorillaWaitTimerMax;

    public float gorillaJumpTimer;
    public float gorillaJumpTimerMax;

    public float gorillaSpawnJumpTimer;
    public float gorillaSpawnJumpTimerMax;

    bool gorillaIsWaiting;
    public bool hasPlatform;
    public bool isOutside;

    bool gorillaFirstJumped;

    public GameObject chickenSpriteObject;
    public GameObject chickenSpriteObject2;

    //This is to save time, allows to choose which platform collision to ignore
    public GameObject targetPlatform;

    // -- Gorilla related ends


    // -- ROOSTER BOSS related starts

    public float roosterMoveTimer;
    public float roosterMoveTimerMax;

    public float roosterWaitTimer;
    public float roosterWaitTimerMax;

    public float roosterShootTimer;
    public float roosterShootTimerMax;

    public int roosterShootCounter;
    public int roosterShootCounterMax;

    public bool roosterCanMove;

    public GameObject shootPos;

    public bool roosterCutsceneOver;

    public GameObject bossCutsceneManager;
    public ParticleSystem bossHurtParticles2;

    // -- ROOSTER BOSS related ends




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
        rooster,
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
        mainCamera = Camera.main;
        currentState = State.Moving;
        currentHealth = maxHealth;
        player = GameObject.Find("Player");
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        
        if(enemyType == EnemyType.bat)
        {
            batIsSleeping = true;
            float rand = Random.Range(1, batSleepTimerMax);
            batSleepTimer = rand;
        }
        //Despawn, once player is in area, spawn them again
        DespawnEnemy();

        SetVariableReferences();

    }

    public void CheckPlayerDistance()
    {
        //halfScreenWidth Is current half of screen size width, change if screensize changes from player

        float spawnerDistanceToPlayerX = player.transform.position.x - this.transform.position.x;
        float spawnerDistanceToPlayerY = player.transform.position.y - this.transform.position.y;
        float aliveDistanceToPlayerX = player.transform.position.x - alive.transform.position.x;
        float aliveDistanceToPlayerY = player.transform.position.y - alive.transform.position.y;

        Vector3 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float halfScreenWidth = player.GetComponent<PlayerController2D>().halfScreenWidth;
        float halfScreenHeight = player.GetComponent<PlayerController2D>().halfScreenHeight;

        if (alive.activeSelf == false) // Is alive currently despawned?
        {
            if(mainCamera.gameObject.GetComponent<CameraManager>().currentMode == CameraManager.Mode.Follow) //Use certain width and height when follow camera on, otherwise screensize is the same constantly
            {
                if ((spawnerDistanceToPlayerX >= -halfScreenWidth) && (spawnerDistanceToPlayerX <= halfScreenWidth) && (spawnerDistanceToPlayerY >= -halfScreenHeight) && (spawnerDistanceToPlayerY <= halfScreenHeight) && !spawnedOnce)
                {
                    //Player is close enough, spawn enemy!
                    if(enemyType == EnemyType.hotdog)
                    {
                        if(hotDogSpawnAmount > 0)
                        {
                            RespawnEnemy();
                            spawnedOnce = true;
                        }
                        else
                        {
                            //Dont
                        }
                    }
                    else
                    {
                        RespawnEnemy();
                        spawnedOnce = true;
                    }
                    
                
                }
                else if (((spawnerDistanceToPlayerX < -halfScreenWidth) || (spawnerDistanceToPlayerX > halfScreenWidth))) //&& ((spawnerDistanceToPlayerY < -halfScreenHeight) || (spawnerDistanceToPlayerY > halfScreenHeight)))
                {
                    spawnedOnce = false;
                }
            }
            else
            {
                if(spawnAreaActive && !spawnedOnce)
                {
                    //  Debug.Log("Spawn!! and once is "+spawnedOnce + " and is active?!?!? + " +alive.activeSelf);
                    if (enemyType == EnemyType.hotdog)
                    {
                        if (hotDogSpawnAmount > 0)
                        {
                            RespawnEnemy();
                            spawnedOnce = true;
                        }
                        else
                        {
                            //Dont
                        }
                    }
                    else
                    {
                        RespawnEnemy();
                        spawnedOnce = true;
                    }
                }
                else if (!spawnAreaActive)
                {
                   // Debug.Log("ELIF Spawn!! and once is " + spawnedOnce + " and is active?!?!? + " + alive.activeSelf);
                    spawnedOnce = false;
                }
            }
         /*   else //Camera is stationary or transition, use new spawn mode
            {
                if (((spawnerDistanceToPlayerX >= -halfScreenWidth * 1.5) && (spawnerDistanceToPlayerX <= halfScreenWidth * 1.5) && !spawnedOnce) && (spawnerDistanceToPlayerY >= -halfScreenHeight * 2) && (spawnerDistanceToPlayerY <= halfScreenHeight * 2))
                {
                    Debug.Log("Respawn else");
                    //Player is close enough, spawn enemy!
                    RespawnEnemy();
                    spawnedOnce = true;

                }
                else if (((spawnerDistanceToPlayerX < -halfScreenWidth * 1) || (spawnerDistanceToPlayerX > halfScreenWidth * 1)) && ((spawnerDistanceToPlayerY < -halfScreenHeight) || (spawnerDistanceToPlayerY > halfScreenHeight)))
                {
                    Debug.Log("spawned once else??");
                    spawnedOnce = false;
                }
            }*/
        }
        else //Alive is active, do not respawn when close enough, but instead if far enough from ALIVE, despawn it.
        {
            if (mainCamera.gameObject.GetComponent<CameraManager>().currentMode == CameraManager.Mode.Follow) //Use certain width and height when follow camera on, otherwise screensize is the same constantly
            {
                if (((aliveDistanceToPlayerX < -halfScreenWidth) || (aliveDistanceToPlayerX > halfScreenWidth))) //&& ((aliveDistanceToPlayerY < -halfScreenHeight ) || (aliveDistanceToPlayerY > halfScreenHeight)))
                {
                    DespawnEnemy();
                }
            }
            else
            {
                if(!spawnAreaActive)
                {
                    DespawnEnemy();
                }
            }
           /* else //Camera is stationary or transition, use new despawn mode
            {
                if (((aliveDistanceToPlayerX < -halfScreenWidth*1) || (aliveDistanceToPlayerX > halfScreenWidth * 1)) && ((aliveDistanceToPlayerY < -halfScreenHeight * 2) || (aliveDistanceToPlayerY > halfScreenHeight * 2)))
                {
                    Debug.Log("Despawn else");
                    DespawnEnemy();
                }
            }*/
            
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
                    if(enemyType == EnemyType.rooster)
                    {
                        DespawnBossWin();

                    }
                    else
                    {
                        DropItem();
                    }
                    
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
        if(enemyType == EnemyType.bat)
        {
            droneSpriteObject.SetActive(false);
            droneSpriteObject2.SetActive(true);
            droneSpriteObject2.GetComponent<Animator>().Play("Drone_Wait");
            batIsSleeping = true;
            float rand = Random.Range(1, batSleepTimerMax);
            batSleepTimer = rand;
        }

        else if(enemyType == EnemyType.gorilla)
        {
            gorillaFirstJumped = false;
            //Do first jump to platform, set ignorelayer
            if(hasPlatform)
            {
                Physics2D.IgnoreCollision(aliveCollider2d, targetPlatform.GetComponent<Collider2D>(), true);
                gorillaSpawnJumpTimer = gorillaSpawnJumpTimerMax;

                //Do first leap here as well!
                facingDirection = -1;
                if(isOutside)
                {
                    movement.Set(1.5f * facingDirection, aliveRb2d.velocity.y + 20f);
                }
                
                aliveRb2d.velocity = movement;
            }
            else
            {
                gorillaIsWaiting = true;
            }
            
        }

        else if(enemyType == EnemyType.rooster) //BOSS
        {
            //Physics2D.IgnoreCollision(aliveCollider2d, targetPlatform.GetComponent<Collider2D>(), true);
            roosterWaitTimer = roosterWaitTimerMax;
        }

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
                    // alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 0f, alive.transform.rotation.z, alive.transform.rotation.w);
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180f, alive.transform.rotation.z, alive.transform.rotation.w);
                   // spriteRenderer.flipX = true;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                    //alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180f, alive.transform.rotation.z, alive.transform.rotation.w);
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 0f, alive.transform.rotation.z, alive.transform.rotation.w);
                   // spriteRenderer.flipX = false;
                }

                if (bunnyIsWaiting) //Pause between movements
                {
                    aliveRb2d.velocity = new Vector3(0f, aliveRb2d.velocity.y, 0f);//Vector3.zero;

                   // Debug.Log("Velocity is is " + movement);
                    bunnyWaitTimer -= Time.deltaTime;
                    bunnyShootTimer -= Time.deltaTime;

                    if ((bunnyShootTimer < 0) && bunnyShootCounter > 0) //Shoot, reset timer
                    {
                        soundManager.PlaySound(SoundManager.Sound.zombieShoot, 1f, true, this.transform.position);
                        bunnyShootTimer = bunnyShootTimerMax;
                        bunnyShootCounter -= 1;

                        GameObject bullet = Instantiate(bulletList[0]);
                        bullet.transform.parent = this.transform;
                        bullet.transform.position = new Vector3(alive.transform.position.x, alive.transform.position.y+ 1.5f, alive.transform.position.z);
                        bullet.GetComponent<EnemyBulletScript>().Shoot(player.transform.position);

                        //bullet.transform.position = new Vector3(transform.position.x + 2f * shootDir, transform.position.y + 0.25f, 0f);
                    }
                    //  Debug.Log("Waitin");
                    if (bunnyWaitTimer < 0)
                    {
                    //    Debug.Log("Wait STOP");
                        bunnyIsWaiting = false;
                        bunnyWalkTimer = bunnyWalkTimerMax;
                        soundManager.PlaySound(SoundManager.Sound.walkerMove, 1f, true, this.transform.position);
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
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 0f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = false;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = true;
                }

                if (batIsFleeing) //Hit player, flee,
                {
                    alive.transform.position = Vector3.MoveTowards(alive.transform.position, batFleePos, currentSpeed*4);
                    if(alive.transform.position == batFleePos)
                    {
                        droneSpriteObject.SetActive(false);
                        droneSpriteObject2.SetActive(true);
                        droneSpriteObject2.GetComponent<Animator>().Play("Drone_Wait");
                        aliveRb2d.velocity = Vector3.zero;
                        batIsFleeing = false;
                        batIsSleeping = true;
                        float rand = Random.Range(1, batSleepTimerMax);
                        batSleepTimer = rand;
                        //Set sleep animation here
                    }
                }
                else
                {
                    if (distanceToPlayer < 20) //Change distance to more accurate number
                    {
                        if (!batIsSleeping) //If awake, allow movement
                        {
                            droneSpriteObject2.SetActive(false);
                            droneSpriteObject.SetActive(true);
                            droneSpriteObject.GetComponent<Animator>().Play("Drone_Attack1");
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

                    if (distanceToPlayer < 1) // If player is invincible, bat does NOT fly off in MM2
                    {
                      //  TouchedPlayer();
                    }
                }

                break;
            case EnemyType.gorilla:
                groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance * 1.5f, groundLayer);
                if (player.transform.position.x > alive.transform.position.x) //Player is on the right side
                {
                    facingDirection = 1;
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180f, alive.transform.rotation.z, alive.transform.rotation.w);
                    //spriteRenderer.flipX = true;
                   // chickenSpriteObject2.GetComponent<SpriteRenderer>().flipX = true;
                    //chickenSpriteObject.GetComponent<SpriteRenderer>().flipX = true;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                    alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 0, alive.transform.rotation.z, alive.transform.rotation.w);
                    //spriteRenderer.flipX = false;
                    //chickenSpriteObject2.GetComponent<SpriteRenderer>().flipX = false;
                   // chickenSpriteObject.GetComponent<SpriteRenderer>().flipX = false;
                }

                if (gorillaFirstJumped) //Has already spawn jumped
                {
                    

                    if(groundDetected)
                    {
                        chickenSpriteObject2.SetActive(false);
                        chickenSpriteObject.SetActive(true);
                    }
                    else
                    {
                        if(isOutside)
                        {
                            chickenSpriteObject2.SetActive(true);
                            chickenSpriteObject.SetActive(false);
                        }
                        else
                        {
                            if(!gorillaIsWaiting)
                            {
                                chickenSpriteObject2.SetActive(true);
                                chickenSpriteObject.SetActive(false);
                            }
                        }
                            
                           
                    }
                    if (gorillaIsWaiting) //Wait, then after time runs out, start 
                    {
                        
                        // aliveRb2d.velocity = new Vector3(0f, aliveRb2d.velocity.y, 0f); //Pause between
                        gorillaWaitTimer -= Time.deltaTime;
                        if (gorillaWaitTimer < 0)
                        {
                           // chickenSpriteObject2.SetActive(true);
                          //  chickenSpriteObject.SetActive(false);
                            soundManager.PlaySound(SoundManager.Sound.chickenJump, 1f, true, this.transform.position);
                            gorillaIsWaiting = false;
                          //  gorillaJumpTimer = gorillaJumpTimerMax;
                           // chickenSpriteObject2.SetActive(true);
                           // chickenSpriteObject.SetActive(false);

                        }
                    }
                    else
                    {
                        gorillaJumpTimer -= Time.deltaTime;

                      //  chickenSpriteObject2.SetActive(true);
                       // chickenSpriteObject.SetActive(false);
                        if (gorillaJumpTimer < 0)
                        {
                            // chickenSpriteObject2.SetActive(false);
                            //  chickenSpriteObject.SetActive(true);
                           
                            gorillaIsWaiting = true;
                            gorillaWaitTimer = gorillaWaitTimerMax;
                          //  chickenSpriteObject2.SetActive(true);
                           // chickenSpriteObject.SetActive(false);
                        }
                        //aliveRb2d.velocity = new Vector3(0f, aliveRb2d.velocity.y, 0f);
                        movement.Set(currentSpeed/2 * facingDirection, aliveRb2d.velocity.y + 15f);
                        aliveRb2d.velocity = movement;
                    }
                }
                else
                {
                    gorillaSpawnJumpTimer -= Time.deltaTime;
                    aliveRb2d.velocity = movement;
                    chickenSpriteObject2.SetActive(true);
                    chickenSpriteObject.SetActive(false);
                    if (gorillaSpawnJumpTimer < 0)
                    {
                        soundManager.PlaySound(SoundManager.Sound.chickenJump, 1f, true, this.transform.position);
                        gorillaFirstJumped = true;
                        if (hasPlatform)
                        {
                            Physics2D.IgnoreCollision(aliveCollider2d, targetPlatform.GetComponent<Collider2D>(), false);
                        }
                        gorillaIsWaiting = true;
                        gorillaWaitTimer = gorillaWaitTimerMax;
                    }
                }
                
                    break;
            case EnemyType.bird:
                break;
            case EnemyType.hotdog:
                if (player.transform.position.x > alive.transform.position.x) //Player is on the right side
                {
                    facingDirection = 1;
                    // alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 0f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = true;

                }
                else //Player is on the left side
                {
                    facingDirection = -1;
                    //alive.transform.rotation = new Quaternion(alive.transform.rotation.x, 180f, alive.transform.rotation.z, alive.transform.rotation.w);
                    spriteRenderer.flipX = false;
                }

                if (hotdogIsWaiting) //Wait, then after time runs out, start shooting
                {
                    hotdogWaitTimer -= Time.deltaTime;
                    if(hotdogWaitTimer < 0 )
                    {
                        hotdogIsWaiting = false;
                        hotdogShootTimer = hotdogShootTimerMax;
                        hotdogShootCounter = hotdogShootCounterMax;
                        soundManager.PlaySound(SoundManager.Sound.pipeShoot, 1f, true, this.transform.position);
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
                        bullet.transform.position = hotdogBulletHole.transform.position;//alive.transform.position;
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
                //ROOSTER == BOSS!
                if(roosterCutsceneOver)
                {
                    if(roosterCanMove) //Allows moving and shooting!
                    {
                        Debug.Log("Canmove");
                        roosterMoveTimer -= Time.deltaTime;
                        roosterShootTimer -= Time.deltaTime;

                        if(roosterShootTimer < 0 && roosterShootCounter > 0) //Shoot, shoots multiple times per movement
                        {
                            roosterShootTimer = roosterShootTimerMax;
                            roosterShootCounter -= 1;

                            GameObject bullet = Instantiate(bulletList[0]);
                            bullet.transform.parent = this.transform;
                            bullet.transform.position = shootPos.transform.position;
                            bullet.GetComponent<EnemyBulletScript>().Shoot(player.transform.position);
                        }

                        if(roosterMoveTimer < 0)
                        {
                            Debug.Log("eback to waitina");
                            roosterCanMove = false; //Start waiting!
                            roosterWaitTimer = roosterWaitTimerMax;
                        }
                        else
                        {
                            //Always goes left
                            movement.Set(currentSpeed * -1, 0);
                            aliveRb2d.velocity = movement;
                        }
                    
                    }
                    else //Wait, until act again
                    {
                        Debug.Log("Waiting");
                        aliveRb2d.velocity = new Vector3(0f, 0f, 0f);
                        roosterWaitTimer -= Time.deltaTime;
                        if(roosterWaitTimer < 0)
                        {
                            Debug.Log("eeaa");
                            roosterCanMove = true;
                            roosterMoveTimer = roosterMoveTimerMax;
                            roosterShootTimer = roosterShootTimerMax;
                            roosterShootCounter = roosterShootCounterMax;
                        }
                    }

                }
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

                if(batIsFleeing)
                {
                    //TODO add sleep animation here!
                    aliveRb2d.velocity = Vector3.zero;
                    batIsFleeing = false;
                    batIsSleeping = true;
                    float rand = Random.Range(1, batSleepTimerMax);
                    batSleepTimer = rand;
                }
                

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

                droneSpriteObject.SetActive(false);
                droneSpriteObject2.SetActive(true);
                droneSpriteObject2.GetComponent<Animator>().Play("Drone_Wait");
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
                if ((enemyType == EnemyType.bat) && batIsSleeping) //Bat is invincible during sleep
                {
                    //Dont take dmg, add some clink effect
                }
                else
                {
                    if (enemyType != EnemyType.rooster)
                    {
                        hurtParticles.Play();
                        soundManager.PlaySound(SoundManager.Sound.enemyTakeDamage, 1f, true, this.transform.position);
                        currentHealth = 0;
                        currentState = State.Dead;
                    }

                    else // Only for boss
                    {
                        hurtParticles.Play();
                        bossHurtParticles2.Play();
                        soundManager.PlaySound(SoundManager.Sound.enemyTakeDamage, 1f, true, this.transform.position);
                        currentHealth = 0;
                        currentState = State.Dead;
                    }

                        
                }
                
            }
            else
            {
                if ((enemyType == EnemyType.bat) && batIsSleeping) //Bat is invincible during sleep
                {
                    //Dont take dmg, add some clink effect
                }
                else
                {
                    if (enemyType != EnemyType.rooster)
                    {
                        hurtParticles.Play();
                        soundManager.PlaySound(SoundManager.Sound.enemyTakeDamage, 1f, true, this.transform.position);
                        currentHealth = tempValue;
                    }
                    else
                    {
                       // hurtParticles.Play();
                        soundManager.PlaySound(SoundManager.Sound.chickenJump, 1f, true, this.transform.position);
                        currentHealth = tempValue;
                    }
                    
                }
                
            }
        }
        else
        {
            //Game over!
            currentState = State.Dead;
        }
    }

    void DespawnBossWin() //For when you win
    {
        if (enemyType == EnemyType.rooster)
        {
            alive.gameObject.SetActive(false);
        }
    }
    void DespawnEnemy()
    {
        // Destroy(alive.gameObject);

        if(enemyType == EnemyType.rooster) //For normal scenarios
        {
            roosterCutsceneOver = false;
            bossCutsceneManager.GetComponent<BossCutscene>().ResetStats();
        }
        alive.gameObject.SetActive(false);
    }

    public void DropItem()
    {
        if(alive != null && enemyType != EnemyType.rooster)
        {
            //RandomNum can be removed, if items are later chosen depending on dropChance
          //  int randomNum = Random.Range(0, dropItemsList.Length);
            float dropChance = Random.Range(0.0f, 100f);
            //Debug.Log("Dropchance: " + dropChance);

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
            if(enemyType == EnemyType.hotdog)
            {
                hotDogSpawnAmount -= 1;
            }
            if (enemyType == EnemyType.bat)
            {
                soundManager.PlaySound(SoundManager.Sound.enemyDieDrone, 0.8f, true, this.transform.position);
            }
            else if (enemyType == EnemyType.bunny)
            {
                soundManager.PlaySound(SoundManager.Sound.enemyDieWalker, 0.8f, true, this.transform.position);
            }
            else if (enemyType == EnemyType.gorilla)
            {
                soundManager.PlaySound(SoundManager.Sound.enemyDieChicken, 0.8f, true, this.transform.position);
            }
            else if (enemyType == EnemyType.hotdog)
            {
                soundManager.PlaySound(SoundManager.Sound.enemyDiePipe, 0.8f, true, this.transform.position);
            }
            DespawnEnemy();
        }

    }

}
