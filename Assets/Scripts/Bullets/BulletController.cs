using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
    //public forevah!
    public bool isActive;    

    private SpriteRenderer sRender;
    private CircleCollider2D cCollider;    
    private GameboardData gbData;

    public BulletData bData;

    private Vector3[] lines;
    private int count=0;

    private void Start()
    {
        sRender = GetComponent<SpriteRenderer>();
        cCollider = GetComponent<CircleCollider2D>();
        bData = GetComponent<BulletData>();
        Reset();
    }   

    public void AssignBullet(Vector3[] _lines, GameboardData _gbData, LayerMask _shooter, LayerMask _collisionMasks)
    {
        bData.target = _lines[0];
        bData.origin = _lines[_lines.Length-1];
        bData.shooter = _shooter;
        bData.collisionMasks = _collisionMasks;

        gbData = _gbData;
        lines = _lines;        

        transform.position = _lines[0];
        sRender.enabled = true;
        cCollider.enabled = true;

        FireActions();
    } 
    private void Reset()
    {
        bData.target = Vector2.zero;
        bData.origin = Vector2.zero;
        lines = new Vector3[0];

        transform.localPosition = Vector2.zero;
        sRender.enabled = false;
        cCollider.enabled = false;
        count = 0;

        isActive = false;
    }

    private IEnumerator MoveProjectile()
    {
        for (int i = 0; i < count; i++)
        {
            transform.position = lines[i];
            yield return new WaitForSeconds(.01f);
        }

        //do effects from when projectile strikes target
        Reset();
    }    
    public void FireActions()
    {
        Debug.Log("FireActions");
        isActive = true;
        count = lines.Length;
        StartCoroutine(MoveProjectile());
    }
}
