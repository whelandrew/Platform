using UnityEngine;

//initiate basic behaviors of AI (enemies, etc)
public class EBehaviors : MonoBehaviour
{
    private EData eData;
    private EController eController;
    public LineOfSight lineOfSight;

    public Scan scan;
    Patrol patrol;
    Alert alert;
    Pursue pursue;
    Attack attack;

    private bool isIdle = true;
    private bool isPatroling = false;
    private bool isScanning = false;
    private bool seesPlayer = false;
    private bool isPursuing = false;

    private Vector2 startPos;
    private Vector2 endPos;

    public Vector3 pursueTarget;

    public LayerMask canHit;

    private void Start()
    {
        eController = GetComponent<EController>();
        eData = GetComponent<EData>();
        patrol = new Patrol();
        alert = new Alert();
        pursue = new Pursue();
        attack = new Attack();
    }

    private void Update()
    {
        seesPlayer = lineOfSight.playerSeen;
        if(seesPlayer)
        {
            //SetPursue();            
        }
        else
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
                //StartScan();
                //A - Alert
                //B - Pursue
                   //1 - Attack
            
            //B - Stationary
                //1 - Scan
                    //A - Alert
                    //B - Pursue
                        //1 - Attack

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

    private void SetPursue()
    {
        if (isPursuing)
        {
            Debug.Log("Pursuing");
            //eController.finalVelocity = pursueTarget * 0.2f;
            eController.finalVelocity = pursue.UpdatePursuitDirection(this.transform.position) * 0.8f;
        }
        else
        {
            isPursuing = true;
            //pursueTarget = lineOfSight.targetLoc;
            pursue.SetTarget(lineOfSight.targetLoc, this.transform.position);
        }        
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
