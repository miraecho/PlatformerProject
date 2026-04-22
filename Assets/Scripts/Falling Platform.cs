using Unity.VisualScripting;
using UnityEngine;

public class FallingPlatformer : MonoBehaviour
{
    public float timeBeforeFall;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            transform.AddComponent<Rigidbody2D>();
            Destroy(transform.parent.gameObject, 3);
        }
    }
}
