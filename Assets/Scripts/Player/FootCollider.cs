using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollider : MonoBehaviour
{
    public Controller cController;
    private CircleCollider2D cCollider;

    private void Start()
    {        
        cCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        //floor collision
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("FootCollider OnTriggerEnter2D");
            cController.IsGrounded = true;
            cController.IsFalling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //fall off ground
        if(collision.gameObject.layer == 9)
        {
            Debug.Log("FootCollider OnTriggerExit2D");            
            cController.IsGrounded = false;
            cController.IsFalling = true;                
        }
    }
}
