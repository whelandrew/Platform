using UnityEngine;


//parent for character controls (all character-related classes go through here)
//movement
//animations TODO - animations for sides of ground & walls, ceiling & bottom of ground
//stats TODO
public class Controller : MonoBehaviour
{
    private ShootController shootController;
    private SwingController swingController;
    public GameboardData gbData;

    //for tracking keys used in movement
    public KeyCode[] MappedKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };

    CapsuleCollider2D cCollider;
    Rigidbody2D rBody;

    public float maxVelocity = 4f;

    private bool isWalking = false;
    private bool isGrounded = false;
    private bool isFalling = false;

    private bool canControl = true;
    public bool CanControl
    {
        get { return canControl; }
        set { canControl = value; }
    }

    public bool IsFalling
    {
        get { return isFalling; }
        set { isFalling = value; }
    }

    private float inputTimer = 0f;
    public float inputTimerMax = 2f;

    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    private Vector2 direction = Vector2.zero;
    private Vector2 moveDir = Vector2.zero;
    private float moveSpeed = 7f;    

    private float elapsed = 0f;
    public float elapsedMax = 0.03f;

    //jump values
    private bool isJumping = false;
    private bool wantsToJump = false;
    public float timeToApex = 1f;
    public float jumpHeight = 5f;
    public float minJumpHeight = 1f;    
    private float initJumpVelocity = 0f;
    private float lastGrounded = 0;
    public float damp = .5f;
    public float fallSpeed = 10f;

    private Vector2 finalVelocity;

    private void Start()
    {
        cCollider = GetComponent<CapsuleCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        shootController = GetComponent<ShootController>();
        swingController = GetComponent<SwingController>();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        //is character on the ground
        if (IsGrounded)
        {
            //is player controllable (pause actions for cutscenes, etc)
            if (CanControl)
            {
                Debug.Log("Character can be controlled");
                Jump();

                if (isJumping)
                {
                    JumpCalc();
                }
                else
                {
                    if (CanMove())
                    {                        
                        Walking();
                    }                    
                }
                Fire();
            }
        }
        else
        {
            if(CanControl)
            {
                Fire();
            }

            //post jump
            JumpTermination();
            //EdgeJump();

            //fall to ground
            Fall();
        }        
    }

    private void FixedUpdate()
    {
        rBody.velocity = finalVelocity;
    }

    private void Fire()
    {        
        if (Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
        {
            //sController.FireWeapon();          
            swingController.Sword();
        }
    }

    private bool Walking()
    {
        Debug.Log("Moving");
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            //up
            direction = Vector2.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //down
            direction = Vector2.down;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //left
            direction = Vector2.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //right
            direction = Vector2.right;
        }

        moveDir = direction;
        //play movement animations

        finalVelocity = moveDir * moveSpeed;
        isWalking = true;

        return direction != Vector2.zero;
    }

    private void EdgeJump()
    {
        if (isGrounded)
        {
            lastGrounded = elapsed;
        }

        if (wantsToJump && (elapsed - lastGrounded < 1f))
        {
            Jump();
        }
    }
    private void Fall()
    {
        if (!isFalling && !isGrounded && !isJumping)
        {
            Debug.Log("Fall");

            float fallG = (2 * fallSpeed) / (timeToApex * timeToApex);
            finalVelocity = new Vector2(finalVelocity.x, -fallG);
            isFalling = true;

            //falling animation
        }
    }

    private void JumpCalc()
    {
        Debug.Log("JumpCalc");
        
        float X = rBody.velocity.x;
        float Y = rBody.velocity.y;

        //Clamping
        float v = Mathf.Sqrt((X*X) + (Y*Y));
        if(v > maxVelocity)
        {
            float vs = maxVelocity / v;
            Vector2 clamp = new Vector2(X * vs, Y * vs);
            rBody.velocity = clamp;
        }

        //Dampening
        Vector2 dampVelocity = new Vector2((X / (1 + damp * Time.deltaTime)), (Y / (1 + damp * Time.deltaTime)));
        rBody.velocity = dampVelocity;

        //-- what is the gravity that would allow jumping to a given height?
        float gravity = (2 * jumpHeight) / (timeToApex * timeToApex);

        //-- what is the initial jump velocity?
        initJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

        //-- how long does it take to reach the maximum height of a jump?
        //-- note: if "initJumpVelocity" is not a multiple of "g" the maximum height is reached between frames
        timeToApex = initJumpVelocity / gravity;

        if (isWalking)
        {
            finalVelocity = new Vector2(moveDir.x * moveSpeed, initJumpVelocity);
        }
        else
        {
            finalVelocity = new Vector2(0, initJumpVelocity);
        }
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            Debug.Log("Jumping");
            isJumping = true;
            elapsed = 0;
            lastGrounded = 0;

            //if lastJump and elapsed - lastJump< 0.2
            float lastJump = (elapsed + Time.deltaTime);
            //if ((lastJump * elapsed) - lastJump < .2f)
            if((lastJump < .2f) && (elapsed < .2f))
            {
                wantsToJump = true;
            }            

            //jump animation            
        }
    }

    private void JumpTermination()
    {
        if (isJumping)
        {
            //is timeToApex breached          
            if (elapsed >= timeToApex)
            {
                isJumping = false;
                isFalling = false;
            }

            //--what is the velocity required to terminate a jump ?
            //--note : only works when "g" is negative
            //termVelocity = math.sqrt(initJumpVelocity^2 + 2*g*(jumpHeight - minJumpHeight))
            float termVelocity = Mathf.Sqrt((initJumpVelocity * initJumpVelocity) + 2 * -gbData.gravity * (jumpHeight - minJumpHeight));

            //-- how much time is available until a jump can no longer be terminated ?
            //--note : "minJumpHeight" must be greater than 0
            //termTime = timeToApex - (2 * (jumpHeight - minJumpHeight) / (initJumpVelocity + termVelocity))

            //if releasedJumpKey then
            if (!Input.GetKey(KeyCode.Space))
            {
                //-- is the player ascending fast enough to allow jump termination
                if (rBody.velocity.y > termVelocity)
                {
                    Vector2 newVelocity = new Vector2(rBody.velocity.x, termVelocity);
                    rBody.velocity = newVelocity;
                    isJumping = false;
                    isFalling = false;
                }
            }
        }
    }

    //checks if there has been user input, or pauses movement
    private bool CanMove()
    {
        inputTimer += 1f * Time.deltaTime;
        
        if (!Input.anyKey)
        {            
            //if no input after i seconds
            if (inputTimer > inputTimerMax)
            {
                //stop in place
                finalVelocity = Vector2.zero;
                isWalking = false;
                return false;
            }
        }
        else
        {
            //detecting key input by mapped movementkeys
            for (int i = 0; i < MappedKeys.Length; i++)
            {
                if (Input.GetKey(MappedKeys[i]))
                {
                    //reset timer if there is player input
                    inputTimer = 0;
                    isWalking = true;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //bullet collision
        if (collision.gameObject.layer == 13)
        {
            Debug.Log("Controller OnTriggerEnter2D");
            BulletData bData = collision.gameObject.GetComponent<BulletData>();
            if (bData.shooter == this.gameObject.layer)
            {
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //floor collision
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("Controller OnTriggerEnter2D");
            //cancel jumping when colliding with floor layer
            if (isJumping)
            {
                isJumping = false;
                fallSpeed = 2f;

                //sliding on the wall animation
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //change fallSpeed if off ground
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("Collision OnTriggerExit2D");
            fallSpeed = 6f;
        }        
    }
}