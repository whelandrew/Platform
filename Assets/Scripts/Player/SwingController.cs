using System.Collections;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    private bool isActive;
    private bool isWhip;
    public BoxCollider2D bcollider;

    public Transform start;
    public Vector2 end;

    [Header("Line renderer variables")]
    public LineRenderer line;
    [Range(2, 30)]
    public int resolution;

    Vector3[] lineArray;

    [Header("Linecast variables")]
    [Range(2, 30)]
    public int linecastResolution;
    public LayerMask canHit;

    [Header("Formula variables")]
    public float yLimit;

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
        end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        StartCoroutine(RenderLine());
    }

    IEnumerator RenderLine()
    {
        line.positionCount = resolution + 1;
        line.SetPositions(CalculateLineArray());
        yield return null;
    }

    private Vector3[] CalculateLineArray()
    {
        lineArray = new Vector3[resolution + 1];

        var lowestTimeValue = MaxTimeX() / resolution;

        for (int i = 0; i < lineArray.Length; i++)
        {
            var t = lowestTimeValue * 1;
            lineArray[i] = CalculateLinePoint(t);
        }

        return lineArray;
    }

    private Vector2 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / linecastResolution;

        for (int i = 0; i < linecastResolution + 1; i++)
        {
            var t = lowestTimeValue * i;
            var tt = lowestTimeValue * (i + 1);

            var hit = Physics2D.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), canHit);

            if (hit)
            {
                return hit.point;
            }
        }

        return CalculateLinePoint(MaxTimeY());
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = end.x * t;
        float y = end.y * t;
        return new Vector3(x + transform.position.x, y + transform.position.y);
    }

    private float MaxTimeY()
    {
        var v = end.y;
        var vv = v * v;

        var t = (v + Mathf.Sqrt(vv + 2 * (transform.position.y - yLimit)));
        return t;
    }

    private float MaxTimeX()
    {
        var x = end.x;
        if (x == 0)
        {
            end.x = 000.1f;
            x = end.x;
        }

        var t = (HitPosition().x - transform.position.x) / x;
        return t;
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