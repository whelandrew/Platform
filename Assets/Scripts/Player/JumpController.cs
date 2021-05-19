using UnityEngine;

public class JumpController : MonoBehaviour
{
    public GameboardData gbData;
    Rigidbody2D rBody;
    Controller characterController;

    public float maxVelocity = 4f;
    private bool wantsToJump = false;    
    public float jumpHeight = 5f;
    public float minJumpHeight = 1f;
    private float initJumpVelocity = 0f;
    private float lastGrounded = 0;
    public float damp = .5f;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        characterController = GetComponent<Controller>();
    }
    public void JumpCalc()
    {
        Debug.Log("JumpCalc");

        float X = rBody.velocity.x;
        float Y = rBody.velocity.y;

        //Clamping
        float v = Mathf.Sqrt((X * X) + (Y * Y));
        if (v > maxVelocity)
        {
            float vs = maxVelocity / v;
            Vector2 clamp = new Vector2(X * vs, Y * vs);
            rBody.velocity = clamp;
        }

        //Dampening
        Vector2 dampVelocity = new Vector2((X / (1 + damp * Time.deltaTime)), (Y / (1 + damp * Time.deltaTime)));
        rBody.velocity = dampVelocity;

        //-- what is the gravity that would allow jumping to a given height?
        float gravity = (2 * jumpHeight) / (gbData.timeToApex * gbData.timeToApex);

        //-- what is the initial jump velocity?
        initJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

        //-- how long does it take to reach the maximum height of a jump?
        //-- note: if "initJumpVelocity" is not a multiple of "g" the maximum height is reached between frames
        gbData.timeToApex = initJumpVelocity / gravity;

        if (characterController.isWalking)
        {
            characterController.finalVelocity = new Vector2(characterController.moveDir.x * characterController.moveSpeed, initJumpVelocity);
        }
        else
        {
            characterController.finalVelocity = new Vector2(0, initJumpVelocity);
        }
    }

    public void InitJump()
    {
        Debug.Log("Jumping");
        characterController.isJumping = true;
        gbData.elapsed = 0;
        lastGrounded = 0;

        //if lastJump and elapsed - lastJump< 0.2
        float lastJump = (gbData.elapsed + Time.deltaTime);
        //if ((lastJump * elapsed) - lastJump < .2f)
        if ((lastJump < .2f) && (gbData.elapsed < .2f))
        {
            wantsToJump = true;
        }

        //jump animation        
    }

    public void JumpTermination()
    {
        if (characterController.isJumping)
        {
            //is timeToApex breached          
            if (gbData.elapsed >= gbData.timeToApex)
            {
                characterController.isJumping = false;
                characterController.isFalling = false;
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
                    characterController.isJumping = false;
                    characterController.isFalling = false;
                }
            }
        }
    }
    private void EdgeJump()
    {
        if (characterController.isGrounded)
        {
            lastGrounded = gbData.elapsed;
        }

        if (wantsToJump && (gbData.elapsed - lastGrounded < 1f))
        {
            InitJump();
        }
    }
}
