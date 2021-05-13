using UnityEngine;


//parent for character controls (all character-related classes go through here)
//movement
//animations
//stats
public class Controller : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    private Vector2 position = Vector2.zero;

    public float maxVelocity = 5f;
    public float dampening = 0f;

    //for tracking keys used in movement
    public KeyCode[] MappedKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };

    CapsuleCollider2D cCollider;
    Rigidbody2D rBody;

    private bool canMove;

    private bool isGrounded;    
    private bool isFalling;

    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    private Vector2 direction = Vector2.zero;
    private Vector2 moveDir = Vector2.zero;
    private float moveSpeed = 7f;

    private float inputTimer = 0;
    public float inputTimeMax = 0.03f;

    private float fallSpeed = 10f;


    //jump values
    private bool isJumping = false;
    private bool wantsToJump = false;
    public float timeToApex = 1f;
    public float jumpHeight = 5f;
    public float minJumpHeight = 1f;
    public float gravity = 9.807f;
    private float elapsed = 0f;
    private float initJumpVelocity = 0f;
    private float lastGrounded = 0;
    public float damp = .5f;

    private void Start()
    {
        cCollider = GetComponent<CapsuleCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        elapsed = elapsed + Time.deltaTime;
        wantsToJump = false;

        //is player controllable
        if (CanMove())
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
                JumpTermination();
                canMove = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsGrounded && !isJumping)
        {
            //character is falling
            Fall();
        }
        else
        {
            if (CanMove())
            {
                if (isJumping)
                {
                    //if pressedJumpKey then
                    rBody.velocity = new Vector2(transform.position.x, initJumpVelocity);
                }
                else
                {
                    //rBody.velocity = moveDir * moveSpeed;
                    if (isFalling)
                    {
                        EdgeJump();
                    }
                }
            }
            else
            {
                rBody.velocity = Vector2.zero;
            }
        }
    }

    private void Reset()
    {
        elapsed = 0;
        wantsToJump = false;
        canMove = false;
        isJumping = false;       
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
        Debug.Log("Fall");
        transform.Translate((Vector2.down * fallSpeed) * Time.deltaTime);
        isFalling = true;
        Reset();
    }

    private void JumpCalc()
    {
        Debug.Log("JumpCalc");
        //clamping
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
        //g = (2*jumpHeight)/(timeToApex^2)   
        gravity = (2 * jumpHeight) / (timeToApex * timeToApex);

        //-- what is the initial jump velocity? 
        //initJumpVelocity = math.sqrt(2*g* jumpHeight)
        initJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

        //-- how long does it take to reach the maximum height of a jump?
        //-- note: if "initJumpVelocity" is not a multiple of "g" the maximum height is reached between frames
        //timeToApex = initJumpVelocity / g
        timeToApex = initJumpVelocity / gravity;        
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Jumping");
            isJumping = true;           

            //if lastJump and elapsed - lastJump< 0.2
                //wantsToJump = true
            float lastJump = (elapsed + Time.deltaTime);
            if ((lastJump * elapsed) - lastJump < .2f)
            {
                wantsToJump = true;
            }            
        }
    }

    private void JumpTermination()
    {
        //timeToApex breached
        if(elapsed >= timeToApex)
        {
            isJumping = false;
        }

        //--what is the velocity required to terminate a jump ?
        //--note : only works when "g" is negative
        //termVelocity = math.sqrt(initJumpVelocity ^ 2 + 2 * g * (jumpHeight - minJumpHeight))
        float termVelocity = Mathf.Sqrt((initJumpVelocity * initJumpVelocity) + 2 * gravity * (jumpHeight - minJumpHeight));        


        //-- how much time is available until a jump can no longer be terminated ?
        //--note : "minJumpHeight" must be greater than 0
        //termTime = timeToApex - (2 * (jumpHeight - minJumpHeight) / (initJumpVelocity + termVelocity))

        //if releasedJumpKey then
        if (!Input.GetKey(KeyCode.Space))
        {
            //-- is the player ascending fast enough to allow jump termination ?
            //if player.yv > termVelocity then
            //player.yv = termVelocity
            if(rBody.velocity.y > termVelocity)
            {
                Vector2 newVelocity = new Vector2(rBody.velocity.x, termVelocity);
                rBody.velocity = newVelocity;
            }
        }
    }

    //checks if there has been user input, or pauses movement
    private bool CanMove()
    {
        inputTimer += 1 * Time.deltaTime;

        if (!Input.anyKey)
        {
            Reset();
            //if no input after i seconds
            if (inputTimer > inputTimeMax)
            {
                //if character is not on the ground
                canMove = IsGrounded;

                //if no input has been detected
                Reset();
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
                    canMove = true;
                }
            }
        }

        return canMove;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}