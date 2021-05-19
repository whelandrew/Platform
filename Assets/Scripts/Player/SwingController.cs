using System.Collections;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    private bool isActive;
    private bool isWhip;
    public BoxCollider2D bcollider;

    public StraightLinePrediction slp;

    private void Start()
    {
        Reset();
    }

    private void Update()
    {        
        //if (isActive)
        {
            //if (isWhip)
            {
                WhipTargeting();
            }
        }
    }

    

    private void WhipTargeting()
    {
        
    }

    private void Reset()
    {
        isActive = false;
        bcollider.enabled = false;
    }
    public void Sword()
    {
        isActive = true;
        bcollider.enabled = true;

        StartCoroutine(SwingSword());
    }

    public void Whip()
    {
        isActive = true;
        isWhip = true;
        bcollider.enabled = true;
        slp.showLine = true;
    }

    IEnumerator SwingWhip()
    {
        //do animation stuff
        yield return new WaitForSeconds(1f);
        Reset();
        yield return null;
    }

    IEnumerator SwingSword()
    {
        Debug.Log("SwingSword");
        //do animation stuff
        yield return new WaitForSeconds(1f);
        Reset();
        yield return null;
    }
}