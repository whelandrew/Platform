using UnityEngine;

//initiate basic behaviors of AI (enemies, etc)
public class EBehaviors : MonoBehaviour
{

    private bool isIdle = true;
    //receive values to progress character behaviors
    public void BehaviorStyles()
    {
        if(isIdle)
        {
            isIdle = false;
            //0 - Idle

            // STARTING ACTIONS
            //A - Patrol
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



}
