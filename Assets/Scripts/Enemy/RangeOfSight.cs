using UnityEngine;

public class RangeOfSight : MonoBehaviour
{
    bool isActive = false;
    public bool playerSeen = false;
    float perimeter;

    CircleCollider2D cCollider;

    private void Start()
    {
        cCollider = GetComponent<CircleCollider2D>();
        ResetPerimeter();
    }
    public void CalculatePerimeter(float radius)
    {
        //get perimeter
        //perimeter = 2 * Mathf.PI * r;
        perimeter = radius;
    }
    public void PerimeterDetection()
    {
        cCollider.radius = perimeter;
        isActive = true;
    }

    private void ResetPerimeter()
    {
        cCollider.radius = 1f;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.gameObject.layer == 12)
            {
                Debug.Log("Player in visual range");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.gameObject.layer == 12)
            {
                Debug.Log("Player left visual range");
                isActive = false;
            }
        }
    }
}
