using UnityEngine;

//initiate basic behaviors of AI (enemies, etc)
public class EBehaviors : MonoBehaviour
{
    private EData eData;
    private EController eController;

    public Scan scan;
    Patrol patrol;    

    private bool isIdle = true;
    private bool isPatroling = false;
    private bool isScanning = false;

    private Vector2 startPos;
    private Vector2 endPos;

    public LayerMask canHit;

    private void Start()
    {
        eController = GetComponent<EController>();
        eData = GetComponent<EData>();
        patrol = new Patrol();
    }

    private void Update()
    {
        if(isScanning)
        {

        }
        else
        if (isPatroling)
        {
            //TODO switch positions
            eController.finalVelocity = patrol.GetCurrentDirection(this.transform.position) * 0.8f;
        }
    }

    //receive values to progress character behaviors
    public void BehaviorStyles()
    {
        if(Idle())
        {
            isIdle = false;
            //0 - Idle

            // STARTING ACTIONS
            //A - Patrol            
            //StartPatrol();
                //1 - Scan
                StartScan();
                    //A - Alert
                    //B - Attack
            //B - Stationary
                //1 - Scan
                    //A - Alert
                    //B - Attack

            //REACTIONS
            //1 - Timid
                //A - Flee
                //B - Pursue
            //2 - Calm
                //A - Stay
                //B - Pursue
            //3 - Aggro
                //A - Stay
                //B - Pursue
        }
    }

    private bool Idle()
    {
        return isIdle;
    }

    private void StartScan()
    {
        if(isScanning)
        {
            scan.SetupScan(this.transform.localPosition, 30, Facing());
        }
    }

    private void StartPatrol()
    {
        patrol.SetPatrolLocations(transform.position, canHit, 50f);

        eController.finalVelocity = patrol.GetCurrentDirection(this.transform.position) * 0.2f;

        isPatroling = true;
    }

    public int Facing()
    {
        if(isPatroling)
            if (patrol.GoingLeft())
                return -1;        

        return 1;
    }
}
