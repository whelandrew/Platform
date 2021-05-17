using System.Collections;
using UnityEngine;

public class HeadCollider : MonoBehaviour
{
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
    }
}
