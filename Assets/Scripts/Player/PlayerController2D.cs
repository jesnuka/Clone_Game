using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    //--Component References
    Rigidbody2D rb2d;
    Animator playerAnimator;
    BoxCollider2D boxCollider2d;
    SpriteRenderer spriteRenderer;
    SoundManager soundManager;

    public Sprite healthBarBlock;
    public Sprite healthBarEmpty;

    public GameObject gameOverUI;

    public ParticleSystem runningParticles;
    public ParticleSystem hurtParticles;
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
    public int playerHealthMax;
    public int playerHealthCurrent;


    //--Variables
    float horizontalInput;
    float verticalInput;


    public float defaultGravity;
    public float runSpeed;
    public float airControl;
    public float jumpPower;
    public float climbSpeed;

    [Range(0.0f, 1.0f)]
    public float cutJumpHeight;

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

    public State currentState;

    public enum State
    {
        Dead,
        Climbing,
        Normal,
        Falling
    }
    private void Awake()
    {
        currentState = State.Normal;
        isFacingRight = true;
        rb2d = this.GetComponent<Rigidbody2D>();
        playerAnimator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
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
            
            else if(currentState == State.Falling)
            {
                FallPlayer();
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

        if ((Input.GetKeyDown("space")) || (Input.GetKeyDown(KeyCode.X)) || (Input.GetButtonDown("Jump")))
        {
            rememberJumpPress = jumpRememberTime;
        }
        if ((Input.GetKeyUp("space"))|| (Input.GetKeyUp(KeyCode.X)) || (Input.GetButtonUp("Jump")))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * cutJumpHeight);
        }

        if(Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire1"))
        {
            WeaponFire();
        }

    }

    public void RemoveHealth(int damage)
    {
        if (playerHealthCurrent > 0)
        {
            int tempValue = playerHealthCurrent;
            tempValue -= damage;
            if (tempValue <= 0)
            {
                //Game over!
                playerHealthCurrent = 0;
                currentState = State.Dead;
            }
            else
            {
                playerHealthCurrent = tempValue;
                CreateParticles(0);
            }
        }
        else
        {
            //Game over!
            GameOver();
        }
    }

    public void IncreaseHealth(int healthUp)
    {
        if(playerHealthCurrent < playerHealthMax)
        {
            int tempValue = playerHealthCurrent;
            tempValue += healthUp;
            if(tempValue > playerHealthMax)
            {
                playerHealthCurrent = playerHealthMax;
            }
            else
            {
                playerHealthCurrent = tempValue;
            }
        }
        else
        {
            //Already have max health
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
                    bullet.transform.position = new Vector3(transform.position.x + 0.8f * shootDir, transform.position.y, 0f);
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
            default:
                break;
        }
    }

    bool IsGrounded()
    {
        //Check if the player is grounded
        Vector3 boxSize = new Vector3(boxCollider2d.bounds.size.x - groundCheckWidthDifference, boxCollider2d.bounds.size.y, boxCollider2d.bounds.size.z);
        RaycastHit2D raycastHit;
        if(currentState != State.Climbing)
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
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(0, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
          //  CreateParticles(1);
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

    void ClimbPlayer()
    {
       
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        CheckLadders();

        //TODO: Snap player to the middle of ladder

        rb2d.gravityScale = 0;
        rb2d.velocity = new Vector2(0, climbSpeed * verticalInput);

        if ((Input.GetKey("space")) || (Input.GetKey(KeyCode.X)) || (Input.GetButton("Jump")))
        {
            currentState = State.Falling;
            rememberJumpPress = 0;
        }
        if(isGrounded)
        {
            currentState = State.Normal;
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
        }
        else
        {
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
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
        }

        if (isGrounded && !isOnSlope)
        {
            
            rb2d.velocity = new Vector2(runSpeed * horizontalInput, rb2d.velocity.y);
            
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
        }
    }


}
