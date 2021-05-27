using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFootCollider : MonoBehaviour
{
    public EController eController;
    public CapsuleCollider2D cCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //floor collision
        if (collision.gameObject.layer == 9)
        {
            //Debug.Log("FootCollider OnTriggerEnter2D");
            eController.isGrounded = true;
            eController.isFalling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //fall off ground
        if (collision.gameObject.layer == 9)
        {
            eController.isGrounded = false;

            //TODO register which side of ground is touched
        }
    }
}