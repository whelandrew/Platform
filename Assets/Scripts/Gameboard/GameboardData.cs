using UnityEngine;

public class GameboardData : MonoBehaviour
{
    public float gravity = 9.81f;
    public Vector2 cursorPos;

    private void Update()
    {
        cursorPos = Input.mousePosition;
    }
}
