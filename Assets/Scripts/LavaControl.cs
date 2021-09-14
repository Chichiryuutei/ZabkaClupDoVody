using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaControl : MonoBehaviour
{
    public float speed = .5f;
    public string parentName;
    public bool gameOver;

    void Update()
    {
        if (!gameOver)
        {
            if (speed >= 2) speed = 2;
            transform.position = new Vector2(0, transform.position.y + speed * Time.deltaTime);
        }
    }
}
