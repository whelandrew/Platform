using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//parent for character controls (all character-related classes go through here)
//movement
//animations
//stats
public class Controller : MonoBehaviour
{
    private float noInputTimer = 0;

    private Rigidbody2D rigidBody;
    private CapsuleCollider2D cCollider;    

    private Vector2 direction = Vector2.zero;
    private Vector2 moveDir = Vector2.zero;

    private bool stopMovement;
    private bool isGrounded;
    private bool isJumping;
    Vector2 groundTarget = Vector2.zero;
    Vector2 jumpTarget = Vector2.zero;

    private float moveSpeed = 7f;
    private float fallSpeed = 10f;

    private float jumpVelocity = 8f;
    private float jumpMagnitude = 8f;
    

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        cCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {        
        if (!StopMovement())
        {            
            if (Input.GetKey(KeyCode.Space))
            {
                if (isGrounded)
                    if (!isJumping)
                        Jumping();

                if (isJumping)
                    transform.Translate(jumpTarget * Time.deltaTime);
            }
            else
            {
                //falling
                if (!isGrounded)
                {
                    transform.Translate((Vector2.down * fallSpeed) * Time.deltaTime);
                    jumpTarget = Vector2.zero;
                    isJumping = false;
                }
                else
                {
                    Moving();
                }
            }
        }
    }

    private bool StopMovement()
    {
        noInputTimer += Time.deltaTime;

        if (!Input.anyKey)
        {
            noInputTimer += 1 * Time.deltaTime;
        }
        else
        {
            stopMovement = false;
            noInputTimer = 0;
        }

        if (noInputTimer >= 1)
        {            
            if (!isGrounded)
            {
                return false;
            }
            else
            {
                stopMovement = true;
                return true;
            }
        }

        return false;
    }

    private void Jumping()
    {        
        isJumping = true;
        isGrounded = false;
        groundTarget = transform.position;

        if (jumpTarget == Vector2.zero)
        {
            //TODO jump in x directions
            float xMag = jumpMagnitude * direction.x;
            jumpTarget = new Vector2(transform.position.x*xMag, transform.position.y + jumpVelocity);
        }

        //TODO cap jump height
    }

    private void Moving()
    {        
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
    }

    private void FixedUpdate()
    {
        if (stopMovement)
        {
            rigidBody.velocity = Vector2.zero;
        }
        else
        {
            if (isGrounded)
            {
                rigidBody.velocity = moveDir * moveSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //floor collision
        isGrounded = (collision.gameObject.layer == 9);
        if (isGrounded)
        {
           isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded =! (collision.gameObject.layer == 9);
    }
}
