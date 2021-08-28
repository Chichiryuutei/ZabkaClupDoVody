using UnityEngine;

public class PlatformMoveVertical : MonoBehaviour
{
    float speed = 1f;
    public float distance = 2;

    Vector2 newPosition;
    Vector2 endPositionRight;
    Vector2 endPositionLeft;
    Rigidbody2D rb;

    bool movingUp;


    void Start()
    {
        movingUp = true;
        rb = GetComponent<Rigidbody2D>();
        endPositionRight = new Vector2(transform.position.x, transform.position.y + distance);
        endPositionLeft = new Vector2(transform.position.x, transform.position.y - distance);
    }

    void FixedUpdate()
    {
        if (movingUp)
        {
            //newPosition = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);

            newPosition = rb.position + new Vector2(0, speed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
        else
        {
            //newPosition = new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime);
            newPosition = rb.position + new Vector2(0, -speed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }

        if (newPosition.y >= endPositionRight.y || newPosition.y <= endPositionLeft.y)
            movingUp = !movingUp;


        transform.position = newPosition;
    }

}

