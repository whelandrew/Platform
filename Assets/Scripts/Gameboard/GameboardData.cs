using UnityEngine;

public class GameboardData : MonoBehaviour
{
    public float gravity = 9.81f;
    public Vector2 cursorPos;
    public float fallSpeed = 10f;
    public float timeToApex = 1f;

    public float elapsed = 0f;
    public float elapsedMax = 0.03f;

    private void Update()
    {
        cursorPos = Input.mousePosition;
    }
}
