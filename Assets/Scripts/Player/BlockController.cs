using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    private bool isActive;
    public BoxCollider2D bcollider;
    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        isActive = false;
        bcollider.enabled = false;
    }
    public void Block()
    {
        isActive = true;
        bcollider.enabled = true;

        StartCoroutine(BlockActions());
    }

    IEnumerator BlockActions()
    {
        Debug.Log("Block");
        //do animation stuff
        yield return new WaitForSeconds(1f);
        Reset();
        yield return null;
    }
}
