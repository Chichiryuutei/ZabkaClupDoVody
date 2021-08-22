using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaterMovement : MonoBehaviour
{

    public CharacterController m_Controller;

    public float m_RunSpeed = 40f;

    float m_HorizontalMove = 0f;
    bool m_CanJump = false;
    bool m_CanDoubleJump = false;
    bool m_Moving = false;
    Vector3 m_LastClickedPos;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
    }

    // Update is called once per frame
    void Update()
    {
        m_HorizontalMove = Input.GetAxisRaw("Horizontal") * m_RunSpeed;


        if (Input.GetButtonDown("Jump")){
            m_CanDoubleJump = true;
            m_CanJump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //m_LastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector3 mouseDir = m_LastClickedPos - gameObject.transform.position;
            //mouseDir.z = 0.0f;
            //mouseDir = mouseDir.normalized;
            //m_Moving = true;
            //m_Controller.MouseMove(mouseDir, false, false);
            //Debug.Log(mouseDir);
            Vector3 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dirToMouse = Input.mousePosition - posInScreen;
            dirToMouse.Normalize();
            GetComponent<Rigidbody2D>().AddForce(new Vector2(dirToMouse.x * 200, 500));
            Debug.Log(counter++);

        }
    }

    private void FixedUpdate()
    {
        //Move character
        //m_Controller.Move(m_HorizontalMove * Time.fixedDeltaTime, m_CanJump, false);
        //m_CanJump = false;
    }
}
