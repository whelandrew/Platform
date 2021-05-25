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

    private void Start()
    {
        eController = GetComponent<EController>();
        eData = GetComponent<EData>();
    }

    private void Update()
    {
        if (isPatroling)
        {
            Debug.Log(transform.position);
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
            Patrol();
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

    void Patrol()
    {
        startPos = this.transform.position;
        endPos = new Vector2(startPos.x + 50, startPos.y);

        //detect any walls that block destination
        //detect any gaps in floor

        eController.finalVelocity = endPos*.2f;

        Debug.Log(endPos * .2f);

        isPatroling = true;
    }

    void WallCheck()
    {

    }
}
