using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTarget;

    public float smoothSpeed = 0.125f;

    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(0, playerTarget.position.y, -10) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }


}
