using UnityEngine;

//initiate basic behaviors of AI (enemies, etc)
public class EBehaviors : MonoBehaviour
{
    EData eData;
    EController eController;

    Patrol patrol;

    private bool isIdle = true;
    private bool isPatroling = false;

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
            StartPatrol();
            //B - Stationary

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

    void StartPatrol()
    {
        patrol.SetPatrolLocations(transform.position, canHit, 50f);

        eController.finalVelocity = patrol.GetCurrentDirection(this.transform.position) * 0.2f;

        isPatroling = true;
    }
}
