using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    //--Component References
    Rigidbody2D rb2d;
    public Animator playerAnimator;
    BoxCollider2D boxCollider2d;
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    SoundManager soundManager;

    public float halfScreenWidth;
    public float halfScreenHeight;

    [SerializeField]
    GameObject blackoutUI;

    public Sprite healthBarBlock;
    public Sprite healthBarEmpty;

    public GameObject gameOverUI;

    public ParticleSystem runningParticles;
    public ParticleSystem hurtParticles;
    public ParticleSystem healParticles;
    public ParticleSystem shootingParticlesRight;
    public ParticleSystem shootingParticlesLeft;

    public GameObject playerParticlesObject;
    [SerializeField]
    PhysicsMaterial2D fullFriction;
    [SerializeField]
    PhysicsMaterial2D noFriction;

    //--Weapon Related
    public GameObject[] bulletList;
    public int weaponMode;
    public int weaponAmmo;

    //--Player Stats Related
    public int playerLivesCurrent;
    public int playerHealthMax;
    public int playerHealthCurrent;


    //--Variables
    float horizontalInput;
    float verticalInput;

    //-- Animation related
    public float animationDelayTime;
    public float animationDelayTimeMax;

    public float defaultGravity;
    public float runSpeed;
    public float airControl;
    public float jumpPower;
    public float climbSpeed;

    [Range(0.0f, 1.0f)]
    public float cutJumpHeight;

    public GameObject currentCheckpoint;

    public GameObject invincibilityLight;
    public float invincibilityTime;
    public float invincibilityTimeMax;
    public float knockbackTime;
    public float knockbackTimeMax;
    public int knockbackDirection;

    public float shootCounter;
    public float shootCounterMax;
    public float shootCounterTimer;
    public float shootCounterTimerMax;
    public float shootTimer;
    public float shootTimerMax;

    public float gameOverTimer;
    public float gameOverTimerMax;

    public float jumpRememberTime;
    public float groundedRememberTime;
    float rememberJumpPress;
    float rememberGrounded;


    public float ladderCheckDistance;
    public float groundCheckDistance;
    public float groundCheckWidthDifference;
    public float slopeCheckDistance;
    public float slopeForce;
    float slopeAngle;
    float slopeAngleOld;
    float slopeSideAngle;

    Vector2 slopeNormalPerpendicular;

    bool isFacingLeft;
    bool isFacingRight;

    public bool isGrounded;
    public bool isOnSlope;
    public bool isClimbing;

    public bool touchingLadder;

    bool gameIsOver;
    public LayerMask groundLayers;
    public LayerMask groundLayersWithLadder;
    public LayerMask ladderLayer;
    public LayerMask ladderBlockLayer;
    public LayerMask enemyLayers;

    public State currentState;
    public bool isInvincible;
    public bool isOnLadder;

    public float pushTimer;
    public float pushTimerMax;
    public bool isPushed;

    public enum State
    {
        Dead,
        Climbing,
        Normal,
        Falling,
        Knockback,
        Respawning
        //Invincible
    }
    private void Awake()
    {
        //halfScreenHeight = Camera.main.pixelHeight / 2;
        //halfScreenWidth = Camera.main.pixelWidth / 2;
        currentState = State.Normal;
        isFacingRight = true;
        rb2d = this.GetComponent<Rigidbody2D>();
        boxCollider2d = this.GetComponent<BoxCollider2D>();
        boxCollider2d = this.GetComponent<BoxCollider2D>();
       /* if (soundManager == null)
        {
            soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }*/

    }



    private void OnGUI()
    {
        //Size of the GUI items
        float x = Camera.main.pixelWidth / 256.0f;
        float y = Camera.main.pixelHeight / 218.0f;
        Vector2 cmrBase = new Vector2(Camera.main.rect.x * Screen.width, Camera.main.rect.y * Screen.height);


        Sprite healthBar = healthBarBlock;

        Rect healthBarRect = new Rect(healthBar.rect.x / healthBar.texture.width, healthBar.rect.y / healthBar.texture.height,
                                healthBar.rect.width / healthBar.texture.width, healthBar.rect.height / healthBar.texture.height);

        Sprite emptyBar = healthBarEmpty;
        Rect emptyBarRect = new Rect(emptyBar.rect.x / emptyBar.texture.width, emptyBar.rect.y / emptyBar.texture.height,
                                emptyBar.rect.width / emptyBar.texture.width, emptyBar.rect.height / emptyBar.texture.height);

        for (int i = 0; i < playerHealthMax; i++)
        {
            if (playerHealthCurrent > i)
                GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + x * 24f, cmrBase.y + y * (72 - i * 2), x * 8, y * 2), healthBar.texture, healthBarRect);
            else
                GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + x * 24f, cmrBase.y + y * (72 - i * 2), x * 8, y * 2), emptyBar.texture, emptyBarRect);
        }
    }
    void Update()
    {
        if(!gameIsOver)
        {
            CheckInput();
        }
       
    }
    void FixedUpdate()
    {
        if(!gameIsOver)
        {
            animationDelayTime -= Time.deltaTime;

            if(pushTimer > 0)
            {
                currentState = State.Normal;
                playerAnimator.Play("Player_Idle");
                pushTimer -= Time.deltaTime;
            }
            else
            {
            
                isGrounded = IsGrounded();
                SlopeCheck();
                if (currentState == State.Dead)
                {
                    //  Debug.Log("GG");
                    GameOver();
                }
                else if (currentState == State.Normal)
                {
                   // Debug.Log("Moving");
                    MovePlayer();
                }
                else if(currentState == State.Climbing)
                {
                   // Debug.Log("Climbing");
                    ClimbPlayer();
                }

                else if (currentState == State.Knockback)
                {
                    // Debug.Log("Knockback");
                    knockbackTime -= Time.deltaTime;
                    KnockbackPlayer(knockbackDirection);
                }
                else if (currentState == State.Respawning)
                {
                    // Debug.Log("Respawning");
                    // knockbackTime -= Time.deltaTime;
                    rb2d.velocity = Vector3.zero;
                    RespawnPlayer();
                }
            
                if (isInvincible)
                {
                    // Debug.Log("Invincible");
                    invincibilityTime-= Time.deltaTime;
                    InvincibilityFrames();
                }

                else if(currentState == State.Falling)
                {
                    FallPlayer();
                }
            }
        }
        
    }

    void GameOver()
    { 
        gameOverUI.SetActive(true);
        gameIsOver = true;
    }

    void CheckInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        shootCounterTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        if(shootCounterTimer < 0)
        {
            shootCounterTimer = shootCounterTimerMax;
            if(shootCounter < shootCounterMax)
            {
                shootCounter += 1;
            }
        }

        if (horizontalInput > 0)
        {
            isFacingRight = true;
            isFacingLeft = false;
            spriteRenderer.flipX = false;
        }
        else if(horizontalInput < 0)
        {
            isFacingRight = false;
            isFacingLeft = true;
            spriteRenderer.flipX = true;
        }
        if((currentState != State.Knockback) && (currentState !=  State.Dead) && (currentState != State.Respawning))
        {
            if ((Input.GetKeyDown("space")) || (Input.GetKeyDown(KeyCode.X)) || (Input.GetButtonDown("Jump")))
            {
                rememberJumpPress = jumpRememberTime;
            }
            if ((Input.GetKeyUp("space")) || (Input.GetKeyUp(KeyCode.X)) || (Input.GetButtonUp("Jump")))
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * cutJumpHeight);
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire1"))
            {
                if ((shootCounter > 0) && (shootTimer < 0))
                {
                    shootCounterTimer = shootCounterTimerMax;
                    shootCounter -= 1;
                    WeaponFire();
                }
                
            }
        }
        

    }

    void RespawnPlayer()
    {
        gameOverTimer -= Time.deltaTime;
        if(gameOverTimer < 0 )
        {
            blackoutUI.SetActive(false);
            playerHealthCurrent = playerHealthMax;
            transform.position = currentCheckpoint.transform.position;
            currentState = State.Normal;
        }
        else
        {
            blackoutUI.SetActive(true);
            //currentState = State.Respawning;
        }
        

        //Add some screen black fade etc. some delay

        //Give ammo back here as well, if added

    }

    public void IncreaseLives(int amount)
    {
        if (currentState != State.Respawning)
        {
            playerLivesCurrent += amount;
            CreateParticles(4);
            soundManager.PlaySound(SoundManager.Sound.playerPickup, 1f);
        }
    }

    public void RemoveHealth(int damage)
    {

        if(!isInvincible && currentState != State.Respawning)
        {
            if (playerHealthCurrent > 0)
            {
                int tempValue = playerHealthCurrent;
                tempValue -= damage;
                if (tempValue <= 0)
                {
                    //Dead, remove a life
                    playerHealthCurrent = 0;
                    if(playerLivesCurrent > 0)
                    {
                        playerLivesCurrent -= 1;
                        gameOverTimer = gameOverTimerMax;
                        currentState = State.Respawning;
                        
                    }
                    else
                    {
                        currentState = State.Dead;
                    }
                   // 
                }
                else
                {
                    playerHealthCurrent = tempValue;
                    CreateParticles(0);
                    rb2d.velocity = Vector2.zero;
                    currentState = State.Knockback;
                    knockbackTime = knockbackTimeMax;
                    soundManager.PlaySound(SoundManager.Sound.playerTakeDamage, 1f);
                }
            }
            else
            {
                //Should never be here, but incase, do same as when taking damage
                if (playerLivesCurrent > 0)
                {
                    playerLivesCurrent -= 1;
                    RespawnPlayer();
                }
                else
                {
                    currentState = State.Dead;
                }
            }
        }
        
    }

    public void IncreaseHealth(int healthUp)
    {
        if(currentState != State.Respawning)
        {
            if(playerHealthCurrent < playerHealthMax)
            {
                int tempValue = playerHealthCurrent;
                tempValue += healthUp;
                if(tempValue > playerHealthMax)
                {
                    //Max out HP
                    CreateParticles(4);
                    soundManager.PlaySound(SoundManager.Sound.playerPickup, 1f);
                    playerHealthCurrent = playerHealthMax;
                }
                else
                {
                    //Heal
                    CreateParticles(4);
                    soundManager.PlaySound(SoundManager.Sound.playerPickup,1f);
                    playerHealthCurrent = tempValue;
                }
            }
            else
            {
                //Already have max health
            }
        }
    }

    void WeaponFire()
    {
        //Check weaponMode to see what to shoot
        int shootDir = 1;
        if(isFacingLeft)
            shootDir = -1;

        switch (weaponMode)
        {
           
            case 0:
                {
                    GameObject bullet = Instantiate(bulletList[weaponMode]);
                    bullet.transform.parent = this.transform;
                    bullet.GetComponent<BulletScript>().Shoot(shootDir);
                    bullet.transform.position = new Vector3(transform.position.x + 2f * shootDir, transform.position.y + 0.25f, 0f);
                    soundManager.PlaySound(SoundManager.Sound.playerShoot,0.3f);
                    if(currentState == State.Climbing)
                    {
                        playerAnimator.Play("Player_ClimbShoot");
                        animationDelayTime = animationDelayTimeMax;
                    }
                    else if(currentState == State.Normal && (horizontalInput != 0) && isGrounded)
                    {
                        playerAnimator.Play("Player_RunShoot");
                        animationDelayTime = animationDelayTimeMax;
                    }
                    else if(currentState == State.Falling || (currentState == State.Normal && !isGrounded))
                    {
                        playerAnimator.Play("Player_FallShoot");
                        animationDelayTime = animationDelayTimeMax;
                    }
                    else
                    {
                        playerAnimator.Play("Player_Shoot");
                        animationDelayTime = animationDelayTimeMax;
                    }
                    
                    if (shootDir == 1)
                        CreateParticles(2);
                    else
                        CreateParticles(3);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void CreateParticles(int i)
    {
        switch (i)
        {
            case 0:
                hurtParticles.Play();
                break;
            case 1:
                runningParticles.Play();
                break;
            case 2:
                shootingParticlesRight.Play();
                break;
            case 3:
                shootingParticlesLeft.Play();
                break;
            case 4:
                healParticles.Play();
                break;
            default:
                break;
        }
    }

    public void PushPlayer(int dir) //Dir is the direction. 0 = left, 1 = right, 2 = up, 3 = down
    {
        switch (dir)
        {
            case 0:
                pushTimer = pushTimerMax;
                currentState = State.Normal;
                rb2d.velocity = new Vector2(rb2d.velocity.x - 0.25f, rb2d.velocity.y + 0.1f);
                break;
            case 1:
                pushTimer = pushTimerMax;
                currentState = State.Normal;
                rb2d.velocity = new Vector2(rb2d.velocity.x + 0.25f, rb2d.velocity.y+ 0.1f);
                break;
            case 2:
                pushTimer = pushTimerMax;
                currentState = State.Normal;
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y + 5f);
                break;
            case 3:
                pushTimer = pushTimerMax;
                currentState = State.Normal;
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y - 5f);
                break;
        }
       
    }

    bool IsGrounded()
    {
        //Check if the player is grounded
        Vector3 boxSize = new Vector3(boxCollider2d.bounds.size.x - groundCheckWidthDifference, boxCollider2d.bounds.size.y, boxCollider2d.bounds.size.z);
        RaycastHit2D raycastHit;
        if((currentState != State.Climbing)) //&& !isOnLadder)
        {
            raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayersWithLadder);
        }
        else
        {
            raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayers);
        }
        
        Color rayColor;
        if(raycastHit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;
        
        //Debug ray drawing
       // Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
     //   Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
       // Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(0, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 8)
        {
          //  CreateParticles(1);
        }

        if (collision.otherRigidbody != null && collision.gameObject.GetComponent<EnemyCollisionChecker>()) //Enemy collision damage.
        {
            EnemyCollisionChecker enemy = collision.gameObject.GetComponent<EnemyCollisionChecker>();
            enemy.enemyController.TouchedPlayer();

            if(enemy.transform.position.x > transform.position.x)//Enemy on the right side
            {
                knockbackDirection = -1;
            }
            else //enemy on the left side
            {
                knockbackDirection = 1;
            }

            RemoveHealth(enemy.GetDamage());
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<EnemyBulletScript>())
        {
            EnemyBulletScript enemyBullet = other.attachedRigidbody.GetComponent<EnemyBulletScript>();

            if (enemyBullet.transform.position.x > transform.position.x)//Bullet on the right side
            {
                knockbackDirection = -1;
            }
            else //enemy on the left side
            {
                knockbackDirection = 1;
            }

            RemoveHealth(enemyBullet.damage);
            if(enemyBullet.bulletType != EnemyBulletScript.BulletType.fire)
            {
                Destroy(other.transform.gameObject);
            }
            
        }
        if (other.attachedRigidbody != null && other.gameObject.GetComponent<EnemyCollisionChecker>()) //Enemy collision damage.
        {
            EnemyCollisionChecker enemy = other.gameObject.GetComponent<EnemyCollisionChecker>();
            enemy.enemyController.TouchedPlayer();

            if (enemy.transform.position.x > transform.position.x)//Enemy on the right side
            {
                knockbackDirection = -1;
            }
            else //enemy on the left side
            {
                knockbackDirection = 1;
            }

            RemoveHealth(enemy.GetDamage());
        }
    }

    void SlopeCheck()
    {
        Vector2 checkPosition = transform.position - new Vector3(0.0f, boxCollider2d.bounds.extents.y);

        SlopeCheckHorizontal(checkPosition);
        SlopeCheckVertical(checkPosition);
    }
    void SlopeCheckHorizontal(Vector2 checkPosition)
    {
        RaycastHit2D raycastHitFront = Physics2D.Raycast(checkPosition, transform.right, slopeCheckDistance, groundLayers);
        RaycastHit2D raycastHitBack = Physics2D.Raycast(checkPosition, -transform.right, slopeCheckDistance, groundLayers);

        if(raycastHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(raycastHitFront.normal, Vector2.up);
        }
        else if(raycastHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(raycastHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0f;
            isOnSlope = false;
        }
    }
    void SlopeCheckVertical(Vector2 checkPosition)
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(checkPosition, Vector2.down, boxCollider2d.bounds.extents.y + slopeCheckDistance, groundLayers);

        if(raycastHit)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(raycastHit.normal).normalized;
            
            //Returns the angle between Y-Axis and the raycastHit normal 
            //=> This is also the angle between X-Axis and the slope
            //Points down the slope
            slopeAngle = Vector2.Angle(raycastHit.normal, Vector2.up);

            if(slopeAngle != slopeAngleOld)
            {
                isOnSlope = true;
            }

            slopeAngleOld = slopeAngle;

            Debug.DrawRay(raycastHit.point, slopeNormalPerpendicular, Color.red);
            Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green);
        }

        if(isOnSlope && horizontalInput == 0f)
        {
            rb2d.sharedMaterial = fullFriction;
        }
        else
        {
            rb2d.sharedMaterial = noFriction;
        }
    }

    void KnockbackPlayer(int direction)
    {
        playerAnimator.Play("Player_Hit");
        isInvincible = true;
        invincibilityTime = invincibilityTimeMax;
        if (knockbackTime < 0)
        {
            knockbackDirection = 0;
            currentState = State.Normal;
        }
        //AddForce for small upward bounce,-> NOT USED CURRENTLY, SLOWS FALL TOO!
        //velocity change for rest of knockback
        //rb2d.AddForce(new Vector2(0, 345f));
        rb2d.velocity = new Vector2(3f * direction, rb2d.velocity.y-1f);

        //AddForce for small upward bounce,-> NOT USED CURRENTLY, SLOWS FALL TOO!
        //velocity change for rest of knockback
        //rb2d.AddForce(new Vector2(0,45f));
        // rb2d.velocity = new Vector2(3f, rb2d.velocity.y);
    }

    void InvincibilityFrames()
    {
        if(invincibilityTime > 0)
        {
            Physics2D.IgnoreLayerCollision(13, 12, true);
            invincibilityLight.SetActive(true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(13, 12, false);
            invincibilityLight.SetActive(false);
            isInvincible = false;
        }
    }

    void ClimbPlayer()
    {
        
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        CheckLadders();

        //TODO: Snap player to the middle of ladder

        rb2d.gravityScale = 0;
        if(animationDelayTime < 0)
        {
            rb2d.velocity = new Vector2(0, climbSpeed * verticalInput);
        }
        else
        {
            rb2d.velocity = new Vector2(0, 0);
        }
       

        if(verticalInput != 0 && (animationDelayTime < 0))
        {
            playerAnimator.Play("Player_Climb");
        }
        else if(verticalInput == 0 && (animationDelayTime < 0))
        {
            playerAnimator.Play("Player_Climb_Idle");
        }

        if ((Input.GetKey("space")) || (Input.GetKey(KeyCode.X)) || (Input.GetButton("Jump")))
        {
            currentState = State.Falling;
            playerAnimator.Play("Player_Fall");
            rememberJumpPress = 0;
        }
        if(isGrounded)
        {
            currentState = State.Normal;
            playerAnimator.Play("Player_Idle");
        }
    }


    void CheckLadders()
    {
        Vector3 boxSize = new Vector3(boxCollider2d.bounds.size.x - 0.01f, boxCollider2d.bounds.size.y, boxCollider2d.bounds.size.z);

        RaycastHit2D hitInfoUp = Physics2D.BoxCast(boxCollider2d.bounds.center, boxSize, 0f, Vector2.up, ladderCheckDistance, ladderLayer);
        RaycastHit2D hitInfoDown = Physics2D.BoxCast(boxCollider2d.bounds.center, boxSize, 0f,  Vector2.down, ladderCheckDistance, ladderLayer);

        //RaycastHit2D hitInfoUp = Physics2D.Raycast(boxCollider2d.bounds.center, Vector2.up, ladderCheckDistance, ladderLayer);
        //RaycastHit2D hitInfoDown = Physics2D.Raycast(boxCollider2d.bounds.center, Vector2.down, ladderCheckDistance, ladderLayer);

        Color rayColorUp;
        Color rayColorDown;

        if (hitInfoUp.collider != null) rayColorUp = Color.green;
        else rayColorUp = Color.red;

        if (hitInfoDown.collider != null) rayColorDown = Color.green;
        else rayColorDown = Color.red;

        //Debug.DrawRay(boxCollider2d.bounds.center, Vector2.down, rayColorDown);
       // Debug.DrawRay(boxCollider2d.bounds.center, Vector2.up, rayColorUp);

        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + ladderCheckDistance), rayColorDown);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + ladderCheckDistance), rayColorDown);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(0, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColorDown);

        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + ladderCheckDistance), rayColorUp);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + ladderCheckDistance), rayColorUp);
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(0, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColorUp);

        if (hitInfoUp.collider != null)
        {
            if(currentState != State.Climbing)
            {
                //Debug.Log("See ladder up!");
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                   // Debug.Log("UP CLIMB");
                    currentState = State.Climbing;
                    isGrounded = false;
                    this.transform.position = new Vector3(hitInfoUp.collider.transform.position.x, transform.position.y);
                    //rb2d.position += new Vector2(0, 1);
                }
            }
           
        }

        else if(hitInfoDown.collider != null)
        {
            if (currentState != State.Climbing)
            {
                //Debug.Log("See ladder down!");
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    //Debug.Log("DOWN CLIMB");
                    currentState = State.Climbing;
                    isGrounded = false;
                    this.transform.position = new Vector3(hitInfoDown.collider.transform.position.x, transform.position.y);
                    /*transform.position = new Vector3(Mathf.Round(transform.position.x),
                                  Mathf.Round(transform.position.y),
                                  Mathf.Round(transform.position.z));*/
                    //rb2d.position = new Vector2(0, -1);
                }
            }
           
        }

        else
        {
            currentState = State.Normal;
        }
        
    }
    void FallPlayer()
    {
        if(isGrounded)
        {
            currentState = State.Normal;
            //playerAnimator.Play("Player_Idle");
        }
        else
        {
            playerAnimator.Play("Player_Fall");
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb2d.gravityScale = defaultGravity;

            rb2d.velocity += new Vector2(horizontalInput * runSpeed * airControl * Time.deltaTime, 0);
            rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -runSpeed, runSpeed), rb2d.velocity.y);
        }
    }

    void MovePlayer()
    {
        
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb2d.gravityScale = defaultGravity;

        rememberJumpPress -= Time.deltaTime;
        rememberGrounded -= Time.deltaTime;

        CheckLadders();

        if(isGrounded)
        {
            rememberGrounded = groundedRememberTime;
        }

        if ((rememberJumpPress > 0) && (rememberGrounded > 0))
        {
            //Jumping
            //Small delay after not grounded makes ledge jumps better for the player
            //Jump can also be pressed a bit before landing for easier timed jumps
            rememberJumpPress = 0;
            rememberGrounded = 0;
            soundManager.PlaySound(SoundManager.Sound.playerJump,0.8f);
            playerAnimator.Play("Player_Jump");
            animationDelayTime = animationDelayTimeMax;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
        }

        if (isGrounded && !isOnSlope)
        {
            
            rb2d.velocity = new Vector2(runSpeed * horizontalInput, rb2d.velocity.y);
            if(horizontalInput == 0 && (animationDelayTime < 0))
            {
                playerAnimator.Play("Player_Idle");
            }
            else if(horizontalInput != 0 && (animationDelayTime < 0))
            {
                playerAnimator.Play("Player_Run");
            }
            

        }
        else if (isGrounded && isOnSlope)
        {
             rb2d.velocity = new Vector2(runSpeed * slopeNormalPerpendicular.x * -horizontalInput, runSpeed * slopeNormalPerpendicular.y * -horizontalInput);
           
        }
        else if (!isGrounded)
        {
            //Air control movement
            rb2d.velocity += new Vector2(horizontalInput * runSpeed * airControl * Time.deltaTime, 0);
            rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -runSpeed, runSpeed), rb2d.velocity.y);
            if(animationDelayTime < 0)
            {
                playerAnimator.Play("Player_Fall");
            }
        }
    }


}
