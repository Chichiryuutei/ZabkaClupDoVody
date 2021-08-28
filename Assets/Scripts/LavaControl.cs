using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaControl : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = .5f;
    public string parentName;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //private void FixedUpdate()
    //{
    //    newPosition = rb.position + new Vector2(0, speed * Time.deltaTime);
    //    rb.MovePosition(newPosition);
    //}

    void Update()
    {
        //position = new Vector2(0, transform.position.y + speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default") || collision.gameObject.layer == LayerMask.NameToLayer("Sticky"))
        {
            parentName = collision.gameObject.transform.name;
            collision.gameObject.transform.DetachChildren();
            Destroy(collision.gameObject);
        }
    }
}
