using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class patrolPositions
{
    public Vector3 pos;
    public LayerMask collidedWith;
    public bool goingLeft;

    public patrolPositions(Vector3 _pos, LayerMask _collidedWith, bool _goingLeft)
    {
        pos = _pos;
        collidedWith = _collidedWith;
        goingLeft = _goingLeft;
    }
}

public class Patrol : MonoBehaviour
{
    private bool canSwap;
    public float range;

    private bool goingLeft;

    private Vector3 start;
    private Vector3 left;
    private Vector3 right;

    private LayerMask canHit;

    private List<patrolPositions> rightPositionsList;
    private List<patrolPositions> lefttPositionsList;

    public bool GoingLeft()
    {
        return goingLeft;
    }

    public void SetPatrolLocations(Vector3 _startPos, LayerMask _canHit, float _range)
    {
        range = _range;
        canHit = _canHit;        
        
        rightPositionsList = new List<patrolPositions>();
        lefttPositionsList = new List<patrolPositions>();

        CheckEveryPosition(_startPos);
    }
    private void CheckEveryPosition(Vector3 startPos)
    {
        //create lists of all unique directional points
        int i = 0;
        while(i<range)
        {
            //set positions for going right
            if(i<(range*.5f))
            {
                //right
                Vector3 hitTarget = new Vector3(startPos.x + i, startPos.y);
                var hit = Physics2D.Linecast(Vector2.up, hitTarget);
                patrolPositions newPos;

                if (hit.collider != null)                
                {
                    
                    if (!rightPositionsList.Any(j => j.collidedWith == hit.collider.gameObject.layer))
                    {
                        float dif = hit.collider.bounds.min.x + startPos.x;
                        Vector2 newRight = new Vector3(dif - .5f, startPos.y);

                        newPos = new patrolPositions(newRight, hit.collider.gameObject.layer, false);
                        rightPositionsList.Add(newPos);
                    }
                }                
            }
            //set positions for going left
            else
            {
                //left
                Vector3 hitTarget = new Vector3(startPos.x - i, startPos.y);
                var hit = Physics2D.Linecast(Vector2.up, hitTarget);
                patrolPositions newPos;

                if (hit.collider != null)
                {
                    if (!lefttPositionsList.Any(j => j.collidedWith == hit.collider.gameObject.layer))
                    {
                        float dif = hit.collider.bounds.max.x - startPos.x;
                        Vector2 newLeft = new Vector3(dif + .5f, left.y);

                        newPos = new patrolPositions(newLeft, hit.collider.gameObject.layer, true);
                        lefttPositionsList.Add(newPos);
                    }
                }
                else
                {
                    if (!lefttPositionsList.Any(j => j.collidedWith == 0))
                    {
                        newPos = new patrolPositions(hitTarget, 0, true);
                        lefttPositionsList.Add(newPos);
                    }
                }
            }

            i++;
        }

        //get max distance from both sides
        DefineStoppingPositions();
    }    
    
    private void DefineStoppingPositions()
    {
        //right
        foreach(patrolPositions i in rightPositionsList)
        {
            switch(i.collidedWith)
            {                
                case 9:
                    right = new Vector3((i.pos.x - .5f), start.y);
                    break;
                case 15:
                    right = new Vector3((i.pos.x - .3f), start.y);
                    break;
            }
        }

        //left        
        foreach (patrolPositions i in lefttPositionsList)
        {
            switch (i.collidedWith)
            {
                case 8:
                    left = new Vector3((i.pos.x + .5f), start.y);
                    break;
                case 15:
                    left = new Vector3((i.pos.x + .3f), start.y);
                    break;                
            }
        }
    }

    public Vector3 GetCurrentDirection(Vector2 curPos)
    {   
        if(goingLeft)
        {
            canSwap = (curPos.x <= left.x);
            if (!canSwap)
                return left;
        }
        else
        {
            canSwap = (curPos.x >= right.x);
            if (!canSwap)
                return right;
        }

        if(canSwap)
        {
            canSwap = false;
            if (goingLeft)
            {
                goingLeft = false;
                return right;
            }
            else
            {
                goingLeft = true;
                return left;
            }
        }

        return curPos;
    }
}
