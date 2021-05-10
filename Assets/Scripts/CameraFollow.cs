using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 10f;
    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, (smoothSpeed*Time.deltaTime));
        transform.position = smoothPos;

        transform.LookAt(target);
    }

}
