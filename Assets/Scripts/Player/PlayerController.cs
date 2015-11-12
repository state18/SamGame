using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float maxSpeed;                      //multiplier of how fast player moves
    bool facingRight = true;

    Animator anim;                              //will hold player's animator component
    PlatformData currentPlatform;
    ConveyorPlatform currentConveyor;
    //int platformCount = 0;

    bool grounded = false;                      //is the player on the ground?
    public bool onMovingPlatform = false;
    public Vector2 platformVelocity;            // x and y component of platform
    public float conveyerVelocity;              // magnitude of x component of conveyer velocity (conveyer belts only go left/right)

    public float yOffset;

    public Transform groundCheck;               //casts a circle to check if player is grounded
    public Transform firePoint;                 //where do projectiles fire from?

    public float groundRadius = 0.08f;          //radius of circle cast from groundCheck
    public LayerMask whatIsGround;              //where is the ground?
    public LayerMask whatIsClimbDown;           //where can we climb down?
    public LayerMask whatIsMovingPlatform;
    public LayerMask whatIsConveyer;
    public float jumpForce = 100f;              //force applied upon jumping
    private bool isJumping;                     //are we in the middle of a jump?
                                                //int platformEntryPoints = 0;


    //MOVEMENT

    private float horizontalInput;              //horizontal axis value held here
    private float verticalInput;                //vertical axis value held here
    private bool jumpInput;                     //is space being pressed?
    private bool jumpInputDown;                 //is space button being pressed down?
    public bool isWalkingHorizontally;



    public float knockback;                     //how hard is the knockback?
    public float knockbackLength;               //how long is the knockback? (sent to hurt player script)
    public float knockbackCounter;              //counts down from knockback length
    public bool knockFromRight;                 //this is used to reverse the knockback depending on direction the player is facing

    public bool isClimbing;                     //is the player climbing?
    public bool CanClimb { get; set; }          //is the player in a climbing zone?
    public float originalGravity;               //normal gravity

    public bool canMove;                        //is movement possible?

    public LevelManager levelManager;
    private Rigidbody2D initialrb;              //cache of the Rigidbody2D of the player
    public AudioSource jumpSound;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        anim = GetComponent<Animator>();
        initialrb = GetComponent<Rigidbody2D>();
        originalGravity = initialrb.gravityScale;
        canMove = true;
        isJumping = false;

    }

    // Update is called once per frame
    void Update() {

        if (canMove) {
            // jumpInput = Input.GetButton ("Jump");
            jumpInputDown = Input.GetButtonDown("Jump");

            if (grounded && jumpInputDown) {
                anim.SetBool("Ground", false);
                isJumping = true;
                //initialrb.AddForce (new Vector2 (0, jumpForce));
                //jumpSound.Play ();
            }

            // sloppy way to respawn player by pressing r key
            if (Input.GetButtonDown("Respawn")) {
                levelManager.RespawnPlayer();
            }
        }
    }

    void FixedUpdate() {
        rb = GetComponent<Rigidbody2D>();

        // If player is on the ground, returns true
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        // isJumping = false;

        // onMovingPlatform = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsMovingPlatform);


        RaycastHit2D platformHit = Physics2D.BoxCast(groundCheck.position, new Vector2(.6f, .1f), 0f, Vector2.down, groundRadius - .8f, whatIsMovingPlatform);
        RaycastHit2D conveyerHit = Physics2D.BoxCast(groundCheck.position, new Vector2(.6f, .1f), 0f, Vector2.down, groundRadius - .8f, whatIsConveyer);

        // Are we on a moving platform? If so, obtain its data and use it below
        if (platformHit) {
            Debug.Log("On platform");
            currentPlatform = platformHit.transform.GetComponent<PlatformData>();
        } else {
            currentPlatform = null;
        }

        // Are we on a conveyer belt? If so, obtain its data and use it below
        if (conveyerHit) {
            Debug.Log("On conveyer belt");
            currentConveyor = conveyerHit.transform.GetComponent<ConveyorPlatform>();
        } else {
            currentConveyor = null;
        }

        anim.SetBool("Ground", grounded);


        if (canMove) {
            //left/right movement received from keyboard/joystick
            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

            anim.SetFloat("playerSpeed", Mathf.Abs(horizontalMovement));

            isWalkingHorizontally = (horizontalMovement != 0) ? true : false;

            // left/right movement applied to the rigidbody of the player. Knockback velocity applied here
            if (knockbackCounter <= 0) {

                // Climbing state handled.
                if (((CanClimb && (verticalMovement != 0)) || isClimbing)) {


                    anim.SetBool("isClimbing", true);

                    rb.gravityScale = 0f;
                    if (verticalMovement != 0f || horizontalMovement != 0f) {
                        anim.speed = 1f;
                    } else {
                        anim.speed = 0f;
                    }
                    rb.velocity = new Vector2(horizontalMovement * maxSpeed / 2f, verticalMovement * maxSpeed / 2f);
                    isClimbing = true;

                } else {

                    // Default state handled
                    anim.SetBool("isClimbing", false);
                    rb.gravityScale = originalGravity;
                    rb.velocity = new Vector2(horizontalMovement * maxSpeed, rb.velocity.y);

                    // Are we on a moving platform?
                    if (currentPlatform != null && !currentPlatform.playerFreed) {
                        Debug.Log("on moving platform");
                        platformVelocity = currentPlatform.GetVelocity();
                        Debug.Log(platformVelocity);
                        rb.velocity = new Vector2(horizontalMovement * maxSpeed, 0f);
                        rb.velocity += platformVelocity;
                        //rb.gravityScale = 0;

                        // Are we on a conveyor belt?
                    } else if (currentConveyor != null) {
                        Debug.Log("on conveyer belt");
                        conveyerVelocity = currentConveyor.GetVelocity();
                        Debug.Log("hi");
                        Debug.Log(" " + conveyerVelocity);
                        rb.velocity += new Vector2(conveyerVelocity, 0f);
                    }

                    // Jumping handled. if on a moving platform, ExitPlatform coroutine begins. the player is freed
                    if (isJumping) {
                        rb.velocity = new Vector2(horizontalMovement * maxSpeed, 0f);
                        rb.AddForce(new Vector2(0, jumpForce));
                        jumpSound.Play();
                        isJumping = false;
                        if (currentPlatform != null) {
                            Debug.Log("starting coroutine");
                            StartCoroutine("ExitPlatform");
                        }
                    }

                }

                // Knockback state handled	
            } else {

                if (knockFromRight)
                    rb.velocity = new Vector2(-knockback, knockback);
                if (!knockFromRight)
                    rb.velocity = new Vector2(knockback, knockback);

                knockbackCounter -= Time.deltaTime;
            }
            if (!isClimbing) {
                if (horizontalMovement > 0 && !facingRight)
                    Flip();
                else if (horizontalMovement < 0 && facingRight)
                    Flip();
            }

        }

    }

    // Flips the player along the y axis by making the x component negative
    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Upon jumping, the platform that was jumped from stops affecting the player's velocity and frees it for a number of seconds TODO Clean this up
    IEnumerator ExitPlatform() {
        var platformToExit = currentPlatform;
        currentPlatform.FreePlayer(true);
        Debug.Log("player freed");
        yield return new WaitForSeconds(.3f);
        Debug.Log("freedom expired");
        platformToExit.FreePlayer(false);
    }


}