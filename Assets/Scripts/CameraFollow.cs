using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTarget;
    Rigidbody2D playerRb;
    public Transform lavaTarget;

    float smoothSpeed = 0.0f;

    public Vector3 offset;

    private void Start()
    {
        playerRb = playerTarget.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        smoothSpeed = Mathf.Abs(playerRb.velocity.y) + 5.0f;
        Vector3 desiredPosition = new Vector3(0, playerTarget.position.y, -10) + offset;
        if (desiredPosition.y < lavaTarget.transform.position.y)
        {
            desiredPosition.y = lavaTarget.transform.position.y;
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }


}
