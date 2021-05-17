using UnityEngine;

public class ShootController : MonoBehaviour
{
    public GameObject BulletCache;
    public Transform firePoint;
    public GameboardData gbData;
    public LayerMask ControlOwner;

    private BulletController[] bullets;

    private bool delayCounterOn;
    private float delayCount=0f;
    public float fireDelay = 1f;

    BulletController activeBullet;

    public ProjectileTrajectory pt;

    public LayerMask colliderMasks;

    private void Update()
    {
        if (delayCounterOn)
        {
            delayCount += Time.deltaTime;
            if(delayCount >= fireDelay)
            {
                delayCounterOn = false;
                delayCount = 0;
            }
        }
    }

    private void Start()
    {
        bullets = BulletCache.GetComponentsInChildren<BulletController>();
        //pt.traceTrajectory = true;
    }

    public void FireWeapon()
    {
        if (delayCount == 0)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (!bullets[i].isActive)
                {                    
                    delayCounterOn = true;
                    activeBullet = bullets[i];
                    activeBullet.AssignBullet(pt.lineArray, gbData, ControlOwner, colliderMasks);
                    break;
                }
            }            
        }
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
