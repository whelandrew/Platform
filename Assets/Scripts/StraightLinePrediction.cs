using System.Collections;
using UnityEngine;

public class StraightLinePrediction : MonoBehaviour
{
    public bool showLine;
    public Transform start;
    private LineRenderer line;
    public LayerMask canHit;
    public float xCap;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (showLine)
        {            
            StartCoroutine(RenderArc());
        }
    }

    private IEnumerator RenderArc()
    {
        Vector3[] arr = new Vector3[] { start.position, HitPosition() };
        line.SetPositions(arr);
        yield return null;
    }

    private Vector2 HitPosition()
    {
        
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mouse;
        /*
        Vector2 end = new Vector2((mouse.x * xCap) * Time.deltaTime, mouse.y);

        var hit = Physics2D.Linecast(end, Vector2.up);
        
        if (hit.collider != null)
        {            
            if ((canHit & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
            {
                //Debug.Log("Hit " + hit.collider);
                return hit.point;
            }
        }

        return end;
        */
    }
}
