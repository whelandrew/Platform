using UnityEngine;

public class EController : MonoBehaviour
{
    public GameboardData gbData;
    private EBehaviors behaviors;
    private Rigidbody2D rBody;
    public EData eData;

    public bool isGrounded = false;
    public bool isFalling = false;
    public bool isJumping = false;
    public bool canControl = true;

    private int facing = 0;

    public Vector3 finalVelocity;
    void Start()
    {
        behaviors = GetComponent<EBehaviors>();
        rBody = GetComponent<Rigidbody2D>();
        eData = GetComponent<EData>();
    }

    void Update()
    {     
        //is character on the ground
        if (isGrounded)
        {
            Facing();
            //is player controllable (pause actions for cutscenes, etc)
            if (canControl)
            {
                //perform behaviors (will be based off of eData)
                behaviors.BehaviorStyles();
            }
        }
        else
        {
            if (canControl)
            {
            }
            
            //fall to ground
            Fall();
        }
    }

    private void FixedUpdate()
    {
        rBody.velocity = finalVelocity;
    }

    private void Facing()
    {
        if (behaviors.Facing() > 0)
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
            //Debug.Log("Enemy Fall");

            float fallG = (2 * gbData.fallSpeed) / (gbData.timeToApex * gbData.timeToApex);
            finalVelocity = new Vector2(finalVelocity.x, -fallG);
            isFalling = true;

            //falling animation
        }
    }
}

