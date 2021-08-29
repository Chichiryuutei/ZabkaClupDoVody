using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAddMoreCheck : MonoBehaviour
{
    public bool m_ColliderTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_ColliderTriggered = true;
        }
    }
}
