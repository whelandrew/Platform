﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFootCollider : MonoBehaviour
{
    public EController eController;
    public CapsuleCollider2D cCollider;

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
                Physics2D.IgnoreCollision(collision, cCollider);
            }
        }

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