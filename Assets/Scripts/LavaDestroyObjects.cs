using UnityEngine;

public class LavaDestroyObjects : MonoBehaviour
{

    public string parentName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default") || collision.gameObject.layer == LayerMask.NameToLayer("Sticky"))
        {
            parentName = collision.gameObject.transform.name;
            collision.gameObject.transform.DetachChildren();
            Destroy(collision.gameObject);
        } 
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Fly"))
        {
            Destroy(collision.gameObject);
        }
    }
}
