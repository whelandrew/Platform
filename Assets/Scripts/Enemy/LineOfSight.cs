using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    private CircleCollider2D cCollider;
    public bool playerSeen = false;

    public bool targetSeen = false;
    public Vector3 targetLoc;

    void Start()
    {
        cCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if(collision.gameObject.layer == 12)
        {
            Debug.Log("<color=#ff0000ff>Player Seen</color>");
            playerSeen = true;
            targetSeen = true;
            targetLoc = collision.gameObject.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            //playerSeen = false;            
        }
    }
}
