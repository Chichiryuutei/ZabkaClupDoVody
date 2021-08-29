using UnityEngine;

public class CheckTongueCollision : MonoBehaviour
{
    public bool m_IsTriggered;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player") && 
            collision.gameObject.layer != LayerMask.NameToLayer("Tongue") && 
            collision.gameObject.layer != LayerMask.NameToLayer("PlatformGenerator"))
        {
            //Debug.Log(collision.gameObject.layer + " " + collision.gameObject.transform.name);
            m_IsTriggered = true;
            this.transform.parent = collision.transform;
        }

    }
}
