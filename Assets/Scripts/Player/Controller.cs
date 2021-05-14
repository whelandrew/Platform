using UnityEngine;


//parent for character controls (all character-related classes go through here)
//movement
//animations
//stats
public class Controller : MonoBehaviour
{ 
    //for tracking keys used in movement
    public KeyCode[] MappedKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };

    CapsuleCollider2D cCollider;
    Rigidbody2D rBody;

    public float maxVelocity = 4f;

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
    public float gravity = 9.807f;    
    private float initJumpVelocity = 0f;
    private float lastGrounded = 0;
    public float damp = .5f;

    private Vector2 finalVelocity;

    private void Start()
    {
        cCollider = GetComponent<CapsuleCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        elapsed = elapsed + Time.deltaTime;
        wantsToJump = false;
        //is player controllable (pause actions for cutscenes, etc)
        if (CanControl)
        {
            //is character on the ground
            if (IsGrounded)
            {
                Jump();                            
                //get jump input
                if (isJumping)
                {
                    JumpCalc();                    
                }
                else
                {
                    //get move input
                    Walking();
                }
            }
            else
            {
                if (isJumping)
                {
                    JumpTermination();
                    EdgeJump();     
                }
            }

            CanMove();
        }
        
        Fall();
    }

    private void FixedUpdate()
    {
        rBody.velocity = finalVelocity;
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

        return direction != Vector2.zero;
    }

    private void EdgeJump()
    {
        elapsed = 0;
        lastGrounded = 0;

        if (isGrounded)
        {
            lastGrounded = elapsed;
        } 

        if (wantsToJump && (elapsed - lastGrounded < 0.1))
        {
            Jump();
        }
    }
    private void Fall()
    {
        if (!isGrounded && !isJumping && !isFalling)
        {
            Debug.Log("Fall");

            float fallG = (2 * jumpHeight) / (timeToApex * timeToApex);
            finalVelocity = new Vector2(finalVelocity.x, -fallG);
            isFalling = true;            
        }
    }

    private void JumpCalc()
    {
        Debug.Log("JumpCalc");
        //Clamping
        float v = Mathf.Sqrt((rBody.velocity.x * rBody.velocity.x) + (rBody.velocity.y * rBody.velocity.y));
        if(v > maxVelocity)
        {
            float vs = maxVelocity / v;
            Vector2 clamp = new Vector2(rBody.velocity.x * vs, rBody.velocity.y * vs);
            rBody.velocity = clamp;
        }

        //Dampening
        Vector2 dampVelocity = new Vector2((rBody.velocity.x / (1 + damp * Time.deltaTime)), (rBody.velocity.y / (1 + damp * Time.deltaTime)));
        rBody.velocity = dampVelocity;

        //-- what is the gravity that would allow jumping to a given height?
        gravity = (2 * jumpHeight) / (timeToApex * timeToApex);

        //-- what is the initial jump velocity?
        initJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

        //-- how long does it take to reach the maximum height of a jump?
        //-- note: if "initJumpVelocity" is not a multiple of "g" the maximum height is reached between frames
        timeToApex = initJumpVelocity / gravity;

        finalVelocity = new Vector2(moveDir.x * moveSpeed, initJumpVelocity);
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Jumping");
            isJumping = true;
            isGrounded = false;
            elapsed = 0;

            //if lastJump and elapsed - lastJump< 0.2
            float lastJump = (elapsed + Time.deltaTime);
            if ((lastJump * elapsed) - lastJump < .2f)
            {
                wantsToJump = true;
            }            
        }
    }

    private void JumpTermination()
    {
        //is timeToApex breached
        if(elapsed >= timeToApex || !Input.GetKey(KeyCode.Space))
        {
            isJumping = false;
            isFalling = false;
        }

        //--what is the velocity required to terminate a jump ?
        //--note : only works when "g" is negative
        float termVelocity = Mathf.Sqrt((initJumpVelocity * initJumpVelocity) + 2 * gravity * (jumpHeight - minJumpHeight));        


        //-- how much time is available until a jump can no longer be terminated ?
        //--note : "minJumpHeight" must be greater than 0
        //termTime = timeToApex - (2 * (jumpHeight - minJumpHeight) / (initJumpVelocity + termVelocity))

        //if releasedJumpKey then
        if (!Input.GetKey(KeyCode.Space))
        {
            //-- is the player ascending fast enough to allow jump termination
            if(rBody.velocity.y > termVelocity)
            {
                Vector2 newVelocity = new Vector2(rBody.velocity.x, termVelocity);
                rBody.velocity = newVelocity;
            }
        }
    }

    //checks if there has been user input, or pauses movement
    private void CanMove()
    {
        inputTimer += 1f * Time.deltaTime;
        
        if (!Input.anyKey && !isFalling)
        {            
            //if no input after i seconds
            if (inputTimer > inputTimerMax)
            {
                //stop in place
                finalVelocity = Vector2.zero;
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
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}