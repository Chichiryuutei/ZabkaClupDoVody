using UnityEngine;

public class PlaterMovement : MonoBehaviour
{

    public CharacterController m_Controller;
    public int player = 1;

    void Update()
    {
        // Check for control input and call appropriate controller functions

        if (Input.GetMouseButtonDown(0))
        {
            // Point click jumpy movement -> get vector from player to mouse position and apply force in that direction
            // All physics and force movement handled throgh CharacterController.cs
            Vector3 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dirToMouse = Input.mousePosition - posInScreen;
            dirToMouse.Normalize();
            m_Controller.MouseMove(dirToMouse);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dirToMouse = Input.mousePosition - posInScreen;
            dirToMouse.Normalize();
            m_Controller.TongueGrapple(dirToMouse);
        }
    }
}
