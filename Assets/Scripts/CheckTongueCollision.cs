using UnityEngine;

public class CheckTongueCollision : MonoBehaviour
{
    public bool m_IsTriggered;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            m_IsTriggered = true;
            this.transform.parent = collision.transform;
        }

    }
}
