using UnityEngine;

public class PlatformMoveHorizontal : MonoBehaviour
{
    float speed = 1f;
    public float distance = 2;

    Vector2 newPosition;
    Vector2 endPositionRight;
    Vector2 endPositionLeft;
    Rigidbody2D rb;

    bool movingRight;


    void Start()
    {
        movingRight = true;
        rb = GetComponent<Rigidbody2D>();
        endPositionRight = new Vector2(transform.position.x + distance, transform.position.y);
        endPositionLeft = new Vector2(transform.position.x - distance, transform.position.y);
    }

    void Update()
    {
        if (movingRight)
        {
            newPosition = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
            //newPosition = rb.position + new Vector2(speed * Time.deltaTime, 0);
        }
        else
        {
            newPosition = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
            //newPosition = rb.position + new Vector2(-speed * Time.deltaTime, 0);
        }

        if (newPosition.x > endPositionRight.x || newPosition.x < endPositionLeft.x)
        movingRight = !movingRight;

        //rb.MovePosition(newPosition);


        transform.position = newPosition;
    }

}
