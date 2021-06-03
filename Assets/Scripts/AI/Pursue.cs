using UnityEngine;
using Tools;

public class Pursue : MonoBehaviour
{
    private Vector3 target;
    bool facingLeft;

    public void SetTarget(Vector3 _target, Vector3 _curLoc)
    {
        this.target = _target;
        facingLeft = AI.FacingLeft(_curLoc, this.target);
    }

    public Vector3 UpdatePursuitDirection(Vector3 curLoc)
    {
        Vector3 newPath = new Vector3((curLoc.x + target.x), target.y);
        float distance = Vector3.Distance(curLoc, target);
        if(distance<=.1f)
        {
            newPath = Vector3.zero;
        }

        return newPath;
    }
}
