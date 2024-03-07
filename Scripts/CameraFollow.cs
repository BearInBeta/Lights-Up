using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target to follow
    public float smoothSpeed = 0.125f; // The smoothness of the camera movement
    public Vector3 offset; // The offset from the target

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = new Vector3(target.position.x, transform.position.y, transform.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

    }
}
