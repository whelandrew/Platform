using UnityEngine;

public class BulletData : MonoBehaviour
{
    public FireType fireType;
    public Vector2 origin;
    public Vector2 target;
    public float speed = 0.5f;

    public float maxVelocity = 5f;
    public float damp = 0.5f;
    public float launchHeight = 2f;
    public float timeToApex = .7f;

    public LayerMask shooter;
    public LayerMask collisionMasks;

}
