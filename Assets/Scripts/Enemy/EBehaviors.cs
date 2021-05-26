using UnityEngine;

//initiate basic behaviors of AI (enemies, etc)
public class EBehaviors : MonoBehaviour
{
    EData eData;
    EController eController;

    private bool isIdle = true;
    private bool isPatroling = false;

    private Vector2 startPos;
    private Vector2 endPos;

    public LayerMask canHit;

    private void Start()
    {
        eController = GetComponent<EController>();
        eData = GetComponent<EData>();
    }

    private void Update()
    {
        if (isPatroling)
        {
            //TODO switch positions
            PatrolSwap();
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

    void SetMovements(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
    }

    void PatrolSwap()
    {

    }

    void StartPatrol()
    {
        SetMovements(this.transform.position, new Vector2(startPos.x + 50f, startPos.y));
        Patrol();
    }

    void Patrol()
    {
        //detect any walls that block destination
        WallCheck();

        //detect any gaps in floor

        eController.finalVelocity = endPos * .2f;
        isPatroling = true;
    }

    private void WallCheck()
    {
        Debug.Log("Start " + startPos);
        Debug.Log("WallCheck " + endPos);

        Vector3 hitTarget = new Vector3(endPos.x, startPos.y);
        var hit = Physisc2D.Linecast(Vector2.up, hitTarget);

        if(hit.collider != null)
        {
            //TODO define canHit in Unity
            if((canHit & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
            {
                if(endPos.x < 0)
                {
                    float dif = hit.collider.bounds.max.x - startPos.x;
                    endPos = Vector3(dif + .5f, endPos.y);
                }
                else
                {
                    float dif = hit.collider.bounds.min.x - startPos.x;
                    endPos = Vector3(dif - .5f, endPos.y);
                }
            }
        }
    }
}
