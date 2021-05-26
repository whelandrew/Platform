using UnityEngine;


//parent for character controls (all character-related classes go through here)
//movement
//animations TODO - animations for sides of ground & walls, ceiling & bottom of ground
//stats TODO
public class Controller : MonoBehaviour
{
    public GameboardData gbData;

    private ShootController shootController;
    private SwingController swingController;
    private JumpController jController;

    //for tracking keys used in movement
    public KeyCode[] MappedKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };
    public LayerMask canHit;

    CapsuleCollider2D cCollider;
    Rigidbody2D rBody;

    public bool isWalking = false;
    public bool isGrounded = false;
    public bool isFalling = false;
    public bool isJumping = false;
    public bool isDashing = false;

    public Vector2 finalVelocity;

    public bool canControl = true;
    private float inputTimer = 0f;
    public float inputTimerMax = 2f;

    private Vector2 direction = Vector2.zero;
    public Vector2 moveDir = Vector2.zero;
    public float moveSpeed = 7f;
    public int facing = 1;    
    public float dashDistance = 10f;

    private void Start()
    {
        cCollider = GetComponent<CapsuleCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        shootController = GetComponent<ShootController>();
        swingController = GetComponent<SwingController>();
        jController = GetComponent<JumpController>();
    }

    private void Update()
    {
        gbData.elapsed += Time.deltaTime;

        //is character on the ground
        if (isGrounded)
        {
            Facing();
            //is player controllable (pause actions for cutscenes, etc)
            if (canControl)
            {
                Dash();
                //Debug.Log("Character can be controlled");
                if (Input.GetKey(KeyCode.Space) && !isJumping)
                {
                    jController.InitJump();
                }

                if (isJumping)
                {
                    jController.JumpCalc();
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
            if (canControl)
            {
                Fire();
            }

            //post jump
            jController.JumpTermination();
            //jController.EdgeJump();

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
            //shootController.FireWeapon();          
            //swingController.Sword();            
        }
    }

    private void StopDash()
    {
    }
    /*
    if (isDashing)
    {
        if (facing > 0)
        {
            if (transform.localPosition.x >= finalVelocity.x)
            {
                isDashing = false;
                finalVelocity = Vector2.zero;
            }
        }
        else
        {
            if (transform.localPosition.x <= finalVelocity.x)
            {
                isDashing = false;
                finalVelocity = Vector2.zero;
            }
        }
    }
}
    */
    private void Dash()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if (!isDashing)
            {
                isDashing = true;

                Vector3 target = Vector3.zero;
                if (target.x == 0)
                {
                    target = new Vector3(dashDistance * facing, 0);
                }
                else
                {
                    target = new Vector3(transform.position.x * facing * dashDistance, 0);
                }


                Vector3 final = DashBreak(target);
                Debug.Log("Final " + final);
                transform.position += final;
                //transform.position -= target;

                //Vector2 dashTarget = new Vector2(facing * transform.localPosition.x, transform.position.y);
                //finalVelocity = DashBreak(dashTarget);                        
            }
        }
    }

    private Vector3 DashBreak(Vector3 target)
    {
        Debug.Log("Start " + transform.position);
        Debug.Log("DashBreak " + target);
        Vector3 hitTarget = new Vector3(target.x, transform.position.y);
        var hit = Physics2D.Linecast(Vector2.up, hitTarget);
        if (hit.collider != null)
        {
            if ((canHit & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
            {                

                //Debug.Log("DashBreak " + hit.collider);
                if (target.x < 0)
                {
                    //float dif = Vector3.Distance(hit.collider.bounds.max, transform.position);
                    float dif = hit.collider.bounds.max.x - transform.position.x;
                    Debug.Log(dif);
                    return new Vector3(dif + .5f, target.y);
                }
                else
                {
                    float dif = hit.collider.bounds.min.x - transform.position.x;
                    return new Vector3(dif - .5f, target.y);
                }
            }
        }

        return target;
    }
    
    private bool Walking()
    {
        //Debug.Log("Moving");
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
            facing = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //right
            direction = Vector2.right;
            facing = 1;
        }

        moveDir = direction;
        //play movement animations

        finalVelocity = moveDir * moveSpeed;
        isWalking = true;

        return direction != Vector2.zero;
    }

    private void Facing()
    {
        if (facing > 0)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    private void Fall()
    {
        if (!isFalling && !isGrounded && !isJumping)
        {
            //Debug.Log("Fall");

            float fallG = (2 * gbData.fallSpeed) / (gbData.timeToApex * gbData.timeToApex);
            finalVelocity = new Vector2(finalVelocity.x, -fallG);
            isFalling = true;

            //falling animation
        }
    }

   

    //checks if there has been user input, or pauses movement
    private bool CanMove()
    {
        inputTimer += 1f * Time.deltaTime;
        
        //if(isDashing)
        //{
        //    return false;
        //}

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
            //Debug.Log("Controller OnTriggerEnter2D");
            //cancel jumping when colliding with floor layer
            if (isJumping)
            {
                //isDashing = false;
                isJumping = false;
                gbData.fallSpeed = 2f;

                //sliding on the wall animation
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //change fallSpeed if off ground
        if (collision.gameObject.layer == 9)
        {
            //Debug.Log("Collision OnTriggerExit2D");
            gbData.fallSpeed = 6f;
        }        
    }
}