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
    public ParticleSystem playerParticles;
    public GameObject playerParticleObject;
    [SerializeField]
    PhysicsMaterial2D fullFriction;
    [SerializeField]
    PhysicsMaterial2D noFriction;

    //--Weapon Related
    public GameObject[] bulletList;
    public int weaponMode;


    //--Variables
    float horizontalInput;

    public float runSpeed;
    public float airControl;
    public float jumpPower;

    [Range(0.0f, 1.0f)]
    public float cutJumpHeight;

    public float jumpRememberTime;
    public float groundedRememberTime;
    float rememberJumpPress;
    float rememberGrounded;


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
    public LayerMask groundLayers;
    

    private void Awake()
    {
        isFacingRight = true;
        rb2d = this.GetComponent<Rigidbody2D>();
        playerAnimator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        boxCollider2d = this.GetComponent<BoxCollider2D>();
        if (playerParticles == null)
        {
            playerParticles = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        }
        if (playerParticleObject == null)
        {
            playerParticleObject = this.transform.GetChild(1).gameObject;
        }
       /* if (soundManager == null)
        {
            soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }*/

    }
    void Update()
    {
        CheckInput();
    }
    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        SlopeCheck();
        MovePlayer();
    }

    void CheckInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if(horizontalInput > 0)
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
            case 1:
                playerParticles.Play();
                break;
            default:
                break;
        }
    }

    bool IsGrounded()
    {
        //Check if the player is grounded
        Vector3 boxSize = new Vector3(boxCollider2d.bounds.size.x - groundCheckWidthDifference, boxCollider2d.bounds.size.y, boxCollider2d.bounds.size.z);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayers);
        Color rayColor;
        if(raycastHit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;
        
        //Debug ray drawing
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x - groundCheckWidthDifference, 0), Vector2.down * (boxCollider2d.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(0, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
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

    void MovePlayer()
    {

        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        rememberJumpPress -= Time.deltaTime;
        rememberGrounded -= Time.deltaTime;
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
